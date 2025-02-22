using ContactManagement.Models.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagement.BLL.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(c => c.ContactName)
                .NotEmpty().WithMessage("Contact name is required.")
                .Length(3, 100).WithMessage("Contact name must be between 3 and 100 characters.");
            RuleFor(c => c.CompanyId)
                .NotEmpty().WithMessage("CompanyId cannot be empty");
            RuleFor(c => c.CountryId)
                .NotEmpty().WithMessage("CountryId cannot be empty");
        }
    }
}
