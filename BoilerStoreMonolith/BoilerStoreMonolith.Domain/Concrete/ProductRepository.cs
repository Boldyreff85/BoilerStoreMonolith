using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BoilerStoreMonolith.Domain.Concrete
{


    public class ProductRepository : IProductRepository
    {
        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<Product> Products
        {
            get { return context.Products; }
        }
        public void SaveProduct(Product product)
        {
            using (var context = new ApplicationContext())
            {
                if (product.ProductID == 0)
                {
                    context.Products.Add(product);
                }
                else
                {
                    Product dbEntry = context.Products.Find(product.ProductID);
                    if (dbEntry != null)
                    {
                        dbEntry.Title = product.Title;
                        dbEntry.Description = product.Description;
                        dbEntry.Price = product.Price;
                        dbEntry.Currency = product.Currency;
                        dbEntry.Category = product.Category;
                        dbEntry.Firm = product.Firm;
                        dbEntry.ImageData = product.ImageData;
                        dbEntry.ImageMimeType = product.ImageMimeType;
                    }
                }
                context.SaveChanges();
            }

        }


        public Product DeleteProduct(int ProductID)
        {
            Product dbEntry = context.Products.Find(ProductID);
            if (dbEntry != null)
            {
                context.Products.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public List<Product> DeleteProducts(List<Product> productsToDelete)
        {
            if (productsToDelete.Any())
            {
                context.Products.RemoveRange(productsToDelete);
                context.SaveChangesAsync();
            }
            return productsToDelete;
        }

    }
}
