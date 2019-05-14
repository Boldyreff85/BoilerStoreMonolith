using BoilerStoreMonolith.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoilerStoreMonolith.Models
{
    public class AdminEditViewModel
    {
        public Product Product { get; set; }
        public string ImageToLoad { get; set; }
    }
}