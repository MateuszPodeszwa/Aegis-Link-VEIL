using System.ComponentModel.DataAnnotations;

namespace AegisLink.Server.Data
{
    public class UserKey
    {
        [Key]
        [MaxLength(8)]
        public string AegisId { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string PublicKey { get; set; } = string.Empty;

    }
}
