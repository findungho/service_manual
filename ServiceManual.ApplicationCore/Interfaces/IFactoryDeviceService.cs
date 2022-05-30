using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceManual.ApplicationCore.Entities;

namespace ServiceManual.ApplicationCore.Interfaces
{
    public interface IFactoryDeviceService
    {
        Task<IEnumerable<FactoryDevice>> GetAll();

        //Task<IEnumerable<FactoryDevice>> Search(Severity? severity);

        Task<FactoryDevice> Get(int id);

        Task<FactoryDevice> Add(FactoryDevice factoryDevice);

        Task<FactoryDevice> Update(FactoryDevice factoryDevice);

        void Delete(int id);

        Task<IEnumerable<MaintenanceTask>> GetAllTasks(int factoryDeviceId);

        Task<IEnumerable<MaintenanceTask>> GetTasksByFDId(int factoryDeviceId, int maintenanceTaskId);

        //Task<MaintenanceTask> AddTask(int factoryDeviceId, MaintenanceTask maintenanceTask);

        //Task<MaintenanceTask> UpdateTask(int factoryDeviceId, int maintenanceTaskId, MaintenanceTask maintenanceTask);

        //Task DeleteTask(int FactoryDeviceId, int maintenanceTaskId);
    }
}