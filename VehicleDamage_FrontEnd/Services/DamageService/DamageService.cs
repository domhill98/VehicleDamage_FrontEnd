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
        //https://www.customvision.ai/projects

        private readonly HttpClient _client;

        public DamageService(HttpClient client)
        {
            client.BaseAddress = new System.Uri("https://uksouth.api.cognitive.microsoft.com/customvision/v3.0/Prediction/f528cff8-6e3b-4d6c-a2ab-bba50697e840/classify/iterations/Iteration1/image");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Prediction-Key", "0ade69a8921c4e9293a1b1e41c6bc19f");
            _client = client;

        }

        public async Task<APIDTO> DamageCheckImg(ImageDTO imgDTO)
        {
            _client.DefaultRequestHeaders.Add("Accept", "application/octet-stream");

            string uri = "image";

            ByteArrayContent byteContent = new ByteArrayContent(ImageDTO.GetByteArrayFromImage(imgDTO.imageFile));

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
