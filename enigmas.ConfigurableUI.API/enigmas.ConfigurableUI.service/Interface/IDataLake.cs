using enigmas.ConfigurableUI.Core.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace enigmas.ConfigurableUI.service.Interface
{
    public interface IDataLake
    {
        public Task<bool> InsertData(string conn, string container,  InputModel input);

        public Task<InputModel> GetData(string conn, string container);
    }
}
