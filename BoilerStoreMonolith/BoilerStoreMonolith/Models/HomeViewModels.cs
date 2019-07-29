using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class IndexViewModel
    {
        public List<Category> Categories { get; set; }
        public InfoEntity infoEntity { get; set; }
    }
}