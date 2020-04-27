using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.Clock;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class ClockHistoryDTO
    {
        public Guid Id { get; set; }

        public Guid driverID { get; set; }

        public DateTime time { get; set; }

        public string lplateNum { get; set; }

        public string state { get; set; }

        public static ClockHistoryDTO CreateDTO(ClockModel model, Guid thisID, Guid driver)
        {
            ClockHistoryDTO newDTO = new ClockHistoryDTO()
            {
                Id = thisID,
                driverID = driver,
                time = DateTime.Parse(model.time),
                lplateNum = model.vehicle.licenceNum,
                state = model.clock
            };
            return newDTO;
        }




    }
}
