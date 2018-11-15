using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            string connString = "";// use Access Key on azure portal to get conn string
            string containerName = "pictures";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            return container;
        }

        public string UploadBlob()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference("sample");
            using (var fileStream = System.IO.File.OpenRead(@"d:\sample.txt"))
            {
                blob.UploadFromStreamAsync(fileStream).GetAwaiter().GetResult();
            }
            return "success!";
        }

        public string DownloadBlob()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference("sample");
            using (var fileStream = System.IO.File.OpenWrite(@"d:\downloadedBlob.txt"))
            {
                blob.DownloadToStreamAsync(fileStream).GetAwaiter().GetResult();
            }
            return "success!";
        }

        public string DeleteBlob()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference("sample");
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