using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ServiceManual.ApplicationCore.Entities;
using ServiceManual.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ServiceManual.ApplicationCore.Services
{
    public class FactoryDeviceService : IFactoryDeviceService
    {
        private readonly FactoryDeviceContext _factoryDeviceContext;

        public FactoryDeviceService(FactoryDeviceContext factoryDeviceContext)
        {
            this._factoryDeviceContext = factoryDeviceContext;
        }

        public ICollection<MaintenanceTask> SortedByTaskSeverity(IEnumerable<MaintenanceTask> tasks)
        {
            return tasks.OrderBy(task => task.Severity)
                        .ThenByDescending(task => task.Created)
                        .ToList();
        }

        /// <summary>
        ///     Get all devices from database then sort by Task Severity and Create date.
        /// </summary>
        public async Task<IEnumerable<FactoryDevice>> GetAll()
        {
            var factoryDevices = await _factoryDeviceContext.FactoryDevices
                                                            .Include(fd => fd.MaintenanceTasks)
                                                            .ToListAsync();

            var sortedList = factoryDevices.OrderBy(
                fd =>
                {
                    fd.MaintenanceTasks = SortedByTaskSeverity(fd.MaintenanceTasks);
                    return fd.Id;
                }).ToList();

            return factoryDevices;
        }

        /// <summary>
        ///     Search devices from database by Severity.
        /// </summary>
        /// <param name="severity">Severity (int)</param>
        //public async Task<IEnumerable<FactoryDevice>> Search(Severity? severity)
        //{
        //    IQueryable<FactoryDevice> query = _factoryDeviceContext.FactoryDevices.Include("MaintenanceTask");

        //    if (severity != null)
        //    {
        //        query = query.Where(fd => fd.MaintenanceTask.Severity == severity);
        //    }

        //    return await query.ToListAsync();
        //}

        /// <summary>
        ///     Get a devices by its Id from database.
        /// </summary>
        public async Task<FactoryDevice> Get(int id)
        {
            var result = await _factoryDeviceContext.FactoryDevices.Include(fd => fd.MaintenanceTasks)
                .FirstOrDefaultAsync(fd => fd.Id == id);

            result.MaintenanceTasks = SortedByTaskSeverity(result.MaintenanceTasks);

            return result;
        }

        /// <summary>
        ///     Add a new device to the database.
        /// </summary>
        public async Task<FactoryDevice> Add(FactoryDevice factoryDevice)
        {
            var result = await _factoryDeviceContext.FactoryDevices.AddAsync(factoryDevice);
            await _factoryDeviceContext.SaveChangesAsync();
            return result.Entity;
        }

        /// <summary>
        ///     Update an existing device in database.
        /// </summary>
        /// <param name="factoryDevice">Factory Device object</param>
        public async Task<FactoryDevice> Update(FactoryDevice factoryDevice)
        {
            var result = await _factoryDeviceContext.FactoryDevices
                .FirstOrDefaultAsync(fd => fd.Id == factoryDevice.Id);

            if (result != null)
            {
                result.Name = factoryDevice.Name;
                result.Year = factoryDevice.Year;
                result.Type = factoryDevice.Type;

                await _factoryDeviceContext.SaveChangesAsync();

                return result;
            }

            return null;
        }

        /// <summary>
        ///     Delete an existing device from database by its Id.
        /// </summary>
        /// <param name="id">FactoryDevice Id</param>
        public async void Delete(int id)
        {
            var result = await _factoryDeviceContext.FactoryDevices
                .FirstOrDefaultAsync(fd => fd.Id == id);

            if (result != null)
            {
                _factoryDeviceContext.FactoryDevices.Remove(result);
                _factoryDeviceContext.SaveChanges();
            }
        }

        public async Task<IEnumerable<MaintenanceTask>> GetAllTasks(int id)
        {
            var fd = await _factoryDeviceContext.FactoryDevices.Include(fd => fd.MaintenanceTasks)
                .FirstOrDefaultAsync(fd => fd.Id == id);

            if (!fd.MaintenanceTasks.Any()) return Enumerable.Empty<MaintenanceTask>();

            fd.MaintenanceTasks = SortedByTaskSeverity(fd.MaintenanceTasks);

            return fd.MaintenanceTasks;
        }

        public async Task<IEnumerable<MaintenanceTask>> GetTasksByFDId(int factoryDeviceId, int maintenanceTaskId)
        {
            var fd = await _factoryDeviceContext.FactoryDevices.Include("MaintenanceTask")
                .FirstOrDefaultAsync(fd => fd.Id == factoryDeviceId);
            var task = new List<MaintenanceTask>();

            if (fd != null)
            {
                task = fd.MaintenanceTasks.Where(task => task.Id == maintenanceTaskId).ToList();
            }

            return task;
        }

        //public async Task<MaintenanceTask> AddTask(int factoryDeviceId, MaintenanceTask maintenanceTask)
        //{

        //    var fd = await _factoryDeviceContext.FactoryDevices.Include("MaintenanceTask")
        //        .FirstOrDefaultAsync(fd => fd.Id == factoryDeviceId);
        //    var a = await _factoryDeviceContext.AddAsync(maintenanceTask);
        //    //var result = await _factoryDeviceContext.FactoryDevices;
        //    await _factoryDeviceContext.SaveChangesAsync();
        //    return a.Entity;
        //}
    }
}