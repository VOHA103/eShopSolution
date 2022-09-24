using eShopSolution.Application.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductImageViewModel = eShopSolution.Application.Catalog.ProductImages.ProductImageViewModel;

namespace eShopSolution.Application.Catalog
{
    public interface IManageProductService
    {
        Task<int> CreateProduct(ProductCreateRequest request);
        Task<int> UpdateProduct(ProductUpdateRequest request);
        Task<int> DeleteProduct(int productId);
        Task<bool> UpdatePrice(int productId,decimal newPrice);
        Task<bool> UpdateStock(int productId,int  addeQuantity);
        Task AddViewcount(int productId);
        Task<ProductImageViewModel> GetById(int productId,string languageId); 

        Task<PageResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request);

        Task<int> AddImage(int productId,ProductImageCreateRequest request);
        Task<int> RemoveImage(int ImageId);
        Task<int> UpdateImage( int ImageId,ProductImageUpdateRequest request);
        Task<List<ProductImageViewModel>> GetListImage(int productId);
        Task<ProductImageViewModel> GetImageById(int imageId);

        Task Update(ProductCreateRequest request);
        Task GetById(int id);
    }
}
