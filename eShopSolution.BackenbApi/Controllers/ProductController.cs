using eShopSolution.Application.Catalog;
using eShopSolution.Application.Catalog.ProductImages;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ProductImageViewModel = eShopSolution.Application.Catalog.ProductImages.ProductImageViewModel;

namespace eShopSolution.BackenbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IPublicProductService _publicProductService;
        private readonly IManageProductService _publicManageService;
        public ProductController(IPublicProductService publicProductService, IManageProductService publicManageService)
        {
            _publicProductService = publicProductService;
            _publicManageService = publicManageService;
        }
       // http://localhost:port/product/
        [HttpGet("{languageId}")]
        public async Task<IActionResult> Get(string languageId)
        {
            var products = await _publicProductService.GetAll(languageId);
            return Ok(products);

        }

        //http://localhost:port/products?pageIndex=1 &pageSize=10&CategoryId=
        [HttpGet("public-paging/{languageId}")]
        public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] GetPublicProductPagingRequest request)
        {
            var products = await _publicProductService.GetAllByCategoryId(languageId, request);
            return Ok(products);

        }
        //http://localhost:port/product/1

        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _publicManageService.GetById(productId, languageId);
            if (product == null)
                return BadRequest("Cannot find product");
            return Ok(product);

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _publicManageService.CreateProduct(request);
            if (productId == 0)
                return BadRequest();
            var product = await _publicManageService.GetById(productId, request.LanguageId);
            return CreatedAction(nameof(GetById), new { id = productId }, product);

        }

        private IActionResult CreatedAction(string v, object p, ProductImageViewModel product)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _publicManageService.UpdateProduct(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var affectedResult = await _publicManageService.DeleteProduct(productId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
        /// <summary>
        /// Update 1 phan cua ban  ghi
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="newPrice"></param>
        /// <returns></returns>
        [HttpPatch("{productId}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            var isSuccessful = await _publicManageService.UpdatePrice(productId, newPrice);
            if (isSuccessful)
                return Ok();
            return BadRequest();

        }



        //Image

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageId = await _publicManageService.AddImage(productId,request);
            if (imageId == 0)
                return BadRequest();
            var image = await _publicManageService.GetImageById(imageId);
            return CreatedAction(nameof(GetImagebyId), new { id = imageId }, image);

        }
        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _publicManageService.UpdateImage(imageId, request);
            if (result == 0)
                return BadRequest();
            return Ok();

        }

        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _publicManageService.RemoveImage(imageId);
            if (result == 0)
                return BadRequest();
            return Ok();

        }

        [HttpGet("{productId}/images/{languageId}")]
        public async Task<IActionResult> GetImagebyId( int  imageId)
        {
            var image = await _publicManageService.GetImageById( imageId);
            if (image == null)
                return BadRequest("Cannot find product");
            return Ok(image);

        }
    }
}
