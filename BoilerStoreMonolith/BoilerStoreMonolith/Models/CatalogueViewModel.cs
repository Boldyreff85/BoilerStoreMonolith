using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class CatalogueViewModel
    {
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Firms { get; set; }
        public ProductListViewModel ProductList { get; set; }
        public List<string> CategoryFeatures { get; set; }
        public List<FeatureRange> FeatureRanges { get; set; }
    }

    public class FeatureRange
    {
        public string FeatureName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Unit { get; set; }
    }
}