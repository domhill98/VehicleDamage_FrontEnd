using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class APIDTO
    {
        public Guid id { get; set; }
        public Guid project { get; set; }
        public Guid iteration { get; set; }
        public DateTime created { get; set; }
        public List<PredictionDTO> predictions { get; set; }
    }
}
