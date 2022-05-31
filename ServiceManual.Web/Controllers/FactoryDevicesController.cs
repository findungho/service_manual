using ServiceManual.ApplicationCore.Entities;
using ServiceManual.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        ///     HTTP GET: api/factorydevices
        ///     Get all devices.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<FactoryDeviceDto>> Get()
        {
            try
            {
                var fds = await _factoryDeviceService.GetAll();
                var fdDtos = new List<FactoryDeviceDto>();

                foreach (var fd in fds)
                {
                    var fdDto = new FactoryDeviceDto
                    {
                        Id = fd.Id,
                        Name = fd.Name,
                        Type = fd.Type,
                        Year = fd.Year,
                        MaintenanceTasks = new List<MaintenanceTaskDto>()
                    };

                    foreach (var task in fd.MaintenanceTasks)
                    {
                        if (task.FactoryDeviceId == fdDto.Id)
                        {
                            fdDto.MaintenanceTasks.Add(new MaintenanceTaskDto
                            {
                                Id = task.Id,
                                Status = task.Status.ToString(),
                                Severity = task.Severity.ToString(),
                                Description = task.Description,
                                Created = task.Created,
                                Updated = task.Updated,
                                FactoryDeviceId = fdDto.Id
                            });
                        }
                    }

                    fdDtos.Add(fdDto);
                }

                return fdDtos;
            }
            catch (Exception)
            {
                return (IEnumerable<FactoryDeviceDto>)StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        ///     HTTP GET: api/factorydevices/1
        ///     Get device by its Id.
        /// </summary>
        /// <param name="deviceId">FactoryDevice Id</param>
        [HttpGet("{deviceId:int}")]
        public async Task<IActionResult> Get(int deviceId)
        {
            try
            {
                var result = await _factoryDeviceService.Get(deviceId);
                var taskDtos = new List<MaintenanceTaskDto>();

                if (result == null) return NotFound($"Factory Device with Id {deviceId} not found!");

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
        /// <param name="deviceId">FactoryDevice Id</param>
        /// <param name="factoryDevice">FactoryDevice object</param>
        [HttpPut("{deviceId:int}")]
        public async Task<ActionResult<FactoryDevice>> Update(int deviceId, FactoryDevice factoryDevice)
        {
            try
            {
                if (deviceId != factoryDevice.Id)
                    return BadRequest("Factory Device ID mismatch");

                var fdToUpdate = await _factoryDeviceService.Get(deviceId);

                if (fdToUpdate == null)
                    return NotFound($"Factory Device with Id {deviceId} not found");

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
        /// <param name="deviceId">Factory Device Id</param>
        [HttpDelete("{deviceId:int}")]
        public async Task<ActionResult<FactoryDevice>> Delete(int deviceId)
        {
            try
            {
                var factoryDeviceToDelete = await _factoryDeviceService.Get(deviceId);

                if (factoryDeviceToDelete == null)
                {
                    return NotFound($"Factory Device with Id = {deviceId} not found");
                }

                _factoryDeviceService.Delete(deviceId);

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
        ///     Get all tasks that belong to specific Factory Device.
        /// </summary>
        /// <param name="deviceId">Factory Device Id</param>
        [HttpGet("{deviceId:int}/{tasks}")]
        public async Task<IEnumerable<MaintenanceTaskDto>> GetAllTasks(int deviceId)
        {
            try
            {
                return (await _factoryDeviceService.GetAllTasks(deviceId)).ToList()
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
        ///     Get task by task Id.
        /// </summary>
        /// <param name="deviceId">Factory Device Id</param>
        /// <param name="taskId">MaintenanceTask Id</param>
        [HttpGet("{deviceId:int}/{tasks}/{taskId:int}")]
        public async Task<IActionResult> GetTasksByTaskId(int deviceId, int taskId)
        {
            try
            {
                var result = await _factoryDeviceService.GetTasksByTaskId(deviceId, taskId);
                if (result == null) return NotFound();
                MaintenanceTaskDto taskDto = new MaintenanceTaskDto
                {
                    Id = result.Id,
                    Status = result.Status.ToString(),
                    Severity = result.Severity.ToString(),
                    Description = result.Description,
                    Created = result.Created,
                    Updated = result.Updated
                };

                return Ok(taskDto);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        ///     HTTP POST: api/factorydevices/1/tasks
        ///     Add new task to Factory Device.
        /// </summary>
        /// <param name="deviceId">Factory Device Id</param>
        /// <param name="maintenanceTask">MaintenanceTask Object</param>
        [HttpPost("{deviceId:int}/{tasks}")]
        public async Task<IActionResult> AddTask(int deviceId, MaintenanceTask maintenanceTask)
        {
            try
            {
                if (deviceId < 0 && maintenanceTask == null)
                    return BadRequest();

                maintenanceTask.Created = DateTime.Now;
                var addTask = await _factoryDeviceService.AddTask(deviceId, maintenanceTask);

                return Ok(new MaintenanceTaskDto
                {
                    Id = addTask.Id,
                    Status = maintenanceTask.Status.ToString(),
                    Severity = maintenanceTask.Severity.ToString(),
                    Description = maintenanceTask.Description,
                    Created = maintenanceTask.Created,
                    Updated = maintenanceTask.Updated
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new MaintenanceTask record");
            }
        }

        /// <summary>
        ///     HTTP PUT: api/factorydevices/1/tasks/1
        ///     Update an existing task by task Id.
        /// </summary>
        /// <param name="deviceId">Factory Device Id</param>
        /// <param name="taskId">MaintenanceTask Id</param>
        /// <param name="maintenanceTask">MaintenanceTask Object</param>
        [HttpPut("{deviceId:int}/{tasks}/{taskId:int}")]
        public async Task<IActionResult> UpdateTask(int deviceId, int taskId, MaintenanceTask maintenanceTask)
        {
            try
            {
                if (deviceId < 0 && taskId < 0 && maintenanceTask == null)
                    return BadRequest();

                maintenanceTask.Updated = DateTime.Now;
                var updatedTask = await _factoryDeviceService.UpdateTask(deviceId, taskId, maintenanceTask);

                return Ok(new MaintenanceTaskDto
                {
                    Id = updatedTask.Id,
                    Status = maintenanceTask.Status.ToString(),
                    Severity = maintenanceTask.Severity.ToString(),
                    Description = maintenanceTask.Description,
                    Created = maintenanceTask.Created,
                    Updated = maintenanceTask.Updated
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new MaintenanceTask record");
            }
        }

        /// <summary>
        ///     HTTP DELETE: api/factorydevices/1/tasks/1
        ///     Delete a task.
        /// </summary>
        /// <param name="deviceId">Factory Device Id</param>
        /// <param name="taskId">MaintenanceTask Id</param>
        [HttpDelete("{deviceId:int}/{tasks}/{taskId}")]
        public async Task<ActionResult<MaintenanceTask>> DeleteTask(int deviceId, int taskId)
        {
            try
            {
                var taskToDelete = await _factoryDeviceService.GetTasksByTaskId(deviceId, taskId);

                if (taskToDelete == null)
                {
                    return NotFound($"MaintenanceTask with Id = {taskId} not found");
                }

                _factoryDeviceService.DeleteTask(deviceId, taskId);

                return Ok($"Deleted {taskToDelete.Id}");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}