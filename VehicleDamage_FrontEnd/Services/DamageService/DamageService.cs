using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Services.DamageService
{
    public class DamageService : IDamageService
    {

        private readonly HttpClient _client;

        public DamageService(HttpClient client)
        {
            client.BaseAddress = new System.Uri("https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/b87c0956-6127-44ff-a4e1-154537cb1c51/classify/iterations/VehicleDamageModel/");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Prediction-Key", "859168816d564b68a63ca406d660e39e");
            _client = client;

        }

        //public async Task<APIDTO> DamageCheckUrl(APIModel model)
        //{
        //    _client.DefaultRequestHeaders.Add("Accept", "application/json");

        //    string uri = "url";

        //    var json = JsonConvert.SerializeObject(model);
        //    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        //    var response = await _client.PostAsync(uri, stringContent);
        //    if (response.StatusCode != HttpStatusCode.OK)
        //    {
        //        string error = response.Content.ToString();

        //        return null;
        //    }
        //    response.EnsureSuccessStatusCode();

        //    APIDTO responseDto = await response.Content.ReadAsAsync<APIDTO>();

        //    return responseDto;
        //}

        public async Task<APIDTO> DamageCheckImg(ImageDTO imgDTO)
        {
            _client.DefaultRequestHeaders.Add("Accept", "application/octet-stream");

            string uri = "image";

            ByteArrayContent byteContent = new ByteArrayContent(imgDTO.imageStream);

            var response = await _client.PostAsync(uri, byteContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string error = response.Content.ToString();

                return null;
            }
            response.EnsureSuccessStatusCode();

            APIDTO responseDto = await response.Content.ReadAsAsync<APIDTO>();

            return responseDto;

        }



    }
}
