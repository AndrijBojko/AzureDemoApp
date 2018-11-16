using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureAppExapmle.Controllers
{
    public class BlobsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            string connString = Config.connString;
            string containerName = "pictures";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            return container;
        }

       [HttpPost]
        public string UploadBlob(IFormFile formFile)
        {
            var filePath = Path.GetTempFileName();


            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(formFile.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
                blob.UploadFromStreamAsync(fileStream).GetAwaiter().GetResult();
            }
            return "success!";
        }

        [HttpPost]
        public string DownloadBlob(string name)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(name);
            using (var fileStream = System.IO.File.OpenWrite($@"c:\{name}"))
            {
                blob.DownloadToStreamAsync(fileStream).GetAwaiter().GetResult();
            }
            return "success!";
        }

        [HttpPost]
        public string DeleteBlob(string name)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(name);
            blob.DeleteAsync().GetAwaiter().GetResult();
            return "success!";
        }

        public ActionResult ListBlobs()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            List<string> blobs = new List<string>();

            BlobContinuationToken continuationToken = null;

            do
            {
                var response = container.ListBlobsSegmentedAsync(continuationToken).GetAwaiter().GetResult();
                continuationToken = response.ContinuationToken;
                var tempres= response.Results;

                foreach (var item in tempres)
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    blobs.Add(blob.Name);
                }
            }
            while (continuationToken != null);

            return View(blobs);
        }
    }
}