using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                    ModelState.AddModelError("licenceNum", "Vehicle not found.");
                    return View(model);
                }
                //If there are any currently unresolved damage cases on the vehicle
                else if (dto.damageHistory != null && dto.damageHistory.Where(x => x.resolved == false).ToList().Count() != 0)
                {
                    ModelState.AddModelError("licenceNum", "Vehicle currently unavailable due to potential damages. Please contact a member of staff.");
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

                //Create ImageDTO for each image
                List<ImageDTO> uploadedImages = new List<ImageDTO>();
                int count = 0;
                foreach (IFormFile img in cModel.imgs)
                {
                    count++;
                    ImageDTO newImageDTO = ImageDTO.CreateDTO(ImageDTO.GenerateImageId(damageID, cModel.time, count.ToString()), GetByteArrayFromImage(img));
                    uploadedImages.Add(newImageDTO);


                    try
                    {

                        //Upload Images to AI API
                        APIDTO returnApiDto = await _damageService.DamageCheckImg(newImageDTO);

                        //Validate the difference is close enough between the two tags and return the appropraite view.
                        double damagedGuess = returnApiDto.predictions.FirstOrDefault(x => x.tagName == "Damaged").probability;
                        double wholeGuess = returnApiDto.predictions.FirstOrDefault(x => x.tagName == "Whole").probability;


                        //See if both results are over 0.1 as this is too uncertain. i.e level of ucnertainty
                        if (wholeGuess >= 0.1 && damagedGuess >= 0.1)
                        {
                            //Set the clock/state so the correct response can be displayed in the view. 
                            cModel.clock = "Inconclusive";

                            //As Uncertain, add to the damage db                                             
                            DamageHistoryDTO newDamageDTO = DamageHistoryDTO.CreateDTO(DamageHistoryModel.CreateModel(cModel, damageID, driverID));
                            updateResponse = await _vehicleService.InsertDamageHistoryAsync(newDamageDTO);
                            if (updateResponse != "Success")
                            {
                                return RedirectToAction("ClockVehicle");
                            }

                            //Save Images to the blob storage.
                            updateResponse = await BlobService.UploadImage(newImageDTO);
                            if (updateResponse != "Success")
                            {
                                return RedirectToAction("ClockVehicle");
                            }

                            //Return back to view as there is no point testing the rest of the images
                            return View("ClockConfirmed", cModel);
                        }
                        //Damaged
                        else if (damagedGuess > wholeGuess)
                        {
                            //Set the clock/state so the correct response can be displayed in the view. 
                            cModel.clock = "Damaged";

                            //As Damaged, add to the damage db                                             
                            DamageHistoryDTO newDamageDTO = DamageHistoryDTO.CreateDTO(DamageHistoryModel.CreateModel(cModel, damageID, driverID));
                            updateResponse = await _vehicleService.InsertDamageHistoryAsync(newDamageDTO);
                            if (updateResponse != "Success")
                            {
                                return RedirectToAction("ClockVehicle");
                            }


                            //Save Images to the blob storage.
                            updateResponse = await BlobService.UploadImage(newImageDTO);
                            if (updateResponse != "Success")
                            {
                                return RedirectToAction("ClockVehicle");
                            }

                            //Return back to view as there is no point testing the rest of the images
                            return View("ClockConfirmed", cModel);
                        }
                        //If Undamaged then we dont need to save the images or the damaged db

                        //Loop to next image
                    }
                    catch
                    {
                        //If fails to reach AI API, simply show inclocusive result and staff member can decide
                        //Set the clock/state so the correct response can be displayed in the view. 
                        cModel.clock = "Inconclusive";

                        //As Uncertain, add to the damage db                                             
                        DamageHistoryDTO newDamageDTO = DamageHistoryDTO.CreateDTO(DamageHistoryModel.CreateModel(cModel, damageID, driverID));
                        updateResponse = await _vehicleService.InsertDamageHistoryAsync(newDamageDTO);
                        if (updateResponse != "Success")
                        {
                            return RedirectToAction("ClockVehicle");
                        }

                        //Save Images to the blob storage.
                        updateResponse = await BlobService.UploadImage(newImageDTO);
                        if (updateResponse != "Success")
                        {
                            return RedirectToAction("ClockVehicle");
                        }
                    }
                }
            }
            return View("ClockConfirmed", cModel);
        }








        //This snippet taken from https://stackoverflow.com/questions/35379309/how-to-upload-files-in-asp-net-core
        private byte[] GetByteArrayFromImage(IFormFile img)
        {
            using (var target = new MemoryStream())
            {
                img.CopyTo(target);
                return target.ToArray();
            }
        }
    }
}