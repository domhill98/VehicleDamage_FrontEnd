using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VehicleDamage_FrontEnd.Models.DTOs;

namespace VehicleDamage_FrontEnd.Services.BlobService
{
    public class BlobService : IBlobService
    {

        private string blobString = "DefaultEndpointsProtocol=https;AccountName=vehicledamageimgstorage;AccountKey=vFD8a2D5Bp46gJziVSVHSiTMrcbAutqeyQ2ei9bHKaC7ZLeViJqpu0doTkmoZbTwyweM49F0/XCIawh9yDlDnQ==";


        public async Task<string> UploadImage(ImageDTO image)
        {
            if (image == null)
            {
                return null;
            }

            string imageFullPath = null;

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("vehicledamageblobcontainer");
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                    );
            }

            //Change here to be the filename
            //string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(image.imageFile.FileName);

            
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image.Id);
            cloudBlockBlob.Properties.ContentType = image.imageFile.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(image.imageFile.OpenReadStream());

            imageFullPath = cloudBlockBlob.Uri.ToString();

            return "Success";
        }

        public async Task<IEnumerable<ImageDTO>> GetImages(string filter) 
        {
            return null;
        }





    }
}
