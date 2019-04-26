using System.ComponentModel.DataAnnotations;

namespace BoilerStoreMonolith.Domain.Entities
{
    public class InfoEntity
    {
        [Key]
        public int InfoID { get; set; }
        public string CompanyInfo { get; set; }
        public string Services { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Schedule { get; set; }
        public string PhoneMain { get; set; }
        public string PhoneAdditional { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public byte[] ImageData2 { get; set; }
        public string ImageMimeType2 { get; set; }

    }
}
