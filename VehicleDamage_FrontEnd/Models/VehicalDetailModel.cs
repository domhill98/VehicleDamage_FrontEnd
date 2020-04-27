using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Models
{
    public class VehicleDetailModel : VehicleModel
    {
        public virtual IEnumerable<ClockHistoryModel> clockHistory { get; set; }

        public new static VehicleDetailModel CreateModel(VehicleDTO dto)
        {
            VehicleDetailModel newModel = new VehicleDetailModel()
            {
                licenceNum = dto.licenseNum,
                model = dto.model,
                make = MakeModel.CreateModel(dto.make),
                state = dto.state,
                colour = dto.colour,
                active = dto.active,
                damageHistory = dto.damageHistory,
                clockHistory = ClockHistoryModel.CreateModelNumerable(dto.clockHistory)
            };

            return newModel;
        }

    }
}
