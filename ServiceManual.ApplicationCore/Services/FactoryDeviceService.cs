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

        /// <summary>
        ///     Get all devices from database then sort by Severity and Create date.
        /// </summary>
        public async Task<IEnumerable<FactoryDevice>> GetAll()
        {
            var factoryDevices = await _factoryDeviceContext.FactoryDevices.ToListAsync();
            var sortedList = factoryDevices
              .OrderBy(fd => (int)(fd.Severity))
              .ThenByDescending(fd => fd.DateCreated)
              .ToList();

            return sortedList;
        }

        /// <summary>
        ///     Search devices from database by Severity.
        /// </summary>
        /// <param name="severity">Severity (int)</param>
        public async Task<IEnumerable<FactoryDevice>> Search(Severity? severity)
        {
            IQueryable<FactoryDevice> query = _factoryDeviceContext.FactoryDevices;

            if (severity != null)
            {
                query = query.Where(fd => fd.Severity == severity);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        ///     Get a devices by its Id from database.
        /// </summary>
        public async Task<FactoryDevice> Get(int id)
        {
            return await _factoryDeviceContext.FactoryDevices
                .FirstOrDefaultAsync(fd => fd.Id == id);
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
                result.Severity = factoryDevice.Severity;

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
    }
}