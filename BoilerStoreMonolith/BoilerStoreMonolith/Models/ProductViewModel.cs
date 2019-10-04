using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{

    public class ProductListViewModel
    {
        public IEnumerable<ProductWithFeatures> ProductWithFeaturesList { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }

    public class ProductWithFeatures
    {
        public Product Product { get; set; }
        public List<ProductFeature> ProductFeatures { get; set; }
        public List<DescriptionFeature> DescriptionFeatures { get; set; }
    }
}