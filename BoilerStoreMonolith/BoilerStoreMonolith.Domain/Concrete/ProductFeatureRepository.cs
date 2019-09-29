using System.Collections.Generic;
using System.Linq;
using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class ProductFeatureRepository : IProductFeatureRepository
    {

        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<ProductFeature> ProductFeatures => context.ProductFeatures;


        public void SaveFeature(ProductFeature productFeature)
        {
            if (productFeature.Id == 0)
            {
                context.ProductFeatures.Add(productFeature);
            }
            else
            {
                ProductFeature dbEntry = context.ProductFeatures.Find(productFeature.Id);
                if (dbEntry != null)
                {
                    dbEntry.Id = productFeature.Id;
                    dbEntry.Name = productFeature.Name;
                    dbEntry.Value = productFeature.Value;
                    dbEntry.Unit = productFeature.Unit;
                }
            }
            context.SaveChanges();
        }

        public ProductFeature DeleteFeature(int productFeatureId)
        {
            ProductFeature dbEntry = context.ProductFeatures.Find(productFeatureId);
            if (dbEntry != null)
            {
                context.ProductFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public List<ProductFeature> DeleteFeatures(List<ProductFeature> productFeaturesToDelete)
        {
            var removedFeatures = new List<ProductFeature>();
            using (var context = new ApplicationContext())
            {
                foreach (var productFeature in productFeaturesToDelete)
                {
                    ProductFeature dbEntry = context.ProductFeatures.Find(productFeature.Id);
                    if (dbEntry != null)
                    {
                        context.ProductFeatures.Remove(dbEntry);
                        context.SaveChanges();
                        removedFeatures.Add(dbEntry);
                    }
                }
            }
            return removedFeatures;
        }
    }
}
