using eShopSolution.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Application.Catalog.Products.Dtos.Public
{
    public class GetProductPagingRequest:PagingRequestBase
    {

        public string Keyword { get; set; }
        public int? CategoryId { get; set; }
        public List<int> CategoryIds { set; get; }
    }
}
