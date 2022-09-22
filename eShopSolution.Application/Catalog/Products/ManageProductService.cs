using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Dtos;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products.Dtos
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbConText _context;
        public ManageProductService(EShopDbConText context)
        {
            _context = context;
        }

        public async Task AddViewcount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
             await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
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
            _context.Products.Add(product);
          return  await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new eShopException($"Cannot find a product:{productId}");
            _context.Products.Remove(product);
           return await _context.SaveChangesAsync();
        }

        public async Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request)
        {
            //1 Select Join
            var query = from p in Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            //2filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));

            if (request.Category.Count > 0)
            {
                query = query.Where(prop => request.CategoryIds.Contains(prop.pic.CategoryId));


            }
            //3/ Paging
            int totalRow = await query.CountAsyns();
            var data = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)
                .Take(request.PageSize)
                .Select(x=>new ProductViewModel()
                {
                    Id = x.Id,
                    Name=x.pt.Name,
                    DateCreated=x.p.DateCreated,
                    Description=x.pt.Description,
                    Details=x.pt.Details,
                    Languageaid=x.pt.LanguageId,
                    OriginalPrice=x.p.OriginalPrie,
                    Price=x.p.Price,
                    SeoDescription=x.p.SeoDescription,
                    SeoTitle=x.pt.SeoTile,
                    Stock=x.p.Stock,
                    ViewCount=x.p.ViewCount


                }).ToListAsync();


            //4 select and projection
            var pagedResult = new PageResult<ProductViewModel>()
            {
                TotaRecord = totalRow,
                Items = data
            };

        }

        private void ToListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStock(int productId, decimal addeQuantity)
        {
            throw new NotImplementedException();
        }

        public Task<PageResult<ProductViewModel>> GetAllPaging(string keyword, string pageIndex, string pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
