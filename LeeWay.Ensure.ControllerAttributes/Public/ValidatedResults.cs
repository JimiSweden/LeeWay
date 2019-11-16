using System.Collections.Generic;
using System.Linq;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    /// <summary>
    /// Contains all the validation results with messages for print
    /// </summary>
    public sealed class ValidatedResults
    {
        private List<ValidationResult> ValidationResults;

        public List<ValidationResult> All => ValidationResults.OrderBy(result => result.Message).ToList();
        public List<ValidationResult> SuccessfulResults
            => ValidationResults.Where(message => message.IsValid).OrderBy(result => result.Message).ToList();
        public List<ValidationResult> FailedResults
            => ValidationResults.Where(message => message.IsInValid).OrderBy(result => result.Message).ToList();

        public ValidatedResults(List<ValidationResult> validationResults)
        {
            ValidationResults = validationResults;
        }
    }
}