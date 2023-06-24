using System.ComponentModel.DataAnnotations;

namespace AugularAuthAPI.Models
{
    public class ConsumerRegistrationUsers
    {
        [Key]
        public int ConsumerRegistrationId { get; set; }
        public string ConsumerFullName { get; set; }
        public string ConsumerContractAccountNumber { get; set; }
        public int ConsumerMobileNumber { get; set; }
        public string ConsumerAddress { get; set; }
        public string ConsumerPassword { get; set; }
        public string CousumerToken { get; set; }
        public string ConsumerRole { get; set; }
    }
}
