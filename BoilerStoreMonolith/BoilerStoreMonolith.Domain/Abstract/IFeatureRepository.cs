﻿using System.Collections.Generic;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface IFeatureRepository
    {
        IEnumerable<Feature> Features { get; }
        Feature DeleteFeature(int featureId);
        List<Feature> DeleteFeatures(List<Feature> featuresToDelete);
        void SaveFeature(Feature feature);
    }
}