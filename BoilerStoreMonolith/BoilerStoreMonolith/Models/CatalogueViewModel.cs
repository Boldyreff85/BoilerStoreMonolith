using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class CatalogueViewModel
    {
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Firms { get; set; }
        public ProductListViewModel ProductList { get; set; }
        public List<string> CategoryFeatures { get; set; }
    }
}