using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;
using System.Web;

namespace BoilerStoreMonolith.Models
{
    public class AdminEditViewModel
    {
        public Product Product { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Firms { get; set; }
        public List<Feature> Features { get; set; }
    }

    public class AdminIndexListViewModel
    {
        public List<Product> Products { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Firms { get; set; }
    }



    // categories view models

    public class EditCategoriesViewModel
    {
        public Category Category { get; set; }
        public List<string> CategoryFeaturesNames { get; set; }
        public string ImageToLoad { get; set; }
    }



    // firms view models


    public class IndexFirmsViewModel_
    {
        public List<Firm> Firms { get; set; }
        public List<string> firmNames { get; set; }
        public List<HttpPostedFileBase> firmImgs { get; set; }
    }

    public class IndexFirmsViewModel
    {
        public List<Firm> Firms { get; set; }
        public string NewFirmName { get; set; }
    }
}