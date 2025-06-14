using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(Get)}: Initiating Get all products");
            try
            {
                var products = await _productService.GetAllAsync();
                if (products != null && products.Any())
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(Get)}: Successfully fetched all products");
                    return Ok(products);
                }
                else
                {
                    _logger.LogWarning($"{nameof(ProductController)}.{nameof(Get)}: No products found");
                    return NotFound("No products found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(Get)}: An un-expected error ocurred while fetching all products");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(Get)}: Initiating Get product with id:{id}");
            try
            {
                var product = await _productService.GetAsync(id);
                if (product != null)
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(Get)}: Successfully fetched product with id:{id}");
                    return Ok(product);
                }
                else
                {
                    _logger.LogWarning($"{nameof(ProductController)}.{nameof(Get)}: No product found with id:{id}");
                    return NotFound($"No product found with id:{id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(Get)}: An un-expected error ocurred while fetching product with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(GetByCode)}: Initiating Get product with code:{code}");
            try
            {
                var product = await _productService.GetByCodeAsync(code);
                if (product != null)
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(GetByCode)}: Successfully fetched product with code:{code}");
                    return Ok(product);
                }
                else
                {
                    _logger.LogWarning($"{nameof(ProductController)}.{nameof(GetByCode)}: No product found with code:{code}");
                    return NotFound($"No product found with code:{code}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(GetByCode)}: An un-expected error ocurred while fetching product with code:{code}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EndpointDescription("Create a new Product: Please leave Id filed as it is (0) same for DimensionsId as well")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductRequest model)
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(Post)}: Initiating create product with Request:{System.Text.Json.JsonSerializer.Serialize(model)}");
            try
            {
                if (model == null)
                {
                    _logger.LogWarning($"{nameof(CustomerController)}.{nameof(Post)}: Request body is null");
                    return BadRequest("Request body cannot be null");
                }

                var opResult = await _productService.CreateAsync(model);
                if (opResult.Success)
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(Post)}: Successfully created product with id:{opResult.Data.Id}");
                    return CreatedAtAction(nameof(Get), new { id = opResult.Data.Id }, opResult.Data);
                }
                else
                {
                    _logger.LogError($"{nameof(ProductController)}.{nameof(Post)}: Error occurred while creating product with Error:{opResult.Message}");
                    return BadRequest(opResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(Post)}: An un-expected error ocurred while creating product");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductRequest model)
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(Put)}: Initiating update product with id:{id} and Request:{System.Text.Json.JsonSerializer.Serialize(model)}");
            try
            {
                if (model == null)
                {
                    _logger.LogWarning($"{nameof(ProductController)}.{nameof(Put)}: Request body is null");
                    return BadRequest("Request body cannot be null");
                }
                var opResult = await _productService.UpdateAsync(id, model);
                if (opResult.Success)
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(Put)}: Successfully updated product with id:{id}");
                    return Ok(opResult.Data);
                }
                else
                {
                    _logger.LogError($"{nameof(ProductController)}.{nameof(Put)}: Error occurred while updating product with Error:{opResult.Message}");
                    return BadRequest(opResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(Put)}: An un-expected error ocurred while updating product with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(Delete)}: Initiating delete product with id:{id}");
            try
            {
                var opResult = await _productService.DeleteAsync(id);
                if (opResult.Success)
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(Delete)}: Successfully deleted product with id:{id}");
                    return NoContent();
                }
                else
                {
                    _logger.LogError($"{nameof(ProductController)}.{nameof(Delete)}: Error occurred while deleting product with Error:{opResult.Message}");
                    return BadRequest(opResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(Delete)}: An un-expected error ocurred while deleting product with id:{id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("code/{code}")]
        public async Task<IActionResult> DeleteByCode(string code)
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(DeleteByCode)}: Initiating delete product with code:{code}");
            try
            {
                var opResult = await _productService.DeleteAsync(code);
                if (opResult.Success)
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(DeleteByCode)}: Successfully deleted product with code:{code}");
                    return NoContent();
                }
                else
                {
                    _logger.LogError($"{nameof(ProductController)}.{nameof(DeleteByCode)}: Error occurred while deleting product with Error:{opResult.Message}");
                    return BadRequest(opResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(DeleteByCode)}: An un-expected error ocurred while deleting product with code:{code}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] List<ProductRequest> products)
        {
            _logger.LogInformation($"{nameof(ProductController)}.{nameof(Upload)}: Initiating upload products with Request:{System.Text.Json.JsonSerializer.Serialize(products)}");
            try
            {
                if (products == null || !products.Any())
                {
                    _logger.LogWarning($"{nameof(ProductController)}.{nameof(Upload)}: Request body is null or empty");
                    return BadRequest("Request body cannot be null or empty");
                }
                var opResult = await _productService.UploadProductsAsync(products);
                if (opResult.Success)
                {
                    _logger.LogInformation($"{nameof(ProductController)}.{nameof(Upload)}: Successfully uploaded products");
                    return Ok(opResult);
                }
                else
                {
                    _logger.LogError($"{nameof(ProductController)}.{nameof(Upload)}: Error occurred while uploading products with Error:{opResult.Message}");
                    return BadRequest(opResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)}.{nameof(Upload)}: An un-expected error ocurred while uploading products");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
