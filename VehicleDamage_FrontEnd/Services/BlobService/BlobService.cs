using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NUnit.Framework;
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
        
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image.Id);
            cloudBlockBlob.Properties.ContentType = image.imageFile.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(image.imageFile.OpenReadStream());

            imageFullPath = cloudBlockBlob.Uri.ToString();

            return "Success";
        }

        public async Task<IEnumerable<byte[]>> GetImages(string filter) 
        {
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

            //If not, get dir of all images and filter through manually.
            BlobContinuationToken continuationToken = null;
            var results = new List<IListBlobItem>();
            do
            {
                var response = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix: filter, continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            //Now get each and return

            List<byte[]> images = new List<byte[]>();

            foreach(var blobItem in results) 
            {
                string imgName = blobItem.Uri.Segments.Last();

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imgName);

                using(var ms = new MemoryStream()) 
                {
                    await cloudBlockBlob.DownloadToStreamAsync(ms);
                    images.Add(ms.ToArray());
                }
            }


            return images.AsEnumerable();
        }





    }
}
