using enigmas.ConfigurableUI.DataAccess.Implementation;
using enigmas.ConfigurableUI.DataAccess.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace enigmas.ConfigurableUI.API
{
    public class RegisterDataAccess
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<IDataLakeDA, DataLakeDA>();
        }
    }
}
