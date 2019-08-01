using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface ICategoryFeatureRepository
    {
        IEnumerable<CategoryFeature> CategoryFeatures { get; }
        void SaveCategoryFeature(CategoryFeature categoryFeature);
        Task<CategoryFeature> DeleteCategoryFeature(int categoryFeatureId);
        List<CategoryFeature> DeleteCategoryFeatures(List<CategoryFeature> categoryFeaturesToDelete);
    }
}