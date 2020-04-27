using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public VehicleController(IBEService beService, IBlobService blobService)
        {
            _beService = beService;
            _blobService = blobService;
        }


        // GET: Staff
        //Takes indexModel which contains the filtering options and applies it to the list of vehicles to be shown to the user.
        public async Task<ActionResult> Index([FromQuery] VehIndexModel indexModel = null)
        {
            //Set the DD lists as null to avoid errors 
            IEnumerable<VehicleModel> vehicles = null;
            IEnumerable<VehicleModel> vehicleConcern = null;
            IEnumerable<MakeModel> makeList = null;
            IEnumerable<string> stateList = null;

            try
            {
                //Call API to get filtered vehicles
                VehFilterAPI apiModel = VehFilterAPI.CreateModel(indexModel.selectedState, indexModel.selectedMake, indexModel.lPlateSearch);
                IEnumerable<VehicleDTO> vehicleDTOs = await _beService.GetVehiclesAsync(apiModel);
                vehicles = VehicleModel.CreateModelNumerable(vehicleDTOs);

                //Create seperate list for vehicles with damage concerns
                vehicleConcern = vehicles.Where(x => x.damageHistory.Any(r => r.resolved == false)).ToList();
                //Drop those in the concern list
                vehicles = vehicles.Except(vehicleConcern);

                //Call API to get Make lists
                makeList = MakeModel.CreateModelNumerable(await _beService.GetMakesAsync());

                indexModel = VehIndexModel.CreateModel(vehicles, vehicleConcern, makeList, indexModel.selectedMake, indexModel.selectedState, indexModel.lPlateSearch);
            }
            catch
            {
                vehicles = Array.Empty<VehicleModel>();
                makeList = Array.Empty<MakeModel>();
                stateList = Array.Empty<string>();

                indexModel = VehIndexModel.CreateModel(null, null, null, Guid.Empty, null, null);
            }

            return View("VehicleList", indexModel);
        }



        public async Task<IActionResult> Details(string lPlate)
        {

            VehicleDTO dto = await _beService.GetVehicleAsync(lPlate);
            VehicleDetailModel model = VehicleDetailModel.CreateModel(dto);

            return View("VehicleDetails", model);
        }



        public async Task<IActionResult> ViewDamageImages(DamageHistoryModel damageModel)
        {
            //Work out Image ID Pretext to use to search
            string imagePreText = ImageDTO.GenerateImagePre(damageModel.Id, damageModel.time.ToString());

            //Search Image DB for images with that name
            IEnumerable<ImageDTO> foundImages = await _blobService.GetImages(imagePreText);

            //Set damageModel list of images to that
            damageModel.images = foundImages;

            if (damageModel.resolved == false)
            {
                return View("VehicleDamageResolve", damageModel);
            }
            else
            {
                return View("VehicleDamageView", damageModel);
            }
        }


        public async Task<IActionResult> ResolveDamages(DamageHistoryModel damageModel)
        {
            //Flip resolved to true
            damageModel.resolved = true;

            //Update the db
            string response = await _beService.UpdateDamageHistoryAsync(DamageHistoryDTO.CreateDTO(damageModel));

            if(response != "Success") 
            {
                return RedirectToAction("ViewDamageImages", damageModel);
            }


            return RedirectToAction("Details", damageModel.lplateNum);
        }
    }
}