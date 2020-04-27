using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Services.BlobService
{
    public interface IBlobService
    {

        Task<string> UploadImage(ImageDTO imgDTO);

        Task<IEnumerable<ImageDTO>> GetImages(string filter);

    }
}
