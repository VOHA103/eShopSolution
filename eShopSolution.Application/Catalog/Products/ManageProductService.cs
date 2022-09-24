using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Catalog.Products;
using System.Net.Http.Headers;
using eShopSolution.Application.Common;
using eShopSolution.Data;
using eShopSolution.Application.Catalog.ProductImages;
using ProductImageViewModel = eShopSolution.Application.Catalog.ProductImages.ProductImageViewModel;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbConText _context;
        private readonly IStorageService _storageService;

        public object Da { get; private set; }

        public ManageProductService(EShopDbConText context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
      

        public async Task AddViewcount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
             await _context.SaveChangesAsync();
        }

        public async Task<int> CreateProduct(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    {
                        Name=request.Name,
                        Description=request.Description,
                        Details=request.Details,
                        SeoDescription=request.SeoDescription,
                        SeoAlias=request.SeoAlias,
                        SeoTitle=request.SeoTitle,
                        LanguageId=request.LanguageId,
                    }
                }

            };
            //save image
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption="Thumbnail image",
                        DateCreated=DateTime.Now,
                        FileSize=request.ThumbnailImage.Length,
                        ImagePath=await this.SaveFile(request.ThumbnailImage),
                        IsDefault=true,
                        SortOrder=1



                        }
                 };
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<int> DeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException($"Cannot find a product:{productId}");
            var images = _context.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }
            _context.Products.Remove(product);

            return await _context.SaveChangesAsync();
        }

      
        public async Task<PageResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {
            //1 Select Join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            //2filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));

            if (request.CategoryIds.Count > 0)
            {
                query = query.Where(p => request.CategoryIds.Contains(p.pic.CategoryId));
            }
            //3/ Paging
            int totalRow = await query.CountAsync();
            var data =await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)
                .Take(request.PageSize)
                .Select(x=>new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name=x.pt.Name,
                    DateCreated=x.p.DateCreated,
                    Description=x.pt.Description,
                    Details=x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice=x.p.OriginalPrice,
                    Price=x.p.Price,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle=x.pt.SeoTitle,
                    Stock=x.p.Stock,
                    ViewCount=x.p.ViewCount


                }).ToListAsync();


            //4 select and projection
            var pagedResult = new PageResult<ProductViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };
            return pagedResult;
        }

        private void ToListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateProduct(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id
              && x.LanguageId == request.LanguageId);

            if (product == null || productTranslation == null) throw new eShopException($"Cannot find a product id:{request.Id}");

            productTranslation.Name = request.Name;
            productTranslation.SeoAlias = request.SeoAlias;
            productTranslation.SeoDescription = request.SeoDescription;
            productTranslation.SeoTitle = request.SeoTitle;
            productTranslation.Description = request.Description;
            productTranslation.Details = request.Details;
            //save image
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
            }
            return await _context.SaveChangesAsync();

        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if(product==null) throw new eShopException($"Cannot find a product id:{productId}");

            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> UpdateStock(int productId, int addeQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new eShopException($"Cannot find a product id:{productId}");
            product.Stock += addeQuantity;
            return await _context.SaveChangesAsync() > 0;

        }

      private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public async Task<ProductViewModel> GetById(int productId, string languageId)
        {
            var product = await _context.Products.FindAsync(productId);
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == languageId);

            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation != null ? productTranslation.Description : null,

                LanguageId = productTranslation.LanguageId,
                Details = productTranslation != null ? productTranslation.Details : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                Stock = product.Stock,
                ViewCount = product.ViewCount

            };
            return productViewModel;
        }

        Task<ProductImages.ProductImageViewModel> IManageProductService.GetById(int productId, string languageId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
           var productImage =new  ProductImage(){
               Caption=request.Caption,
               DateCreated=DateTime.Now,
               IsDefault=request.IsDefault,
               ProductId=productId,
               SortOrder=request.SortOrder
            };
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length; 
            }

            _context.ProductImages.Add(productImage);

             await _context.SaveChangesAsync();
            return productImage.Id;
        }

 


        public async Task<int> RemoveImage( int ImageId)
        {
            var productImage = await _context.ProductImages.FindAsync(ImageId);
            if (productImage == null)
                throw new Exception($"Cannot find an image with id{ ImageId}");
            _context.ProductImages.Remove(productImage);
            return await _context.SaveChangesAsync();

        }

        public async Task<int> UpdateImage( int ImageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(ImageId);
            if (productImage == null)
                throw new Exception($"Cannot find an image with id {ImageId}");

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }

            _context.ProductImages.Update(productImage);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<ProductImages.ProductImageViewModel>> GetListImage(int productId)
        {
            return await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(i => new ProductImageViewModel()
                {
                    Caption=i.Caption,
                    DateCreated=DateTime.Now,
                    FileSize=i.FileSize,
                    Id=i.Id,
                    ImagePath=i.ImagePath,
                    IsDefault=i.IsDefault,
                    ProductId=i.ProductId,
                    SortOrder=i.SortOrder
                }).ToListAsync();
        }

        public Task Update(ProductCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                throw new Exception($"Cannot find an image with id {imageId}");
              var viewModel= new ProductImageViewModel()
               {
                   Caption = image.Caption,
                   DateCreated = DateTime.Now,
                   FileSize = image.FileSize,
                   Id = image.Id,
                   ImagePath = image.ImagePath,
                   IsDefault = image.IsDefault,
                   ProductId = image.ProductId,
                   SortOrder = image.SortOrder
               };
            return viewModel;
        }
    }
}
