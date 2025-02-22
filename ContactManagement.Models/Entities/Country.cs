namespace ContactManagement.Models.Entities
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public ICollection<Contact>? Contacts { get; set; }

        public Dictionary<string, int> GetCompanyStatisticsByCountryId(int countryId)
        {
            var statistics = Contacts?
                .Where(c => c.CountryId == countryId)
                .GroupBy(c => c.Company.CompanyName)
                .ToDictionary(g => g.Key, g => g.Count());

            return statistics ?? new Dictionary<string, int>();
        }
    }
}
