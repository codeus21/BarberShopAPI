using Microsoft.AspNetCore.Mvc;
using BarberShopAPI.Server.Repositories;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ServiceRepository _serviceRepository;

        public ServicesController(ServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _serviceRepository.GetActiveServicesAsync();
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }

        // POST: api/Services
        [HttpPost]
        public async Task<ActionResult<Service>> CreateService(Service service)
        {
            var createdService = await _serviceRepository.AddAsync(service);
            return CreatedAtAction(nameof(GetService), new { id = createdService.Id }, createdService);
        }

        // PUT: api/Services/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            try
            {
                await _serviceRepository.UpdateAsync(service);
            }
            catch
            {
                if (!await _serviceRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}