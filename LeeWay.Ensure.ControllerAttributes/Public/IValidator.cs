using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    /// <summary>
    /// This is the validator, It is used from the ValidatorBuilder
    /// <br/>
    /// Validator has methods for validating and throw on error, get all results, print all results,
    /// and an option to exclude actions from validation
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Rules created by the user 
        /// </summary>
        List<IValidationRule> ValidationRulesConfigured { get; set; }

        Attribute DefaultAuthorizeAttribute { get; set; }

        /// <summary>
        /// Excludes actions from validation; this must be done before executing validations
        /// </summary>
        /// <param name="actionsToExclude"></param>
        void ExcludeActionsFromValidation(IEnumerable<MethodInfo> actionsToExclude);

        /// <summary>
        /// Prints all validated results to the test output window, no matter if validation is successful or not.
        /// <br/>
        /// Usage: in your test class, set ITestOutputHelper as a dependency in the constructor and xUnit will wire it up for you, then pass it here
        /// </summary>
        /// <param name="outputHelper"></param>
        void PrintValidatedResults(ITestOutputHelper outputHelper);

        /// <summary>
        /// Returns the validated results for printing or whatever you like to do with it,
        /// use this instead of ValidateAndThrowOnError to get all results instead of throwing on fail.
        /// <br/>
        /// Tip: use ITestOutputHelper from xUnit to write to the test window
        /// </summary>
        /// <returns></returns>
        ValidatedResults ValidatedResults();

        /// <summary>
        /// Validates all rules and throws XunitException on error, prints all the failed results in the test window.
        /// <br/>
        /// If you want more control use <seealso cref="PrintValidatedResults(ITestOutputHelper)"/> to print all results
        /// <br/>
        /// or use <seealso cref="ValidatedResults"/> to get all validation results and do what you like
        /// </summary>
        void ValidateAndThrowOnError();
    }
}