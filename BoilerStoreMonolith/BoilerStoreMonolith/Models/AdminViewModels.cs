using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;
using System.Web;

namespace BoilerStoreMonolith.Models
{
    public class AdminEditViewModel
    {
        public Product Product { get; set; }
        public string ImageToLoad { get; set; }
        public Category Category { get; set; }
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

    // categories view models

    public class EditCategoriesViewModel
    {
        public Category Category { get; set; }
        public List<string> Specs { get; set; }
        public string ImageToLoad { get; set; }
    }



    // firms view models


    public class IndexFirmsViewModel
    {
        public List<Firm> Firms { get; set; }
        public List<string> firmName { get; set; }
        public List<HttpPostedFileBase> firmImg { get; set; }
    }


}