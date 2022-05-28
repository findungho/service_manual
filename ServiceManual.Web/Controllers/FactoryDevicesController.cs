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
                return (await _factoryDeviceService.GetAll()).ToList()
                    .Select(fd =>
                        new FactoryDeviceDto
                        {
                            Id = fd.Id,
                            Name = fd.Name,
                            Year = fd.Year,
                            Type = fd.Type,
                            DateCreated = fd.DateCreated,
                            Severity = fd.Severity.ToString()
                        }
                    );
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
        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<FactoryDevice>>> Search(Severity? severity)
        {
            try
            {
                var result = await _factoryDeviceService.Search(severity);

                if (result.Any())
                {
                    return Ok(result.ToList()
                        .Select(fd =>
                            new FactoryDeviceDto
                            {
                                Id = fd.Id,
                                Name = fd.Name,
                                Year = fd.Year,
                                Type = fd.Type,
                                DateCreated = fd.DateCreated,
                                Severity = fd.Severity.ToString()
                            }
                    ));
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

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

                if (result == null) return NotFound();

                return Ok(new FactoryDeviceDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Year = result.Year,
                    Type = result.Type,
                    Severity = result.Severity.ToString()

                });
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

                var employeeToUpdate = await _factoryDeviceService.Get(id);

                if (employeeToUpdate == null)
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
    }
}