using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private string blobConn = "";

        public BlobService(IConfiguration configuration) 
        {
            _configuration = configuration;
            blobConn = _configuration.GetValue<string>("BlobService:blobConnection");
        }




        /// <summary>
        /// Upload an Image to the blob Storage
        /// </summary>
        /// <param name="image">Image file to upload</param>
        /// <returns></returns>
        public async Task<string> UploadImage(ImageDTO image)
        {
            if (image == null)
            {
                return null;
            }

            //Connect to the azure account
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobConn);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            //Go to the Vehicle Damage Container/File
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("vehicledamageblobcontainer");
           //if it doesn't exist, create it
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                    );
            }
            //Create a refernce with the given image ID/name
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image.Id);
            cloudBlockBlob.Properties.ContentType = image.imageFile.ContentType;
            //Upload the image to the reference
            await cloudBlockBlob.UploadFromStreamAsync(image.imageFile.OpenReadStream());


            return "Success";
        }

        /// <summary>
        /// Get images from the blob storage
        /// </summary>
        /// <param name="filter">The search text for the image name(s)</param>
        /// <returns></returns>
        public async Task<IEnumerable<byte[]>> GetImages(string filter) 
        {
            //Connect tohe azure storage account
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobConn);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            //Set container to the VEhicle Damage Container
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("vehicledamageblobcontainer");
            //Create it if it doesnt exist
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                    );
            }

            //Set the continuation token which is set when the end of the stream is reached
            BlobContinuationToken continuationToken = null;
            var results = new List<IListBlobItem>();        
            do
            {
                //Get all images from dir that have the filter
                var response = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix: filter, continuationToken);
                continuationToken = response.ContinuationToken;
                //Add the images to a list
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            List<byte[]> images = new List<byte[]>();

            //Loop through each image and save as a byte[]
            foreach(var blobItem in results) 
            {
                //Get image name from the path
                string imgName = blobItem.Uri.Segments.Last();

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imgName);
                //Download the image using the filename
                using(var ms = new MemoryStream()) 
                {
                    await cloudBlockBlob.DownloadToStreamAsync(ms);
                    images.Add(ms.ToArray());
                }
            }

            //Return image array
            return images.AsEnumerable();
        }

    }
}
