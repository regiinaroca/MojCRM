
namespace MojCRM.Helpers
{
    public class CreateContact
    {
        public int OrganizationId { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Title { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Email { get; set; }
        public string User { get; set; }
        public int DoNotCall { get; set; }
        public int ContactType { get; set; }
    }
}