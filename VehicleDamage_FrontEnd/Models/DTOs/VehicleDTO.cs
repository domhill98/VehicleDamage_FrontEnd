using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class VehicleDTO
    {

        public string licenseNum { get; set; }

        public string model { get; set; }

        public virtual MakeDTO make { get; set; }

        public string colour { get; set; }

        public string state { get; set; }

        public bool active { get; set; }

        //public virtual IEnumerable<DamageHistoryDTO> damageHistory { get; set; }

        //public virtual IEnumerable<ClockHistoryDTO> clockHistory { get; set; }
        public static VehicleDTO CreateDTO(VehicleModel vehmodel)
        {
            VehicleDTO newDTO = new VehicleDTO()
            {
                licenseNum = vehmodel.licenceNum,
                model = vehmodel.model,
                make = MakeDTO.CreateDTO(vehmodel.make),
                colour = vehmodel.colour,
                state = vehmodel.state,
                active = vehmodel.active
            };
            return newDTO;
        }





    }
}
