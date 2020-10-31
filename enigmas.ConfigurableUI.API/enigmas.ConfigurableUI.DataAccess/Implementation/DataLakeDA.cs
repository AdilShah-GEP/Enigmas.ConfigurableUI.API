using enigmas.ConfigurableUI.Core.Entity;
using enigmas.ConfigurableUI.DataAccess.Interface;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace enigmas.ConfigurableUI.DataAccess.Implementation
{
    public class DataLakeDA : IDataLakeDA
    {
        public async Task<MemoryStream> GetData(InputParamsDataLake inputParamsDL,string defaultFile)
        {
            var container = GetContainer(inputParamsDL.Connection, inputParamsDL.ContainerName);
            var blockBlob = container.GetBlockBlobReference(inputParamsDL.FileName);
            var ms = new MemoryStream();
            if (blockBlob.Exists())
                blockBlob.DownloadToStream(ms);
            else
            {
                blockBlob = container.GetBlockBlobReference(defaultFile);
                blockBlob.DownloadToStream(ms);
            }
            return ms;
        }

        public async Task<bool> InsertData(InputParamsDataLake inputParamsDL, InputModel[] input)
        {
            var cloudBlobContainer = GetContainer(inputParamsDL.Connection,inputParamsDL.ContainerName);
            cloudBlobContainer.CreateIfNotExists();
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(inputParamsDL.FileName);
            cloudBlockBlob.Properties.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
            cloudBlockBlob.UploadText(JsonConvert.SerializeObject(input), null, null, null, null);
            return true;
        }

        public CloudBlobContainer GetContainer(string conn, string containerName)
        {
            var storageacc = CloudStorageAccount.Parse(conn);
            var blobClient = storageacc.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }
    }

}
