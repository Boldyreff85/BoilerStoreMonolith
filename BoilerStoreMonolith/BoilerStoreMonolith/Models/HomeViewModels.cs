using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class IndexViewModel
    {
        public IEnumerable<string> categories { get; set; }
        public InfoEntity infoEntity { get; set; }
    }
}