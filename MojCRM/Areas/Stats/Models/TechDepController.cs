using System.ComponentModel.DataAnnotations;

namespace MojCRM.Areas.Stats.Models
{
    public class TechDepController
    {

        [Key]
        public int id { get; set; }
        public string Agent { get; set; }
        public int TechIncomingCall { get; set; }
        public int TechOutgoingCall { get; set; }
        public int TechCallDuration { get; set; }
        public int TechCompaniesActivated { get; set; }
        public int TechEmialCustomers { get; set; }
        public int TechEmialIntegrators { get; set; }
    }
}