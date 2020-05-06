using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.Staff
{
    public class VehIndexModel
    {
        public List<VehicleModel> vehicleList { get; set; }

        public List<VehicleModel> vehicleWithConcernList { get; set; }

        [Display(Name = "Make")]
        public SelectList makeList { get; set; }

        [Display(Name = "Make")]
        public Guid selectedMake { get; set; }

        [Display(Name = "State")]
        public SelectList stateList { get; set; }

        [Display(Name = "State")]
        public string selectedState { get; set; }

        [Display(Name = "LPlate")]
        public string lPlateSearch { get; set; }

        //Create model given the current paramaters
        public static VehIndexModel CreateModel(IEnumerable<VehicleModel> vehicles, IEnumerable<VehicleModel> concernVehicles,
            IEnumerable<MakeModel> makes,
            Guid selMake,
            string selState,
            string lPlatetxt)
        {
            VehIndexModel newModel = new VehIndexModel();

            //Add the "All" option to the select lists
            MakeModel allMake = new MakeModel() { id = Guid.Empty, name = "All" };
            List<MakeModel> temp = makes.ToList();
            temp.Insert(0, allMake);
            makes = temp.AsEnumerable();

            //Vehicle list passed through this gauntlet of filters
            if (vehicles != null)
            {
                newModel.vehicleList = vehicles.ToList();
            }
            if (concernVehicles != null)
            {
                newModel.vehicleWithConcernList = concernVehicles.ToList();
            }
            if (selMake != null)
            {
                newModel.selectedMake = selMake;
            }
            if (makes != null)
            {
                newModel.makeList = new SelectList(makes, "id", "name", selMake);
            }
            if (selState != null && selState != "All")
            {
                newModel.selectedState = selState;
            }
            else
            {
                newModel.selectedState = "All";
            }
            if (lPlatetxt != null)
            {
                newModel.lPlateSearch = lPlatetxt;
            }

            newModel.stateList = new SelectList(CreateStateList());
            newModel.stateList.First(x => x.Text == newModel.selectedState).Selected = true;
            return newModel;
        }

        //Fixed Enum States. Returns the lsit of possible states.
        private static IEnumerable<string> CreateStateList()
        {
            List<string> tempList = new List<string>();
            tempList.Add("All");
            tempList.Add("In");
            tempList.Add("Out");
            tempList.Add("Under Investigation");
            return tempList.AsEnumerable();
        }

    }
}
