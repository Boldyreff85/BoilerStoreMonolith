using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
