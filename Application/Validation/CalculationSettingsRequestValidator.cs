using Application.DTOs.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class CalculationSettingsRequestValidator : AbstractValidator<CalculationSettingsRequest>
    {
        public CalculationSettingsRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).Length(0,300);
            RuleFor(x => x.WorseningSettings).NotEmpty().Must(x => x.Count > 0);
            RuleFor(x => x.CountOfImplementations).NotNull().GreaterThan(0);
            RuleFor(x => x.SechNumber).NotNull().GreaterThan(0);
            RuleFor(x => x.PercentForWorsening).NotNull().GreaterThan(0);
        }
    }
}
