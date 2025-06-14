using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;
        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Get)}: Initiating Get all customers");
            try
            {
                var customers = await _customerService.GetAllAsync();
                if (customers != null && customers.Any())
                {
                    _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Get)}: Successfully fetched all customers");
                    return Ok(customers);
                }
                else
                {
                    _logger.LogWarning($"{nameof(CustomerController)}.{nameof(Get)}: No customers found");
                    return NotFound("No customers found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerController)}.{nameof(Get)}: An un-expected error ocurred while fetching all customers");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Get)}: Initiating Get customer with id:{id}");
            try
            {
                var customer = await _customerService.GetAsync(id);
                if (customer != null)
                {
                    _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Get)}: Successfully fetched customer with id:{id}");
                    return Ok(customer);
                }
                else
                {
                    _logger.LogWarning($"{nameof(CustomerController)}.{nameof(Get)}: No customer found with id:{id}");
                    return NotFound("No customers found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerController)}.{nameof(Get)}: An un-expected error ocurred while fetching customer with id:{id}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [EndpointDescription("Create a new customer: Please leave Id filed as it is (0) same for AddressId as well")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerRequest model)
        {
            _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Post)}: Initiating Create customer with Request:{JsonSerializer.Serialize(model)}");
            try
            {
                if (model == null)
                {
                    _logger.LogWarning($"{nameof(CustomerController)}.{nameof(Post)}: Request body is null");
                    return BadRequest("Request body cannot be null");
                }
                var result = await _customerService.CreateAsync(model);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Post)}: Successfully created customer with id:{result.Data.Id}");
                    return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result.Data);
                }
                else
                {
                    _logger.LogError($"{nameof(CustomerController)}.{nameof(Post)}: Error occurred while creating customer with Error:{result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerController)}.{nameof(Post)}: An un-expected error ocurred while creating customer");
                return StatusCode((int)HttpStatusCode.InternalServerError);

            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CustomerRequest model)
        {
            _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Put)}: Initiating Update customer with id:{id} and Request:{JsonSerializer.Serialize(model)}");
            try
            {
                if (model == null)
                {
                    _logger.LogWarning($"{nameof(CustomerController)}.{nameof(Put)}: Request body is null");
                    return BadRequest("Request body cannot be null");
                }
                var result = await _customerService.UpdateAsync(id, model);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Put)}: Successfully updated customer with id:{id}");
                    return Ok(result.Data);
                }
                else
                {
                    _logger.LogError($"{nameof(CustomerController)}.{nameof(Put)}: Error occurred while updating customer with Error:{result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerController)}.{nameof(Put)}: An un-expected error ocurred while updating customer with id:{id}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Delete)}: Initiating Delete customer with id:{id}");
            try
            {
                var result = await _customerService.DeleteAsync(id);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Delete)}: Successfully deleted customer with id:{id}");
                    return NoContent();
                }
                else
                {
                    _logger.LogError($"{nameof(CustomerController)}.{nameof(Delete)}: Error occurred while deleting customer with Error:{result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerController)}.{nameof(Delete)}: An un-expected error ocurred while deleting customer with id:{id}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] List<CustomerRequest> customers)
        {
            _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Upload)}: Initiating Upload customers with Request:{JsonSerializer.Serialize(customers)}");
            try
            {
                if (customers == null || !customers.Any())
                {
                    _logger.LogWarning($"{nameof(CustomerController)}.{nameof(Upload)}: Request body is null or empty");
                    return BadRequest("Request body cannot be null or empty");
                }
                var result = await _customerService.UploadCustomersAsync(customers);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(CustomerController)}.{nameof(Upload)}: Successfully uploaded customers");
                    return Ok(result);
                }
                else
                {
                    _logger.LogError($"{nameof(CustomerController)}.{nameof(Upload)}: Error occurred while uploading customers with Error:{result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerController)}.{nameof(Upload)}: An un-expected error ocurred while uploading customers");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
