using dm.lib.core.nuget;
using enigmas.ConfigurableUI.Core.Entity;
using enigmas.ConfigurableUI.DataAccess.Interface;
using enigmas.ConfigurableUI.service.Implementation;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace enigmas.ConfigurableUI.Service.Test
{
    public class DataLakeServiceTest
    {

        private Mock<IGepService> _gepservice;
        private Mock<IDataLakeDA> _dataLakeDA;
        private DataLakeService dataLake;

        [SetUp]
        public void Setup()
        {
            _gepservice = new Mock<IGepService>();
            _dataLakeDA = new Mock<IDataLakeDA>();
            dataLake = new DataLakeService(_gepservice.Object, _dataLakeDA.Object);
        }



    }
}
