using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace JonkerBudgetCore.Api.Auth
{
    public class ValidationResult
    {
        public readonly ClaimsIdentity Identity = null;
        public readonly bool IsValid = false;
        public readonly string ValidationError = "";

        public ValidationResult(ClaimsIdentity identity)
        {
            this.Identity = identity;
            this.IsValid = true;
            this.ValidationError = "";
        }

        public ValidationResult(string validationError)
        {
            this.IsValid = false;
            this.ValidationError = validationError;
        }
    }
}
