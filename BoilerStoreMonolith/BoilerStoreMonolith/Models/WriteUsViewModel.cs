using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BoilerStoreMonolith.Models
{
    public class WriteUsViewModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Введите сообщение")]
        public string Message { get; set; }
        [Required(ErrorMessage = "Введите адрес электронной почты")]
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}