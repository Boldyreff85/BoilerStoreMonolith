

//DescriptionFeatureRepository

using System.Collections.Generic;
using System.Linq;
using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class DescriptionFeatureRepository : IDescriptionFeatureRepository
    {
        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<DescriptionFeature> DescriptionFeatures => context.DescriptionFeatures;

        public void SaveFeature(DescriptionFeature descriptionFeature)
        {
            if (descriptionFeature.Id == 0)
            {
                context.DescriptionFeatures.Add(descriptionFeature); 
            }
            else
            {
                DescriptionFeature dbEntry = context.DescriptionFeatures.Find(descriptionFeature.Id);
                if (dbEntry != null)
                {
                    dbEntry.Id = descriptionFeature.Id;
                    dbEntry.Name = descriptionFeature.Name;
                    dbEntry.Value = descriptionFeature.Value;
                    dbEntry.ProductId = descriptionFeature.ProductId;
                }
            }
            context.SaveChanges();
        }

        public DescriptionFeature DeleteDescriptionFeature(int descriptionFeatureId)
        {
            DescriptionFeature dbEntry = context.DescriptionFeatures.Find(descriptionFeatureId);
            if (dbEntry != null)
            {
                context.DescriptionFeatures.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public List<DescriptionFeature> DeleteDescriptionFeatures(List<DescriptionFeature> descriptionFeaturesToDelete)
        {
            var removedFeatures = new List<DescriptionFeature>();
            using (var context = new ApplicationContext())
            {
                foreach (var feature in descriptionFeaturesToDelete)
                {
                    DescriptionFeature dbEntry = context.DescriptionFeatures.Find(feature.Id);
                    if (dbEntry != null)
                    {
                        context.DescriptionFeatures.Remove(dbEntry);
                        context.SaveChanges();
                        removedFeatures.Add(dbEntry);
                    }
                }
            }
            return removedFeatures;
        }
    }
}
