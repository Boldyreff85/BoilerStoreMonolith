using System.ComponentModel.DataAnnotations;

namespace BoilerStoreMonolith.Domain.Entities
{
    public class CategorySpec
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите название Характеристики")]
        [Display(Name = "Характеристика")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите значение")]
        [Display(Name = "Значение")]
        public string Value { get; set; }
    }
}
