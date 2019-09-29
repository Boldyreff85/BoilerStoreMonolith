using System.Collections.Generic;
using System.Linq;
using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class FeatureRepository : IFeatureRepository
    {

        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<Feature> Features => context.Features;

        public void SaveFeature(Feature feature)
        {
            // делаем предварительную обрезку пустых символов
            feature.Name = feature.Name.Trim();
            // тут добавим проверку на уже имеющееся имя
            if (feature.Id == 0 && !context.Features.Any(f => f.Name == feature.Name))
            {
                context.Features.Add(feature);
            }
            else
            {
                Feature dbEntry = context.Features.Find(feature.Id);
                if (dbEntry != null)
                {
                    dbEntry.Id = feature.Id;
                    dbEntry.Name = feature.Name;
                }
            }
            context.SaveChanges();
        }

        public Feature DeleteFeature(int featureId)
        {
            Feature dbEntry = context.Features.Find(featureId);
            if (dbEntry != null)
            {
                context.Features.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public List<Feature> DeleteFeatures(List<Feature> featuresToDelete)
        {
            var removedFeatures = new List<Feature>();
            using (var context = new ApplicationContext())
            {
                foreach (var feature in featuresToDelete)
                {
                    Feature dbEntry = context.Features.Find(feature.Id);
                    if (dbEntry != null)
                    {
                        context.Features.Remove(dbEntry);
                        context.SaveChanges();
                        removedFeatures.Add(dbEntry);
                    }
                }
            }
            return removedFeatures;
        }
    }
}
