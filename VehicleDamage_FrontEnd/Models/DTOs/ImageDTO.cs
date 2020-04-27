using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleDamage_FrontEnd.Models.DTOs
{
    public class ImageDTO
    {
        public string Id { get; set; }

        public byte[] imageStream { get; set; }


        public static ImageDTO CreateDTO(string imgID, byte[] stream)
        {
            ImageDTO newdto = new ImageDTO()
            {
                Id = imgID,
                imageStream = stream
            };
            return newdto;
        }

        public static string GenerateImageId(Guid dmgID, string dTime, string seqText)
        {

            return GenerateImagePre(dmgID, dTime) + "_" + seqText; ;
        }

        public static string GenerateImagePre(Guid dmgId, string dTime)
        {
            //Convert datetime from / amd such to desired format for file
            string newTime = DateTime.Parse(dTime).ToString("ddMMMyyyy_HHmmss");

            return dmgId + "_" + dTime;
        }





    }
}
