using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BoilerStoreMonolith.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите название категории")]
        [Display(Name = "Категория")]
        public string Name { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public ICollection<CategorySpec> CategorySpecs { get; set; }
        public ICollection<Feature> Features { get; set; }
    }
}
