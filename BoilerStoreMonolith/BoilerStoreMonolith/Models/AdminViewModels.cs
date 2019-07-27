﻿using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Models
{
    public class AdminEditViewModel
    {
        public Product Product { get; set; }
        public string ImageToLoad { get; set; }
        public EditCategoriesViewModel NewCategory { get; set; }
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

    public class AdminIndexCategoriesListViewModel
    {
        public List<AdminIndexCategoryViewModel> IndexList { get; set; }
        public string ImageToLoad { get; set; }
    }

    public class AdminIndexCategoryViewModel
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public bool IsDelete { get; set; }
    }
}