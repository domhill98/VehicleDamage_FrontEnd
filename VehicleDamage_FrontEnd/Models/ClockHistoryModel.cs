using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Models
{
    public class ClockHistoryModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Driver")]
        public Guid driverID { get; set; }

        [Display(Name = "Date")]
        public DateTime time { get; set; }

        [Display(Name = "LPlate")]
        public string lplateNum { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        public static ClockHistoryModel CreateModel(ClockHistoryDTO dto)
        {
            ClockHistoryModel newModel = new ClockHistoryModel()
            {
                Id = dto.Id,
                driverID = dto.driverID,
                time = dto.time,
                lplateNum = dto.lplateNum,
                state = dto.state
            };
            return newModel;
        }

        public static IEnumerable<ClockHistoryModel> CreateModelNumerable(IEnumerable<ClockHistoryDTO> dtos)
        {
            List<ClockHistoryModel> tempList = new List<ClockHistoryModel>();

            if (dtos != null)
            {
                foreach (ClockHistoryDTO dto in dtos)
                {
                    tempList.Add(CreateModel(dto));
                };
            }

            return tempList.AsEnumerable();
        }


    }
}
