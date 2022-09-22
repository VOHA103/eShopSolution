using eShopSolution.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Application.Catalog.Products.Dtos
{
    public class GetProductPagingRequest:PagingRequestBase
    {

        public string Keyword { get; set; }
        public List<int> Category { get; set; }



    }
}
