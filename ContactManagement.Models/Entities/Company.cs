using System.ComponentModel.DataAnnotations;

namespace ContactManagement.Models.Entities
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public ICollection<Contact>? Contacts { get; set; }

    }
}
