using System.ComponentModel.DataAnnotations;

namespace BoilerStoreMonolith.Domain.Entities
{
    public class InfoEntity
    {
        [Key]
        public int InfoID { get; set; }
        [Display(Name = "Информация о компании")]
        public string CompanyInfo { get; set; }
        [Display(Name = "Услуги")]
        public string Services { get; set; }
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "Расписание")]
        public string Schedule { get; set; }
        [Display(Name = "Основной телефон")]
        public string PhoneMain { get; set; }
        [Display(Name = "Дополнительный телефон")]
        public string PhoneAdditional { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public byte[] ImageData2 { get; set; }
        public string ImageMimeType2 { get; set; }

        // email settings fields
        public string Email { get; set; }
        // в случае с gmail тут указывается пароль приложений или app specific password
        // получить такой пароль можно в настройках аутентификации аккаунта google
        public string Password { get; set; } 
        public string Host { get; set; }
        public int Port { get; set; }
        public bool doUseSsl { get; set; }

    }
}
