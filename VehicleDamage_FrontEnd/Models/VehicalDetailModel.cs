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
        public virtual IEnumerable<DamageHistoryModel> damageHistory { get; set; }

        public new static VehicleDetailModel CreateModel(VehicleDTO dto, IEnumerable<DamageHistoryDTO> dmgHistory, IEnumerable<ClockHistoryDTO> clkHistory)
        {
            VehicleDetailModel newModel = new VehicleDetailModel()
            {
                licenceNum = dto.licenseNum,
                model = dto.model,
                make = MakeModel.CreateModel(dto.make),
                state = dto.state,
                colour = dto.colour,
                active = dto.active,
                damageHistory = DamageHistoryModel.CreateModelNumerable(dmgHistory).OrderByDescending(x => x.time),
                clockHistory = ClockHistoryModel.CreateModelNumerable(clkHistory).OrderByDescending(x => x.time)
            };

            return newModel;
        }

    }
}
