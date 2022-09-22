﻿using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Application.Catalog.Products
{
   public interface  IPublicProductService
    {
        PageResult<ProductViewModel> GetAllByCategoryId(GetProductPagingRequest request);

    }
}
