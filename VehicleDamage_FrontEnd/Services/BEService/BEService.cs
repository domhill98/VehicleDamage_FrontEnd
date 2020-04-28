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

        public BEService(HttpClient client) 
        {
            client.BaseAddress = new System.Uri("https://vehicledamagebe.azurewebsites.net");
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }


        public async Task<VehicleDTO> GetVehicleAsync(string lplateNum) 
        {
            string uri = "api/vehicle/" + lplateNum;

            var response = await _client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var vehicle = await response.Content.ReadAsAsync<VehicleDTO>();
            return vehicle;
        }

        public async Task<IEnumerable<VehicleDTO>> GetVehiclesAsync(VehFilterAPI apiModel) 
        {
            var uri = "api/vehicle/";
            HttpResponseMessage response;

            if (apiModel == null)
            {
                response = await _client.GetAsync(uri);
            }
            else
            {
                uri += "filtered";
                var json = JsonConvert.SerializeObject(apiModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");


                response = await _client.PostAsync(uri, stringContent);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var vehicles = await response.Content.ReadAsAsync<IEnumerable<VehicleDTO>>();

            return vehicles;
        }

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
