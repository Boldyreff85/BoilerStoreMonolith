using System.Collections.Generic;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface IDescriptionFeatureRepository
    {
        IEnumerable<DescriptionFeature> DescriptionFeatures { get; }
        void SaveFeature(DescriptionFeature descriptionFeature);
        DescriptionFeature DeleteDescriptionFeature(int descriptionFeatureId);
        List<DescriptionFeature> DeleteDescriptionFeatures(List<DescriptionFeature> descriptionFeaturesToDelete);
    }
}