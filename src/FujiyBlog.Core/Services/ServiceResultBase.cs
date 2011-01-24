using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FujiyBlog.Core.Services
{
    public abstract class ServiceResultBase
    {
        protected ServiceResultBase(IEnumerable<ValidationResult> validationResults)
        {
            if (validationResults == null)
            {
                throw new ArgumentNullException("validationResults");
            }

            RuleViolations = validationResults;
        }

        public IEnumerable<ValidationResult> RuleViolations
        {
            get;
            private set;
        }
    }
}
