using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Models
{
    public class MakeModel
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public static MakeModel CreateModel(MakeDTO dto)
        {
            MakeModel newModel = new MakeModel()
            {
                id = dto.id,
                name = dto.name
            };
            return newModel;
        }

        public static IEnumerable<MakeModel> CreateModelNumerable(IEnumerable<MakeDTO> dtos)
        {
            List<MakeModel> tempList = new List<MakeModel>();

            foreach (MakeDTO dto in dtos)
            {
                tempList.Add(CreateModel(dto));
            }
            return tempList.AsEnumerable();
        }
    }
}
