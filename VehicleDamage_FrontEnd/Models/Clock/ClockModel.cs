using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Models.Clock
{
    public class ClockModel
    {
        [Display(Name = "Clock")]
        public string clock { get; set; }

        [Display(Name = "Time")]
        public string time { get; set; }

        //https://stackoverflow.com/questions/35379309/how-to-upload-files-in-asp-net-core
        [Display(Name = "Vehicle Images")]
        public List<IFormFile> imgs { get; set; }

        public VehicleModel vehicle { get; set; }

        public static ClockModel CreateModel(VehicleDTO inVehicle)
        {
            ClockModel newModel = new ClockModel()
            {
                time = DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss"),
                clock = inVehicle.state == "In" ? "Out" : "In",
                vehicle = VehicleModel.CreateModel(inVehicle)
            };
            return newModel;
        }
    }
}
