using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class CatalogueModelView
    {
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Firms { get; set; }
        public ProductListViewModel ProductList { get; set; }
    }
}