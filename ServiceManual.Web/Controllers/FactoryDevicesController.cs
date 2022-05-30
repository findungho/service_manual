using ServiceManual.ApplicationCore.Entities;
using ServiceManual.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ServiceManual.Web.Controllers
{
    [Route("api/[controller]")]
    public class FactoryDevicesController : ControllerBase
    {
        private readonly IFactoryDeviceService _factoryDeviceService;

        public FactoryDevicesController(IFactoryDeviceService factoryDeviceService)
        {
            _factoryDeviceService = factoryDeviceService;
        }

        /// <summary>
        ///     HTTP GET: api/factorydevices/
        ///     Get all devices.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<FactoryDeviceDto>> Get()
        {
            try
            {
                var fds = await _factoryDeviceService.GetAll();
                var fdDtos = new List<FactoryDeviceDto>();
                var taskDtos = new List<MaintenanceTaskDto>();

                foreach (var fd in fds)
                {
                    if (fd.MaintenanceTasks.Count != 0)
                    {
                        foreach (var task in fd.MaintenanceTasks)
                        {
                            taskDtos.Add(new MaintenanceTaskDto
                            {
                                Id = task.Id,
                                Status = task.Status.ToString(),
                                Severity = task.Severity.ToString(),
                                Description = task.Description,
                                Created = task.Created,
                                Updated = task.Updated
                            });
                        }
                    }

                    var fdDto = new FactoryDeviceDto
                    {
                        Id = fd.Id,
                        Name = fd.Name,
                        Type = fd.Type,
                        Year = fd.Year,
                        MaintenanceTasks = taskDtos
                    };

                    fdDtos.Add(fdDto);
                }

                fdDtos.OrderBy(fd =>
                {
                    fd.MaintenanceTasks = fd.MaintenanceTasks
                        .OrderBy(task => task.Severity)
                        .ThenByDescending(task => task.Created)
                        .ToList();

                    return fd.Id;
                });

                return fdDtos;
            }
            catch (Exception)
            {
                return (IEnumerable<FactoryDeviceDto>)StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        ///     HTTP GET: api/factorydevices/search
        ///     Search devices by Severity.
        /// </summary>
        /// <param name="severity">Severity (int)</param>
        //[HttpGet("{search}")]
        //public async Task<ActionResult<IEnumerable<FactoryDevice>>> Search(Severity? severity)
        //{
        //    try
        //    {
        //        var result = await _factoryDeviceService.Search(severity);

        //        if (result.Any())
        //        {
        //            return Ok(result.ToList()
        //                .Select(fd =>
        //                    new FactoryDeviceDto
        //                    {
        //                        Id = fd.Id,
        //                        Name = fd.Name,
        //                        Year = fd.Year,
        //                        Type = fd.Type,
        //                        DateCreated = fd.DateCreated,
        //                        Severity = fd.Severity.ToString()
        //                    }
        //            ));
        //        }

        //        return NotFound();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error retrieving data from the database");
        //    }
        //}

        /// <summary>
        ///     HTTP GET: api/factorydevices/1
        ///     Get device by its Id.
        /// </summary>
        /// <param name="id">FactoryDevice Id</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _factoryDeviceService.Get(id);
                var taskDtos = new List<MaintenanceTaskDto>();

                if (result == null) return NotFound();

                if (result.MaintenanceTasks.Count != 0)
                {
                    foreach (var task in result.MaintenanceTasks)
                    {
                        taskDtos.Add(new MaintenanceTaskDto
                        {
                            Id = task.Id,
                            Status = task.Status.ToString(),
                            Severity = task.Severity.ToString(),
                            Description = task.Description,
                            Created = task.Created,
                            Updated = task.Updated
                        });
                    }
                }

                FactoryDeviceDto fdDto = new FactoryDeviceDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Type = result.Type,
                    Year = result.Year,
                    MaintenanceTasks = taskDtos
                };

                return Ok(fdDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        ///     HTTP POST: api/factorydevices
        ///     Create a new device.
        /// </summary>
        /// <param name="factoryDevice">FactoryDevice object</param>
        [HttpPost]
        public async Task<IActionResult> Create(FactoryDevice factoryDevice)
        {
            try
            {
                if (factoryDevice == null)
                    return BadRequest();

                var createFactoryDevice = await _factoryDeviceService.Add(factoryDevice);

                return CreatedAtAction(nameof(Get),
                    new { id = createFactoryDevice.Id }, createFactoryDevice);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new Factory Device record");
            }
        }

        /// <summary>
        ///     HTTP PUT: api/factorydevices/1
        ///     Update an existing device.
        /// </summary>
        /// <param name="id">FactoryDevice Id</param>
        /// <param name="factoryDevice">FactoryDevice object</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<FactoryDevice>> Update(int id, FactoryDevice factoryDevice)
        {
            try
            {
                if (id != factoryDevice.Id)
                    return BadRequest("Factory Device ID mismatch");

                var fdToUpdate = await _factoryDeviceService.Get(id);

                if (fdToUpdate == null)
                    return NotFound($"Factory Device with Id = {id} not found");

                return await _factoryDeviceService.Update(factoryDevice);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        /// <summary>
        ///     HTTP DELETE: api/factorydevices/1
        ///     Delete a device.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<FactoryDevice>> Delete(int id)
        {
            try
            {
                var factoryDeviceToDelete = await _factoryDeviceService.Get(id);

                if (factoryDeviceToDelete == null)
                {
                    return NotFound($"Factory Device with Id = {id} not found");
                }

                _factoryDeviceService.Delete(id);

                return Ok($"Deleted {factoryDeviceToDelete.Name}");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }

        /// <summary>
        ///     HTTP GET: api/factorydevices/1/tasks
        ///     Get all devices.
        /// </summary>
        [HttpGet("{id:int}/{tasks}")]
        public async Task<IEnumerable<MaintenanceTaskDto>> GetAllTasks(int id)
        {
            try
            {
                return (await _factoryDeviceService.GetAllTasks(id)).ToList()
                    .Select(task =>
                        new MaintenanceTaskDto
                        {
                            Id = task.Id,
                            Status = task.Status.ToString(),
                            Severity = task.Severity.ToString(),
                            Description = task.Description,
                            Created = task.Created,
                            Updated = task.Updated
                        }
                    );
            }
            catch (Exception)
            {
                return (IEnumerable<MaintenanceTaskDto>)StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        ///     HTTP GET: api/factorydevices/1/tasks/1
        ///     Get all devices.
        /// </summary>
        //    [HttpGet("{id:int}/tasks/{taskId:int}")]
        //    public async Task<IEnumerable<MaintenanceTaskDto>> GetTasksByFDId(int factoryDeviceId, int maintenanceTaskId)
        //    {
        //        try
        //        {
        //            return (await _factoryDeviceService.GetTasksByFDId(factoryDeviceId, maintenanceTaskId)).ToList()
        //                .Select(task =>
        //                    new MaintenanceTaskDto
        //                    {
        //                        Id = task.Id,
        //                        FDeviceId = task.FDeviceId,
        //                        Status = task.Status.ToString(),
        //                        Severity = task.Severity.ToString(),
        //                        Description = task.Description,
        //                        Created = task.Created,
        //                        Updated = task.Updated
        //                    }
        //                );
        //        }
        //        catch (Exception)
        //        {
        //            return (IEnumerable<MaintenanceTaskDto>)StatusCode(StatusCodes.Status500InternalServerError,
        //                "Error retrieving data from the database");
        //        }
        //    }
    }
}