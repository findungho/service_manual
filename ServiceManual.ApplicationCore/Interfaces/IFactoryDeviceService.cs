using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceManual.ApplicationCore.Entities;

namespace ServiceManual.ApplicationCore.Interfaces
{
    public interface IFactoryDeviceService
    {
        Task<IEnumerable<FactoryDevice>> GetAll();

        Task<FactoryDevice> Get(int factoryDeviceId);

        Task<FactoryDevice> Add(FactoryDevice factoryDevice);

        Task<FactoryDevice> Update(FactoryDevice factoryDevice);

        void Delete(int factoryDeviceId);

        Task<IEnumerable<MaintenanceTask>> GetAllTasks(int factoryDeviceId);

        Task<MaintenanceTask> GetTasksByTaskId(int factoryDeviceId, int maintenanceTaskId);

        Task<MaintenanceTask> AddTask(int factoryDeviceId, MaintenanceTask maintenanceTask);

        Task<MaintenanceTask> UpdateTask(int factoryDeviceId, int maintenanceTaskId, MaintenanceTask maintenanceTask);

        void DeleteTask(int factoryDeviceId, int maintenanceTaskId);
    }
}