using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceManual.ApplicationCore.Entities;

namespace ServiceManual.ApplicationCore.Interfaces
{
    public interface IFactoryDeviceService
    {
        Task<IEnumerable<FactoryDevice>> GetAll();

        Task<IEnumerable<FactoryDevice>> Search(Severity? severity);

        Task<FactoryDevice> Get(int id);

        Task<FactoryDevice> Add(FactoryDevice factoryDevice);

        Task<FactoryDevice> Update(FactoryDevice factoryDevice);

        void Delete(int id);
    }
}