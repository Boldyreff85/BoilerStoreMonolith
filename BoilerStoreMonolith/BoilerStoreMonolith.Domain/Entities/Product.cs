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
        [Required(ErrorMessage = "Введите категорию")]
        [Display(Name = "Категория")]
        public string Category { get; set; }
        [Display(Name = "Картинка категории")]
        public byte[] CategoryImageData { get; set; }
        public string CategoryImageMimeType { get; set; }
        [Required(ErrorMessage = "Введите производителя")]
        [Display(Name = "Производитель")]
        public string Firm { get; set; }
        public byte[] FirmImageData { get; set; }
        public string FirmImageMimeType { get; set; }
        [Required(ErrorMessage = "Введите мощность")]
        [Display(Name = "Мощность")]
        public string Power { get; set; }
        [Required(ErrorMessage = "Укажите цену")]
        [Display(Name = "Цена")]
        public string Price { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}
