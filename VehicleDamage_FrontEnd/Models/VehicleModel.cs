using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Models
{
    public class VehicleModel
    {
        [Display(Name = "License Number")]
        [MaxLength(12)]
        [MinLength(3)]
        public string licenceNum { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Model")]
        public string model { get; set; }

        [Display(Name = "Make")]
        public MakeModel make { get; set; }

        [Display(Name = "Colour")]
        public string colour { get; set; }

        public bool active { get; set; }

        public virtual IEnumerable<DamageHistoryDTO> damageHistory { get; set; }

        public static VehicleModel CreateModel(VehicleDTO dto)
        {
            VehicleModel newModel = new VehicleModel()
            {
                licenceNum = dto.licenseNum,
                model = dto.model,
                make = MakeModel.CreateModel(dto.make),
                state = dto.state,
                colour = dto.colour,
                active = dto.active,
                damageHistory = dto.damageHistory

            };

            return newModel;
        }

        public static IEnumerable<VehicleModel> CreateModelNumerable(IEnumerable<VehicleDTO> dtos)
        {
            List<VehicleModel> tempList = new List<VehicleModel>();

            foreach (VehicleDTO dto in dtos)
            {
                tempList.Add(CreateModel(dto));
            }

            return tempList.AsEnumerable();
        }




    }
}
