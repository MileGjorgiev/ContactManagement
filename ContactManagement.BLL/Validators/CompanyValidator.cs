using ContactManagement.Models.Entities;
using FluentValidation;

namespace ContactManagement.BLL.Validators
{
    public class CompanyValidator : AbstractValidator<Company>
    {
        public CompanyValidator()
        {
            RuleFor(c => c.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .Length(3, 100).WithMessage("Company name must be between 3 and 100 characters.");
        }
    }
}
