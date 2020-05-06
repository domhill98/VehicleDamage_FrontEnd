using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Services.BlobService
{
    //Interface which extends to the real or fake service
    public interface IBlobService
    {
        Task<string> UploadImage(ImageDTO imgDTO);

        Task<IEnumerable<byte[]>> GetImages(string filter);

    }
}
