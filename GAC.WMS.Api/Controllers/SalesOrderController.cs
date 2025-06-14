using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private readonly ILogger<SalesOrderController> _logger;
        private readonly ISalesOrderService _salesOrderService;
        public SalesOrderController(ILogger<SalesOrderController> logger, ISalesOrderService salesOrderService)
        {
            _logger = logger;
            _salesOrderService = salesOrderService;
        }
       
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Get)}: Initiating Get all sales orders");
            try
            {
                var salesOrders = await _salesOrderService.GetAllAsync();
                if (salesOrders != null && salesOrders.Any())
                {
                    _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Get)}: Successfully fetched all sales orders");
                    return Ok(salesOrders);
                }
                else
                {
                    _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Get)}: No sales orders found");
                    return NotFound("No sales orders found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderController)}.{nameof(Get)}: An unexpected error occurred while fetching all sales orders");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Get)}: Initiating Get sales order with id:{id}");
            try
            {
                var salesOrder = await _salesOrderService.GetAsync(id);
                if (salesOrder != null)
                {
                    _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Get)}: Successfully fetched sales order with id:{id}");
                    return Ok(salesOrder);
                }
                else
                {
                    _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Get)}: No sales order found with id:{id}");
                    return NotFound($"No sales order found with id:{id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderController)}.{nameof(Get)}: An unexpected error occurred while fetching sales order with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EndpointDescription("Create a new Sales Order: Please leave Id filed as it is (0)")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SalesOrderRequest model)
        {
            _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Post)}: Initiating Create sales order");
            if (model == null)
            {
                _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Post)}: Invalid sales order data received");
                return BadRequest("Invalid sales order data");
            }
            else
            {
                try
                {
                    var result = await _salesOrderService.CreateAsync(model);
                    if (result.Success)
                    {
                        _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Post)}: Successfully created sales order with id:{result.Data.Id}");
                        return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result.Data);
                    }
                    else
                    {
                        _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Post)}: Failed to create sales order - {result.Message}");
                        return BadRequest(result.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{nameof(SalesOrderController)}.{nameof(Post)}: An unexpected error occurred while creating sales order");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SalesOrderRequest model)
        {
            _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Put)}: Initiating Update sales order with id:{id}");
            if (model == null)
            {
                _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Put)}: Invalid sales order data received");
                return BadRequest("Invalid sales order data");
            }
            try
            {
                var result = await _salesOrderService.UpdateAsync(id, model);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Put)}: Successfully updated sales order with id:{id}");
                    return Ok(result.Data);
                }
                else
                {
                    _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Put)}: Failed to update sales order - {result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderController)}.{nameof(Put)}: An unexpected error occurred while updating sales order with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Delete)}: Initiating Delete sales order with id:{id}");
            try
            {
                var result = await _salesOrderService.DeleteAsync(id);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Delete)}: Successfully deleted sales order with id:{id}");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Delete)}: Failed to delete sales order - {result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderController)}.{nameof(Delete)}: An unexpected error occurred while deleting sales order with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] List<SalesOrderRequest> salesOrders)
        {
            _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Upload)}: Initiating Upload sales orders");
            if (salesOrders == null || !salesOrders.Any())
            {
                _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Upload)}: No sales orders provided for upload");
                return BadRequest("No sales orders provided for upload");
            }
            try
            {
                var result = await _salesOrderService.UploadSalesOrdersAsync(salesOrders);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(SalesOrderController)}.{nameof(Upload)}: Successfully uploaded sales orders");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"{nameof(SalesOrderController)}.{nameof(Upload)}: Failed to upload sales orders - {result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderController)}.{nameof(Upload)}: An unexpected error occurred while uploading sales orders");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
