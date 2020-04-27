using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class PredictionDTO
    {
        public double probability { get; set; }
        public string tagId { get; set; }
        public string tagName { get; set; }
    }
}
