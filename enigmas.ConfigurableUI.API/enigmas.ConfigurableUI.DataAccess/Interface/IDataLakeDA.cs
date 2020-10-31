using enigmas.ConfigurableUI.Core.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace enigmas.ConfigurableUI.DataAccess.Interface
{
    public interface IDataLakeDA
    {
        public Task<MemoryStream> GetData(InputParamsDataLake inputParamsDataLake, string defaultFile);

        public Task<bool> InsertData(InputParamsDataLake inputParamsDataLake, InputModel[] input);

    }
}
