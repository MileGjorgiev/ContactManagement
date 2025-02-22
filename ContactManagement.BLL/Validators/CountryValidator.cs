using ContactManagement.Models.Entities;
using FluentValidation;

namespace ContactManagement.BLL.Validators
{
    public class CountryValidator : AbstractValidator<Country>
    {
        public CountryValidator()
        {
            RuleFor(c => c.CountryName)
                .NotEmpty().WithMessage("Country name is required.")
                .Length(3, 100).WithMessage("Country name must be between 3 and 100 characters.");
        }
    }
}
