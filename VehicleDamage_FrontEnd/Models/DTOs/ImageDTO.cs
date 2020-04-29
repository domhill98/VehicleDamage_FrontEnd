using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class ImageDTO
    {
        public string Id { get; set; }

        //public byte[] imageStream { get; set; }

        public IFormFile imageFile { get; set; }


        public static ImageDTO CreateDTO(string imgID, IFormFile imgFile)
        {
            ImageDTO newdto = new ImageDTO()
            {
                Id = imgID,
                imageFile = imgFile
            };
            return newdto;
        }

        public static string GenerateImageId(Guid dmgID, string dTime, string seqText, IFormFile imgFile)
        {

            return GenerateImagePre(dmgID, dTime) + "_" + seqText + Path.GetExtension(imgFile.FileName); ;
        }

        public static string GenerateImagePre(Guid dmgId, string dTime)
        {
            //Convert datetime from / amd such to desired format for file
            string newTime = DateTime.Parse(dTime).ToString("ddMMMyyyy_HHmmss");

            return dmgId.ToString() + "_" + newTime;
        }

        //This snippet taken from https://stackoverflow.com/questions/35379309/how-to-upload-files-in-asp-net-core
        //AI Model only takes byte[]
        public static byte[] GetByteArrayFromImage(IFormFile img)
        {
            using (var target = new MemoryStream())
            {
                img.CopyTo(target);
                return target.ToArray();
            }
        }



    }
}
