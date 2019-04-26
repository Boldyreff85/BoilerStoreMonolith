using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);
        Product DeleteProduct(int ProductID);
    }
}


