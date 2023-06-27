using System.ComponentModel.DataAnnotations;

namespace AugularAuthAPI.Models
{
    public class ConsumerRegistrationUsers
    {
        [Key]
        public int ConsumerRegistrationId { get; set; }
        public string? ConsumerFullName { get; set; }
        public string? ConsumerContractAccountNumber { get; set; }
        public string? ConsumerMeterNumber { get; set; }
        public string? ConsumerMobileNumber { get; set; }
        public string? ConsumerAddress { get; set; }
        public string? ConsumerPassword { get; set; }
        public string? CousumerToken { get; set; }
        public string? ConsumerRole { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
