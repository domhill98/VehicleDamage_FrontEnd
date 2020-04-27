﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.Clock;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Models
{
    public class DamageHistoryModel
    {
        public Guid Id { get; set; }

        public Guid driverID { get; set; }

        public DateTime time { get; set; }

        public string lplateNum { get; set; }

        public string state { get; set; }

        public bool resolved { get; set; }

        public virtual IEnumerable<ImageDTO> images { get; set; }




        public static DamageHistoryModel CreateModel(ClockModel model, Guid thisID, Guid driver)
        {
            DamageHistoryModel newModel = new DamageHistoryModel()
            {
                Id = thisID,
                driverID = driver,
                time = DateTime.Parse(model.time),
                lplateNum = model.vehicle.licenceNum,
                state = model.clock,
                resolved = false
            };
            return newModel;
        }

        public static DamageHistoryModel CreateModel(DamageHistoryDTO dto)
        {
            DamageHistoryModel newModel = new DamageHistoryModel()
            {
                Id = dto.Id,
                driverID = dto.driverID,
                time = dto.time,
                lplateNum = dto.lplateNum,
                state = dto.state,
                resolved = dto.resolved
            };
            return newModel;
        }

        public static IEnumerable<DamageHistoryModel> CreateModelNumerable(IEnumerable<DamageHistoryDTO> dtos)
        {
            List<DamageHistoryModel> tempList = new List<DamageHistoryModel>();

            if (dtos != null)
            {
                foreach (DamageHistoryDTO dto in dtos)
                {
                    tempList.Add(CreateModel(dto));
                };
            }

            return tempList.AsEnumerable();
        }



    }
}
