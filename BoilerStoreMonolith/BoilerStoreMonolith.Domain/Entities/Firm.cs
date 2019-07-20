using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerStoreMonolith.Domain.Entities
{
    public class Firm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите название производителя")]
        [Display(Name = "Производитель")]
        public string Name { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}
