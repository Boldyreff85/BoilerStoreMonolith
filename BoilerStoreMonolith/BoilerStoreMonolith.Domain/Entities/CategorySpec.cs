using System.ComponentModel.DataAnnotations;

namespace BoilerStoreMonolith.Domain.Entities
{
    public class CategorySpec
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите название Характеристики")]
        [Display(Name = "Характеристика")]
        public string Name { get; set; }

    }
}
