using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Interfaces
{
    public interface IValidationService
    {
        bool Validate<T>(Func<T, bool> validator, T value);
        bool ValidatePESEL(string pesel);
        bool ValidateBirthDate(DateTime? date);
    }
}
