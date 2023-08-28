using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using University.Interfaces;
using University.Extensions;

namespace University.Services
{
    public class ValidationService : IValidationService
    { 
        public bool Validate<T>(Func<T, bool> validator, T value)
        {
            return validator.Invoke(value);
        }

        public bool ValidatePESEL(string pesel)
        {
            Func<string, bool> validator = StringExtensions.IsValidPESEL;
            return Validate(validator, pesel);
        }

        public bool ValidateBirthDate(DateTime? birthDate)
        {
            Func<DateTime?, bool> validator = date => date is not null && date < DateTime.Now;
            return Validate(validator, birthDate);
        }
    }
}
