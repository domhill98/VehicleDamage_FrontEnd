using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.Staff
{
    public class VehFilterAPI
    {
        public string state { get; set; }

        public Guid makeID { get; set; }

        public string lPlate { get; set; }

        public static VehFilterAPI CreateModel(string st, Guid mk, string plt)
        {
            if ((st == null || st == "All") && mk == Guid.Parse("00000000-0000-0000-0000-000000000000") && plt == null)
            {
                return null;
            }

            VehFilterAPI newModel = new VehFilterAPI()
            {
                state = st,
                makeID = mk,
                lPlate = plt
            };

            return newModel;
        }

    }
}
