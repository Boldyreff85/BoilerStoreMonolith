using System.Collections.Generic;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface IProductFeatureRepository
    {
        IEnumerable<ProductFeature> ProductFeatures { get; }
        void SaveFeature(ProductFeature productFeature);
        ProductFeature DeleteFeature(int productFeatureId);
        List<ProductFeature> DeleteFeatures(List<ProductFeature> productFeaturesToDelete);
    }
}