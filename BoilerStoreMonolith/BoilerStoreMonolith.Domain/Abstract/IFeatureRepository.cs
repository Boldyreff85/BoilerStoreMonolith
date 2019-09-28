using System.Collections.Generic;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface IProductFeatureRepository
    {
        IEnumerable<ProductFeature> Features { get; }
        void SaveFeature(ProductFeature feature);
        ProductFeature DeleteFeature(int featureId);
        List<ProductFeature> DeleteFeatures(List<ProductFeature> featuresToDelete);
    }
}