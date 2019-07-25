using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class AdminEditViewModel
    {
        public Product Product { get; set; }
        public string ImageToLoad { get; set; }
        public NewCategoryViewModel NewCategory { get; set; }
    }

    public class NewCategoryViewModel
    {
        public string Name { get; set; }
        public List<string> Features { get; set; }
        public string ImageToLoad { get; set; }
    }

    public class AdminIndexListViewModel
    {
        public List<AdminIndexViewModel> IndexList { get; set; }
    }

    public class AdminIndexViewModel
    {
        public int ProductID { get; set; }
        public string Title { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public bool IsDelete { get; set; }
    }
}