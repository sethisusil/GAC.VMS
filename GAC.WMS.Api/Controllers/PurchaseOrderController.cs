using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly IPurchaseOrderService _purchaseOrderService;
        public PurchaseOrderController(ILogger<PurchaseOrderController> logger, IPurchaseOrderService purchaseOrderService)
        {
            _logger = logger;
            _purchaseOrderService = purchaseOrderService;
        }

       
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Get)}: Initiating Get all purchase orders");
            try
            {
                var purchaseOrders = await _purchaseOrderService.GetAllAsync();
                if (purchaseOrders != null && purchaseOrders.Any())
                {
                    _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Get)}: Successfully fetched all purchase orders");
                    return Ok(purchaseOrders);
                }
                else
                {
                    _logger.LogWarning($"{nameof(PurchaseOrderController)}.{nameof(Get)}: No purchase orders found");
                    return NotFound("No purchase orders found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderController)}.{nameof(Get)}: An unexpected error occurred while fetching all purchase orders");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Get)}: Initiating Get purchase order with id:{id}");
            try
            {
                var purchaseOrder = await _purchaseOrderService.GetAsync(id);
                if (purchaseOrder != null)
                {
                    _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Get)}: Successfully fetched purchase order with id:{id}");
                    return Ok(purchaseOrder);
                }
                else
                {
                    _logger.LogWarning($"{nameof(PurchaseOrderController)}.{nameof(Get)}: No purchase order found with id:{id}");
                    return NotFound($"No purchase order found with id:{id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderController)}.{nameof(Get)}: An unexpected error occurred while fetching purchase order with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EndpointDescription("Create a new Purchase Order: Please leave Id filed as it is (0)")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PurchaseOrderRequest model)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Post)}: Initiating Create purchase order");
            try
            {
                var result = await _purchaseOrderService.CreateAsync(model);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Post)}: Successfully created purchase order with id:{result.Data.Id}");
                    return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result.Data);
                }
                else
                {
                    _logger.LogWarning($"{nameof(PurchaseOrderController)}.{nameof(Post)}: Failed to create purchase order - {result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderController)}.{nameof(Post)}: An unexpected error occurred while creating purchase order");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PurchaseOrderRequest model)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Put)}: Initiating Update purchase order with id:{id}");
            try
            {
                var result = await _purchaseOrderService.UpdateAsync(id, model);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Put)}: Successfully updated purchase order with id:{id}");
                    return Ok(result.Data);
                }
                else
                {
                    _logger.LogWarning($"{nameof(PurchaseOrderController)}.{nameof(Put)}: Failed to update purchase order - {result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderController)}.{nameof(Put)}: An unexpected error occurred while updating purchase order with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Delete)}: Initiating Delete purchase order with id:{id}");
            try
            {
                var result = await _purchaseOrderService.DeleteAsync(id);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Delete)}: Successfully deleted purchase order with id:{id}");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"{nameof(PurchaseOrderController)}.{nameof(Delete)}: Failed to delete purchase order - {result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderController)}.{nameof(Delete)}: An unexpected error occurred while deleting purchase order with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] List<PurchaseOrderRequest> purchaseOrders)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Upload)}: Initiating upload of purchase orders");
            try
            {
                if (purchaseOrders == null || !purchaseOrders.Any())
                {
                    _logger.LogWarning($"{nameof(PurchaseOrderController)}.{nameof(Upload)}: Request body is null or empty");
                    return BadRequest("Request body cannot be null or empty");
                }

                var result = await _purchaseOrderService.UploadPurchaseOrdersAsync(purchaseOrders);
                if (result.Success)
                {
                    _logger.LogInformation($"{nameof(PurchaseOrderController)}.{nameof(Upload)}: Successfully uploaded purchase orders");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"{nameof(PurchaseOrderController)}.{nameof(Upload)}: Failed to upload purchase orders - {result.Message}");
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderController)}.{nameof(Upload)}: An unexpected error occurred while uploading purchase orders");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
