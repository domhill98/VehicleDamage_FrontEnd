using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;
using VehicleDamage_FrontEnd.Models.Staff;

namespace VehicleDamage_FrontEnd.Services.BEService
{
    public class BEService : IBEService
    {
        private readonly HttpClient _client;

        //Set the URI for the BackEnd APU
        public BEService(HttpClient client) 
        {
            client.BaseAddress = new System.Uri("https://vehicledamagebe.azurewebsites.net");
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }

        /// <summary>
        /// GET Call to the API service
        /// Get vehicle with passed lplate
        /// </summary>
        /// <param name="lplateNum">filter</param>
        /// <returns>Vehicle with lplate</returns>
        public async Task<VehicleDTO> GetVehicleAsync(string lplateNum) 
        {
            //Set uri for API endpoint
            string uri = "api/vehicle/" + lplateNum;
            //Send request
            var response = await _client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            //Read response
            var vehicle = await response.Content.ReadAsAsync<VehicleDTO>();
            return vehicle;
        }

        /// <summary>
        /// POST Call to the API service
        /// Get Vehicles that accord to the 3 filter values in the model
        /// </summary>
        /// <param name="apiModel">Contains the filters</param>
        /// <returns>Vehicles</returns>
        public async Task<IEnumerable<VehicleDTO>> GetVehiclesAsync(VehFilterAPI apiModel) 
        {
            //Set uri for endpoint
            var uri = "api/vehicle/";
            HttpResponseMessage response;

            if (apiModel == null)
            {
                //If no model, jsut get all vehicles
                response = await _client.GetAsync(uri);
            }
            else
            {
                //Set endpoint to filtered value
                uri += "filtered";
                //Serialise the object to JSON for posting
                var json = JsonConvert.SerializeObject(apiModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                //Post model as JSON
                response = await _client.PostAsync(uri, stringContent);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            //Read the response
            var vehicles = await response.Content.ReadAsAsync<IEnumerable<VehicleDTO>>();

            return vehicles;
        }

        /// <summary>
        /// GET Call to the API service
        /// Get the clock histories for a vehicle
        /// </summary>
        /// <param name="lplateNum">key for the vehicle to search for</param>
        /// <returns>List of Clock Histories for given vehicle</returns>
        public async Task<IEnumerable<ClockHistoryDTO>> GetClockHistoriesAsync(string lplateNum) 
        {
            string uri = "api/ClockHistory/" + lplateNum;

            var response = await _client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var histories = await response.Content.ReadAsAsync< IEnumerable<ClockHistoryDTO>>();
            return histories;
        }

        /// <summary>
        /// GET Call to the API service
        /// Get the damage histories for a vehicle
        /// </summary>
        /// <param name="lplateNum">key for the vehicle to search for</param>
        /// <returns>List of Damage Histories for given vehicle</returns>
        public async Task<IEnumerable<DamageHistoryDTO>> GetDamageAsync(string lplateNum) 
        {
            string uri = "api/DamageHistory/" + lplateNum;

            var response = await _client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var histories = await response.Content.ReadAsAsync<IEnumerable<DamageHistoryDTO>>();
            return histories;
        }


        /// <summary>
        /// GET Call to the API service
        /// Gets the list of available vehicle makes
        /// </summary>
        /// <returns>List of makes</returns>
        public async Task<IEnumerable<MakeDTO>> GetMakesAsync() 
        {
            string uri = "api/make";

            var response = await _client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var makes = await response.Content.ReadAsAsync<IEnumerable<MakeDTO>>();
            return makes;
        }

        /// <summary>
        /// POST Call to the API service
        /// Update a vehicle in the db
        /// </summary>
        /// <param name="vehDTO">Vehicle model to replace existing entry</param>
        /// <returns></returns>
        public async Task<string> UpdateVehicleAsync(VehicleDTO vehDTO) 
        {
            if (vehDTO == null)
            {
                return "dto passed as null";
            }

            var uri = "api/vehicle/update/";

            var json = JsonConvert.SerializeObject(vehDTO);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, stringContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Error with code:" + response.StatusCode;
            }
            response.EnsureSuccessStatusCode();

            return "Success";
        }

        /// <summary>
        /// POST Call to the API service
        /// Update Damage History in db
        /// </summary>
        /// <param name="dhDTO">Damage History model to replace existing entry</param>
        /// <returns></returns>
        public async Task<string> UpdateDamageHistoryAsync(DamageHistoryDTO dhDTO) 
        {
            if (dhDTO == null)
            {
                return "dto passed as null";
            }

            var uri = "api/DamageHistory/Update/";

            var json = JsonConvert.SerializeObject(dhDTO);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, stringContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Error with code:" + response.StatusCode;
            }
            response.EnsureSuccessStatusCode();

            return "Success";
        }

        /// <summary>
        /// POST Call to the API service
        ///  Update Clock History in db
        /// </summary>
        /// <param name="vehDTO"></param>
        /// <returns></returns>
        public async Task<string> InsertVehicleAsync(VehicleDTO vehDTO) 
        {
            if (vehDTO == null)
            {
                return "dto passed as null";
            }

            var uri = "api/Vehicle/Insert/";

            var json = JsonConvert.SerializeObject(vehDTO);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, stringContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Error with code:" + response.StatusCode;
            }
            response.EnsureSuccessStatusCode();

            return "Success";
        }


        /// <summary>
        /// POST Call to the API service
        /// Update Clock History in db
        /// </summary>
        /// <param name="hisDTO">History model to insert entry</param>
        /// <returns></returns>
        public async Task<string> InsertClockHistoryAsync(ClockHistoryDTO hisDTO) 
        {
            if (hisDTO == null)
            {
                return "dto passed as null";
            }

            var uri = "api/ClockHistory/Insert/";

            var json = JsonConvert.SerializeObject(hisDTO);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, stringContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Error with code:" + response.StatusCode;
            }
            response.EnsureSuccessStatusCode();

            return "Success";
        }

        /// <summary>
        /// POST Call to the API service
        /// Insert Damage History to DB
        /// </summary>
        /// <param name="hisDTO">History modell to insert</param>
        /// <returns></returns>
        public async Task<string> InsertDamageHistoryAsync(DamageHistoryDTO hisDTO) 
        {
            if (hisDTO == null)
            {
                return "dto passed as null";
            }

            var uri = "api/DamageHistory/Insert/";

            var json = JsonConvert.SerializeObject(hisDTO);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, stringContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Error with code:" + response.StatusCode;
            }
            response.EnsureSuccessStatusCode();

            return "Success";
        }



    }
}
