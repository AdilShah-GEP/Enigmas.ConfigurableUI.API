using enigmas.ConfigurableUI.service.Implementation;
using enigmas.ConfigurableUI.service.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace enigmas.ConfigurableUI.API
{
    public class RegisterBusinessService
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<IDataLakeService, DataLakeService>();
        }
    }
}
