using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BoilerStoreMonolith.Domain.Entities
{
    public class Product
    {
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Введите наименование товара")]
        [Display(Name = "Наименование")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Введите описание товара")]
        [Display(Name = "Описание")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Выберите производителя")]
        [Display(Name = "Производитель")]
        public string Firm { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

    }
}
