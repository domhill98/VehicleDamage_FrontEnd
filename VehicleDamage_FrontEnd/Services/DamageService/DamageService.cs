using Microsoft.Extensions.Configuration;
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

        /// <summary>
        /// Default constructor for the Damage Service.
        /// Sets the base address and adds the neccessary headers
        /// </summary>
        /// <param name="client"></param>
        public DamageService(HttpClient client, IConfiguration configuration)
        {
            client.BaseAddress = new System.Uri(configuration.GetValue<string>("AIService:aiConnectionURL"));
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Prediction-Key", configuration.GetValue<string>("AIService:aiConnectionKey"));
            _client = client;

        }

        /// <summary>
        /// Calls the AI API to check an image from damage
        /// </summary>
        /// <param name="imgDTO">Image model to be checked</param>
        /// <returns>The resulting API return as a model</returns>
        public async Task<APIDTO> DamageCheckImg(ImageDTO imgDTO)
        {
            _client.DefaultRequestHeaders.Add("Accept", "application/octet-stream");

            string uri = "image";
            //Extract the byte array content from the image
            ByteArrayContent byteContent = new ByteArrayContent(ImageDTO.GetByteArrayFromImage(imgDTO.imageFile));
            //Post the raw byte array to the API
            var response = await _client.PostAsync(uri, byteContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string error = response.Content.ToString();

                return null;
            }
            response.EnsureSuccessStatusCode();
            //Read the response into an API Model
            APIDTO responseDto = await response.Content.ReadAsAsync<APIDTO>();

            return responseDto;
        }



    }
}
