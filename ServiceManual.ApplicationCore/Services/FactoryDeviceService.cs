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
            //var newListOfFD = new List<FactoryDevice>();

            //foreach (var factoryDevice in factoryDevices)
            //{
            //    if (factoryDevice.MaintenanceTasks.Count != 0)
            //    {
            //        foreach(var task in factoryDevice.MaintenanceTasks)
            //        {
            //            if (task.FactoryDeviceId != factoryDevice.Id)
            //            {
            //                factoryDevice.MaintenanceTasks = null;
            //            }
            //        }
            //    }

            //    newListOfFD.Add(factoryDevice);
            //}

            var sortedList = factoryDevices.OrderBy(
                fd =>
                {
                    fd.MaintenanceTasks = SortedByTaskSeverity(fd.MaintenanceTasks);
                    return fd.Id;
                }).ToList();

            return sortedList;
        }

        /// <summary>
        ///     Get a devices by its Id from database.
        /// </summary>
        /// <param name="id">Factory Device Id</param>
        public async Task<FactoryDevice> Get(int id)
        {
            var result = await _factoryDeviceContext.FactoryDevices.Include(fd => fd.MaintenanceTasks)
                .FirstOrDefaultAsync(fd => fd.Id == id);

            if (result == null) return null;

            result.MaintenanceTasks = SortedByTaskSeverity(result.MaintenanceTasks);

            return result;
        }

        /// <summary>
        ///     Add a new device to the database.
        /// </summary>
        /// <param name="factoryDevice">Factory Device object</param>
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
        public async void Delete(int factoryDeviceId)
        {
            var result = await _factoryDeviceContext.FactoryDevices
                .FirstOrDefaultAsync(fd => fd.Id == factoryDeviceId);

            if (result != null)
            {
                _factoryDeviceContext.FactoryDevices.Remove(result);
                _factoryDeviceContext.SaveChanges();
            }
        }

        /// <summary>
        ///     Get all tasks that belong to a specific Factory Device Id.
        /// </summary>
        /// <param name="id">FactoryDevice Id</param>
        public async Task<IEnumerable<MaintenanceTask>> GetAllTasks(int factoryDeviceId)
        {
            var fd = await _factoryDeviceContext.FactoryDevices.Include(fd => fd.MaintenanceTasks)
                .FirstOrDefaultAsync(fd => fd.Id == factoryDeviceId);

            if (!fd.MaintenanceTasks.Any()) return Enumerable.Empty<MaintenanceTask>();

            fd.MaintenanceTasks = SortedByTaskSeverity(fd.MaintenanceTasks);

            return fd.MaintenanceTasks;
        }

        /// <summary>
        ///     Get task by task Id.
        /// </summary>
        /// <param name="factoryDeviceId">FactoryDevice Id</param>
        /// <param name="maintenanceTaskId">MaintenanceTask Id</param>
        public async Task<MaintenanceTask> GetTasksByTaskId(int factoryDeviceId, int maintenanceTaskId)
        {
            var fd = await _factoryDeviceContext.FactoryDevices.Include(fd => fd.MaintenanceTasks)
                .FirstOrDefaultAsync(fd => fd.Id == factoryDeviceId);
            var task = new MaintenanceTask();

            if (fd != null)
            {
                task = fd.MaintenanceTasks.FirstOrDefault(task => task.Id == maintenanceTaskId);
            }

            return task;
        }

        /// <summary>
        ///     Add a new task for a Factory Device.
        /// </summary>
        /// <param name="factoryDeviceId">FactoryDevice Id</param>
        /// <param name="maintenanceTask">MaintenanceTask object</param>
        public async Task<MaintenanceTask> AddTask(int factoryDeviceId, MaintenanceTask maintenanceTask)
        {

            var fd = await _factoryDeviceContext.FactoryDevices
                .FirstOrDefaultAsync(fd => fd.Id == factoryDeviceId);

            maintenanceTask.FactoryDeviceId = factoryDeviceId;

            var result = await _factoryDeviceContext.AddAsync(maintenanceTask);
            await _factoryDeviceContext.SaveChangesAsync();
            return result.Entity;
        }

        /// <summary>
        ///     Update an existing MaintenanceTask.
        /// </summary>
        /// <param name="factoryDeviceId">FactoryDevice Id</param>
        /// <param name="maintenanceTaskId">MaintenanceTask Id</param>
        /// <param name="maintenanceTask">MaintenanceTask object</param>
        public async Task<MaintenanceTask> UpdateTask(int factoryDeviceId, int maintenanceTaskId, MaintenanceTask maintenanceTask)
        {
            var result = await _factoryDeviceContext.FactoryDevices.Include(fd => fd.MaintenanceTasks)
                .FirstOrDefaultAsync(fd => fd.Id == factoryDeviceId);

            if (result != null && result.MaintenanceTasks.Count != 0)
            {
                var taskToUpdate = result.MaintenanceTasks.FirstOrDefault(task => task.Id == maintenanceTaskId);
                if (taskToUpdate != null)
                {
                    taskToUpdate.FactoryDeviceId = factoryDeviceId;
                    taskToUpdate.Status = maintenanceTask.Status;
                    taskToUpdate.Severity = maintenanceTask.Severity;
                    taskToUpdate.Description = maintenanceTask.Description;
                    taskToUpdate.Updated = DateTime.Now;

                    await _factoryDeviceContext.SaveChangesAsync();

                    return taskToUpdate;
                }
            }

            return null;
        }

        /// <summary>
        ///     Delete an existing task from database by its Id
        ///     from a Factory Device Id where it belongs to.
        /// </summary>
        /// <param name="id">FactoryDevice Id</param>
        /// <param name="maintenanceTaskId">MaintenanceTask Id</param>
        public void DeleteTask(int factoryDeviceId, int maintenanceTaskId)
        {
            var result = _factoryDeviceContext.FactoryDevices.Include(fd => fd.MaintenanceTasks)
                .FirstOrDefault(fd => fd.Id == factoryDeviceId);

            if (result != null)
            {
                var taskToDelete = result.MaintenanceTasks.FirstOrDefault(task => task.Id == maintenanceTaskId);
                result.MaintenanceTasks.Remove(taskToDelete);
                _factoryDeviceContext.SaveChanges();
            }
        }
    }
}