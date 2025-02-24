using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContactManagement.Models.Entities
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Contact>? Contacts { get; set; }
    }
}
