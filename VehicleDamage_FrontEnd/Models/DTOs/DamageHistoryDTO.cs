using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class DamageHistoryDTO
    {
        public Guid Id { get; set; }

        public Guid driverID { get; set; }

        public DateTime time { get; set; }

        public string lplateNum { get; set; }

        public string state { get; set; }

        public bool resolved { get; set; }


        public static DamageHistoryDTO CreateDTO(DamageHistoryModel model)
        {
            DamageHistoryDTO newDTO = new DamageHistoryDTO()
            {
                Id = model.Id,
                driverID = model.driverID,
                time = model.time,
                lplateNum = model.lplateNum,
                state = model.state,
                resolved = model.resolved
            };
            return newDTO;
        }


    }
}
