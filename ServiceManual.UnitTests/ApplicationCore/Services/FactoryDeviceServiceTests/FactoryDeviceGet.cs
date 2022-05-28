using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiceManual.ApplicationCore.Entities;
using ServiceManual.ApplicationCore.Interfaces;
using ServiceManual.ApplicationCore.Services;
using Xunit;

namespace ServiceManual.UnitTests.ApplicationCore.Services.FactoryDeviceServiceTests
{
    public class FactoryDeviceGet
    {
        [Fact]
        public async void AllCars()
        {
            FactoryDeviceContext factoryDeviceContext = new FactoryDeviceContext(UseInMemoryDatabase("ServiceManual"));
            IFactoryDeviceService factoryDeviceService = new FactoryDeviceService(factoryDeviceContext);

            var fds = (await factoryDeviceService.GetAll()).ToList();

            Assert.NotNull(fds);
            Assert.NotEmpty(fds);
            Assert.Equal(3, fds.Count);
        }

        private DbContextOptions<FactoryDeviceContext> UseInMemoryDatabase(string v)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async void ExistingCardWithId()
        {
            FactoryDeviceContext factoryDeviceContext = new FactoryDeviceContext(UseInMemoryDatabase("ServiceManual"));
            IFactoryDeviceService FactoryDeviceService = new FactoryDeviceService(factoryDeviceContext);
            int fdId = 1;

            var fd = await FactoryDeviceService.Get(fdId);

            Assert.NotNull(fd);
            Assert.Equal(fdId, fd.Id);
        }

        [Fact]
        public async void NonExistingCardWithId()
        {
            FactoryDeviceContext factoryDeviceContext = new FactoryDeviceContext(UseInMemoryDatabase("ServiceManual"));
            IFactoryDeviceService FactoryDeviceService = new FactoryDeviceService(factoryDeviceContext);
            int fdId = 4;

            var fd = await FactoryDeviceService.Get(fdId);

            Assert.Null(fd);
        }
    }
}