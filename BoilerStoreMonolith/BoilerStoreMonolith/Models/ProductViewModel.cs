using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{

    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }

}