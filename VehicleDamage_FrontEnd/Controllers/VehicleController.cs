using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VehicleDamage_FrontEnd.Models;
using VehicleDamage_FrontEnd.Models.DTOs;
using VehicleDamage_FrontEnd.Models.Staff;
using VehicleDamage_FrontEnd.Services.BEService;
using VehicleDamage_FrontEnd.Services.BlobService;

namespace VehicleDamage_FrontEnd.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IBEService _beService;
        private readonly IBlobService _blobService;

        /// <summary>
        /// Constructor for the controller that sets a context for each of the services.
        /// Which service is passed is controlled by the Enviornment in the startup
        /// </summary>
        /// <seealso cref="Startup"/>
        /// <param name="beService"></param>
        /// <param name="blobService"></param>
        public VehicleController(IBEService beService, IBlobService blobService)
        {
            _beService = beService;
            _blobService = blobService;
        }


        // GET: Staff
        /// <summary>
        /// Vehicle List page shows all the vehicles and highlights any with open invesitgations
        /// </summary>
        /// <param name="indexModel">Model that contains the filter params and list of vehicles</param>
        /// <returns></returns>
        //Takes indexModel which contains the filtering options and applies it to the list of vehicles to be shown to the user.
        public async Task<IActionResult> Index([FromQuery] VehIndexModel indexModel = null)
        {
            //Set the DD lists as null to avoid errors 
            IEnumerable<VehicleModel> vehicles = null;
            IEnumerable<VehicleModel> vehicleConcern = null;
            IEnumerable<MakeModel> makeList = null;
            IEnumerable<string> stateList = null;

            try
            {
                //Create model based on the passed values. At first will be null so everything set to default.
                //Once filter submitted, these will have values to filter the vehicles
                VehFilterAPI apiModel = VehFilterAPI.CreateModel(indexModel.selectedState, indexModel.selectedMake, indexModel.lPlateSearch);
                //Call API to get filtered vehicles
                IEnumerable<VehicleDTO> vehicleDTOs = await _beService.GetVehiclesAsync(apiModel);
                vehicles = VehicleModel.CreateModelNumerable(vehicleDTOs);

                //Create seperate list for vehicles with damage concerns so can display them at the top
                vehicleConcern = vehicles.Where(x => x.state == "Under Investigation").ToList();
                //Drop those in the concern list to avoid duplicates
                vehicles = vehicles.Except(vehicleConcern);

                //Call API to get Make lists
                makeList = MakeModel.CreateModelNumerable(await _beService.GetMakesAsync());
                //Create model based off the new lists e.g.
                indexModel = VehIndexModel.CreateModel(vehicles, vehicleConcern, makeList, indexModel.selectedMake, indexModel.selectedState, indexModel.lPlateSearch);
            }
            catch
            {
                vehicles = Array.Empty<VehicleModel>();
                makeList = Array.Empty<MakeModel>();
                stateList = Array.Empty<string>();

                indexModel = VehIndexModel.CreateModel(null, null, null, Guid.Empty, null, null);
            }
            //Return view with model
            return View("VehicleList", indexModel);
        }

        /// <summary>
        /// Shows advanced details of a vehicle including its clock and damage history
        /// </summary>
        /// <param name="lPlate">liscence plate number</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Details(string lPlate)
        {
            //Get vehicle details
            VehicleDTO dto = await _beService.GetVehicleAsync(lPlate);
            //Get Clock History for vehicle
            IEnumerable<ClockHistoryDTO> clkdtos = await _beService.GetClockHistoriesAsync(lPlate);
            //Get damage history for vehicle
            IEnumerable<DamageHistoryDTO> dmgdtos = await _beService.GetDamageAsync(lPlate);
            //Create model
            VehicleDetailModel model = VehicleDetailModel.CreateModel(dto, dmgdtos, clkdtos);

            return View("VehicleDetails", model);
        }

        /// <summary>
        /// Shows the images linked to the selected Damage History entry
        /// </summary>
        /// <param name="damageModel">Selected damage history entry</param>
        /// <returns></returns>
        public async Task<IActionResult> ViewDamageImages(DamageHistoryModel damageModel)
        {
            //Work out Image ID Pretext to use to search
            string imagePreText = ImageDTO.GenerateImagePre(damageModel.Id, damageModel.time.ToString());

            //Search Image DB for images that start with the prefix
            IEnumerable<byte[]> foundImages = await _blobService.GetImages(imagePreText);

            //Set damageModel list of images to that
            damageModel.images = foundImages;

            //If it is an open investigation case
            if (damageModel.resolved == false)
            {
                //Return the resolve view
                return View("VehicleDamageResolve", damageModel);
            }
            else
            {
                //Reutn the view for looking at the images
                return View("VehicleDamageView", damageModel);
            }
        }

        /// <summary>
        /// Resolves a damage history case that is open.
        /// </summary>
        /// <param name="damageModel">Damage model from user selection</param>
        /// <returns></returns>
        public async Task<IActionResult> ResolveDamages(DamageHistoryModel damageModel)
        {
            //Flip resolved to true so in future will only be viewable
            damageModel.resolved = true;

            //Update the db
            string response = await _beService.UpdateDamageHistoryAsync(DamageHistoryDTO.CreateDTO(damageModel));

            if(response != "Success") 
            {
                return RedirectToAction("ViewDamageImages", damageModel);
            }

            //Set the vehicle state to in so can be used again
            VehicleDTO veh = await _beService.GetVehicleAsync(damageModel.lplateNum);
            veh.state = "In";
            response = await _beService.UpdateVehicleAsync(veh);
            if (response != "Success")
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Details", damageModel.lplateNum);
        }

        public async Task<IActionResult> CreateVehicle() 
        {
            VehicleModel model = new VehicleModel();

            //Get makes
            IEnumerable<MakeModel> makeList = MakeModel.CreateModelNumerable(await _beService.GetMakesAsync());

            //Add to dropdown
            ViewBag.selList = makeList;

            return View("VehicleCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle(VehicleModel vehModel) 
        {
            if (vehModel == null || vehModel.licenceNum == null)
            {
                return RedirectToAction("Index");
            }

            //Get makes
            IEnumerable<MakeModel> makeList = MakeModel.CreateModelNumerable(await _beService.GetMakesAsync());

            VehicleDTO searchdto = await _beService.GetVehicleAsync(vehModel.licenceNum);

            if(searchdto != null) 
            {
                //Add to dropdown
                ViewBag.selList = makeList;
                return View("VehicleCreate", vehModel);
            }

            vehModel.state = "In";


            VehicleDTO dto = VehicleDTO.CreateDTO(vehModel);

            string response = await _beService.InsertVehicleAsync(dto);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> EditVehicle(string lPlate) 
        {
            if(lPlate == null) 
            {
                return RedirectToAction("Index");
            }

            VehicleDTO dto = await _beService.GetVehicleAsync(lPlate);
            VehicleModel model = VehicleModel.CreateModel(dto);

            return View("VehicleEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicle(VehicleModel vehModel) 
        {
            if(vehModel == null || vehModel.licenceNum == null) 
            {
                return RedirectToAction("Index");
            }

            VehicleDTO dto = VehicleDTO.CreateDTO(vehModel);
            string response = await _beService.UpdateVehicleAsync(dto);
            if (response != "Success")
            {
                return RedirectToAction("Index");
            }

            return View("Details", vehModel.licenceNum);
        }

    }
}