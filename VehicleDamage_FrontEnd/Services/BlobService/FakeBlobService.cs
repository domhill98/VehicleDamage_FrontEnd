using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Services.BlobService
{
    public class FakeBlobService : IBlobService
    {


        private static readonly ImageDTO[] _images =
            {

                new ImageDTO { Id = "image1", imageFile = null},
                new ImageDTO { Id = "image2", imageFile = null}
        };


        public Task<string> UploadImage(ImageDTO imgDTO)
        {
            if (imgDTO == null)
            {
                return Task.FromResult("dto passed as null");
            }

            return Task.FromResult("Success");
        }


        public Task<IEnumerable<ImageDTO>> GetImages(string filter)
        {
            return Task.FromResult(_images.AsEnumerable());
        }


    }
}
