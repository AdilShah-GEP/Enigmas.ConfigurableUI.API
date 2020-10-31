using dm.lib.core.nuget;
using enigmas.ConfigurableUI.Core.Entity;
using enigmas.ConfigurableUI.DataAccess.Interface;
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
    public class DataLakeService : IDataLakeService
    {
        private readonly IGepService _gepService;
        private readonly IDataLakeDA _dataLakeDA;
        public DataLakeService(IGepService gepService, IDataLakeDA dataLakeDA)
        {
            _gepService = gepService;
            _dataLakeDA = dataLakeDA;
        }

        public async Task<bool> InsertData(string conn, string container, InputModel[] input)
        {
            try
            {
                InputParamsDataLake inputParams = new InputParamsDataLake()
                {
                    Connection = conn,
                    ContainerName = container,
                    FileName = GetFileName()
                };
                await _dataLakeDA.InsertData(inputParams, input);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on UploadFile : " + e.ToString());
            }
            return true;
        }

        public async Task<InputModel[]> GetData(string conn, string containerName)
        {
            InputParamsDataLake inputParams = new InputParamsDataLake()
            {
                Connection = conn,
                ContainerName = containerName,
                FileName = GetFileName()
            };
            var ms = await _dataLakeDA.GetData(inputParams, GetDefaultFile());
            return await ConvertMStreamtoObject(ms);
        }

        public async Task<InputModel[]> ConvertMStreamtoObject(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(ms, Encoding.UTF8);
            var str = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<InputModel[]>(str);
        }

        public string GetFileName()
        {
            StringBuilder sb = new StringBuilder("JsonFiles");
            sb.Append("/" + _gepService.GetUserContext().BuyerPartnerCode);
            sb.Append("/" + _gepService.RegionId);
            sb.Append("/" + _gepService.GetUserContext().Culture + ".json");
            return sb.ToString();
        }

        public string GetDefaultFile()
        {
            StringBuilder sb = new StringBuilder("JsonFiles");
            sb.Append("/" + "Default");
            sb.Append("/" + _gepService.GetUserContext().Culture + ".json");
            return sb.ToString();
        }
    }
}
