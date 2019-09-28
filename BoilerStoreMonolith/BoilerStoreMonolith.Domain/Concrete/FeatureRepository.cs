using System.Collections.Generic;
using System.Linq;
using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class ProductFeatureRepository : IProductFeatureRepository
    {

        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<ProductFeature> Features => context.ProductFeatures;

        public void SaveFeature(ProductFeature feature)
        {
            if (feature.Id == 0)
            {
                context.ProductFeatures.Add(feature);
            }
            else
            {
                ProductFeature dbEntry = context.ProductFeatures.Find(feature.Id);
                if (dbEntry != null)
                {
                    dbEntry.Id = feature.Id;
                    dbEntry.Name = feature.Name;
                    dbEntry.Value = feature.Value;
                    dbEntry.Unit = feature.Unit;
                }
            }
            context.SaveChanges();
        }

        public ProductFeature DeleteFeature(int featureId)
        {
            ProductFeature dbEntry = context.ProductFeatures.Find(featureId);
            if (dbEntry != null)
            {
                context.ProductFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public List<ProductFeature> DeleteFeatures(List<ProductFeature> featuresToDelete)
        {
            var removedFeatures = new List<ProductFeature>();
            using (var context = new ApplicationContext())
            {
                foreach (var feature in featuresToDelete)
                {
                    ProductFeature dbEntry = context.ProductFeatures.Find(feature.Id);
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
