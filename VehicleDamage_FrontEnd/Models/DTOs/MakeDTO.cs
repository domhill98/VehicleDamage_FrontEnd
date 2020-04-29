using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class MakeDTO
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public static MakeDTO CreateDTO(MakeModel model)
        {
            MakeDTO newDTO = new MakeDTO()
            {
                id = model.id,
                name = model.name
            };
            return newDTO;
        }


    }
}
