using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VehicleDamage_FrontEnd.Models;
using VehicleDamage_FrontEnd.Models.Clock;
using VehicleDamage_FrontEnd.Models.DTOs;
using VehicleDamage_FrontEnd.Services.BEService;
using VehicleDamage_FrontEnd.Services.BlobService;
using VehicleDamage_FrontEnd.Services.DamageService;

namespace VehicleDamage_FrontEnd.Controllers
{
    public class ClockController : Controller
    {
        private readonly IDamageService _damageService;
        private readonly IBEService _vehicleService;
        private readonly IBlobService _blobService;

        public ClockController(IDamageService damService, IBEService veService, IBlobService blService)
        {
            _damageService = damService;
            _vehicleService = veService;
            _blobService = blService;
        }

        public IBlobService BlobService => _blobService;

        public IActionResult ClockVehicle()
        {
            ClockModel newModel = new ClockModel();

            return View(newModel);
        }

        // POST: Vehicle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClockVehicle(ClockModel model)
        {

            try
            {
                //Check liscence plate belongs to active vehicle
                VehicleDTO dto = await _vehicleService.GetVehicleAsync(model.vehicle.licenceNum);

                if (dto == null)
                {
                    ModelState.AddModelError("vehicle.licenceNum", "Vehicle not found.");
                    return View(model);
                }
                //If there are any currently unresolved damage cases on the vehicle
                else if (dto.state == "Under Investigation")
                {
                    ModelState.AddModelError("vehicle.licenceNum", "Vehicle currently unavailable due to potential damages. Please contact a member of staff.");
                    return View(model);
                }


                //Create model from the DTO
                ClockModel newModel = ClockModel.CreateModel(dto);

                return View(newModel);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmClockVehicle(ClockModel cModel)
        {
            //This would be replaced with the curent driver iD
            Guid driverID = Guid.NewGuid();


            //Update status in vehicle table
            cModel.vehicle.state = cModel.clock;
            VehicleDTO vehDTO = VehicleDTO.CreateDTO(cModel.vehicle);
            string updateResponse = await _vehicleService.UpdateVehicleAsync(vehDTO);
            if (updateResponse != "Success")
            {
                return RedirectToAction("ClockVehicle");
            }


            //Insert into the history table.           
            //Replace the new GUID with the guid of the driver when worked out.
            Guid historyID = Guid.NewGuid();
            ClockHistoryDTO newDTO = ClockHistoryDTO.CreateDTO(cModel, historyID, driverID);
            updateResponse = await _vehicleService.InsertClockHistoryAsync(newDTO);
            if (updateResponse != "Success")
            {
                return RedirectToAction("ClockVehicle");
            }


            //If clock in, check for damagesagainst the AI service
            if (cModel.clock == "In")
            {
                Guid damageID = Guid.NewGuid();

                //Create ImageDTO list so each can be added ready to be uploaded post loop.
                List<ImageDTO> vehicleImages = new List<ImageDTO>();

                //Set state of tracking = null
                string verdictTracker = "Undamaged";
                int count = 0;

                //Loop through uploads
                foreach(IFormFile img in cModel.imgs) 
                {
                    count++;
                    ImageDTO newImageDTO = ImageDTO.CreateDTO(ImageDTO.GenerateImageId(damageID, cModel.time, count.ToString(), img), img);
                    vehicleImages.Add(newImageDTO);

                    try
                    {
                        APIDTO returnApiDto = await _damageService.DamageCheckImg(newImageDTO);

                        //Validate the difference is close enough between the two tags and return the appropraite view.
                        double damagedGuess = returnApiDto.predictions.FirstOrDefault(x => x.tagName == "Damaged").probability;
                        double wholeGuess = returnApiDto.predictions.FirstOrDefault(x => x.tagName == "Whole").probability;

                        //See if both results are over 0.1 as this is too uncertain. i.e level of ucnertainty
                        if (wholeGuess >= 0.1 && damagedGuess >= 0.1)
                        {
                            if(verdictTracker == "Undamaged") 
                            {
                                verdictTracker = "Inconclusive";
                            }
                        }
                        //Damaged
                        else if (damagedGuess > wholeGuess)
                        {
                            //If Damaged then overrules the above inmconclusive of a previous image
                                verdictTracker = "Damaged";                          
                        }
                    }
                    catch 
                    {
                        verdictTracker = "Inconclusive";
                    }
                }

                //After loop of images, check the state
                if (verdictTracker != "Undamaged") 
                {
                    try 
                    { 
                        if (verdictTracker == "Inconclusive") 
                        {
                            cModel.clock = "Inconclusive";
                        }
                        //Damaged
                        else 
                        {
                            cModel.clock = "Damaged";
                        }

                        //Insert damage history
                        DamageHistoryDTO newDamageDTO = DamageHistoryDTO.CreateDTO(DamageHistoryModel.CreateModel(cModel, damageID, driverID));
                        updateResponse = await _vehicleService.InsertDamageHistoryAsync(newDamageDTO);
                        if (updateResponse != "Success")
                        {
                            return RedirectToAction("ClockVehicle");
                        }

                        //Update Vehicle with new state
                        cModel.vehicle.state = "Under Investigation";
                        VehicleDTO newVehDto = VehicleDTO.CreateDTO(cModel.vehicle);
                        updateResponse = await _vehicleService.UpdateVehicleAsync(newVehDto);
                        if (updateResponse != "Success")
                        {
                            return RedirectToAction("ClockVehicle");
                        }

                        //Loop and insert images
                        foreach(ImageDTO img in vehicleImages) 
                        {
                            //Save Images to the blob storage.
                            updateResponse = await BlobService.UploadImage(img);
                            if (updateResponse != "Success")
                            {
                                return RedirectToAction("ClockVehicle");
                            }
                        }


                        //Return back to view
                        return View("ClockConfirmed", cModel);

                    }
                    catch 
                    {
                        return RedirectToAction("ClockVehicle");
                    }                                 
                }
            }
            return View("ClockConfirmed", cModel);
        }






    }
}