using dm.lib.core.nuget;
using enigmas.ConfigurableUI.Core.Entity;
using enigmas.ConfigurableUI.service.Interface;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace enigmas.ConfigurableUI.service.Implementation
{
    public class DataLake : IDataLake
    {
        private readonly IGepService _gepService;
        public DataLake(IGepService gepService)
        {
            _gepService = gepService;
        }

        public async Task<bool> InsertData(string conn, string container, InputModel input)
        {
            try
            {
                var cloudBlobContainer = GetContainer(conn,container);
                cloudBlobContainer.CreateIfNotExists();
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(GetFileName());
                cloudBlockBlob.Properties.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                cloudBlockBlob.UploadText(JsonConvert.SerializeObject(input), null, null, null, null);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on UploadFile : " + e.ToString());
            }
            return true;
        }

        public async Task<InputModel> GetData(string conn, string containerName)
        {
            var container = GetContainer(conn,containerName);
            var blockBlob = container.GetBlockBlobReference("JsonFiles/Chevron/Pascaguola/es-ES/Configured.json");//GetFileName());
            var ms = new MemoryStream();
            if (blockBlob.Exists())
                blockBlob.DownloadToStream(ms);

            ms.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(ms, Encoding.UTF8);
            var str = reader.ReadToEnd();
            var output = JsonConvert.DeserializeObject<InputModel>(str);

            return output;
        }

        public CloudBlobContainer GetContainer(string conn, string containerName)
        {
            var storageacc = CloudStorageAccount.Parse(conn);
            var blobClient = storageacc.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        public string GetFileName()
        {
            StringBuilder sb = new StringBuilder("JsonFiles");
            sb.Append("/"+_gepService.GetUserContext().BuyerPartnerCode);
            sb.Append("/" + _gepService.RegionId);
            sb.Append("/"+_gepService.GetUserContext().Culture);
            sb.Append("/" + "Configured.json");
            return sb.ToString();
        }
    }
}
