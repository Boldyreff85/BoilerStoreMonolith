using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class CatalogueTreeViewModel
    {
        public string CurrCategory { get; set; }
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    }

    public class CategoryModel
    {
        public string Name { get; set; }
        public List<string> Firms { get; set; }
    }

}