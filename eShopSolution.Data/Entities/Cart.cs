using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DateCreated { get; set; }
        public Guid UserId { get; set; }
        public Product Product { get; set; }
        public DateTime DateCreat { get; set; }
        public AppUser AppUser { get; set; }

    }
}
