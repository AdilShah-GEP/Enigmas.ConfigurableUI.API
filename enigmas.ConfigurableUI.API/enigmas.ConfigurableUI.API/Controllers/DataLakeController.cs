using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using enigmas.ConfigurableUI.Core.Entity;
using enigmas.ConfigurableUI.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace enigmas.ConfigurableUI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataLakeController : ControllerBase
    {
        private readonly IDataLakeService _dataLake;
        public DataLakeController(IDataLakeService dataLake)
        {
            _dataLake = dataLake;
        }

        [HttpPost]
        public Task<bool> CreateFile(InputModel[] input)
        {
            return _dataLake.InsertData(Startup.connection,Startup.container, input);
        }

        [HttpGet]
        public Task<InputModel[]> GetData()
        {
            return _dataLake.GetData(Startup.connection, Startup.container);
        }
    }
}
