using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using LeeWay.Ensure.ControllerAttributes.Public;
using Microsoft.AspNetCore.Authorization;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LeeWay.Ensure.ControllerAttributes.Internal
{
    /// <summary>
    /// This is the validator, It is used from the ValidatorBuilder
    /// </summary>
    internal sealed class Validator : IValidator
    {
        /// <summary>
        /// Rules created by the user 
        /// </summary>
        public List<IValidationRule> ValidationRulesConfigured { get; set; } = new List<IValidationRule>();
        
        /// <summary>
        /// only used as helper in error message, probably to be removed later
        /// </summary>
        private readonly string _assemblyName;

        /// <summary>
        /// Actions from Controllers in Assembly
        /// (by default all, but can actions can be removed by filter in builder)
        /// </summary>
        private IEnumerable<MethodInfo> _actionsToValidate;

        
        public Attribute DefaultAuthorizeAttribute { get; set; }
        
        private ValidatedResults _validatedResults;
        private IValidationRulesContainer AuthorizationValidationRules;
        
        public Validator(IEnumerable<MethodInfo> actionsToValidate, string assemblyName)
        {
            _actionsToValidate = actionsToValidate;
            _assemblyName = assemblyName;
        }

        public void ExcludeActionsFromValidation(IEnumerable<MethodInfo> actionsToExclude)
        {
            var allActions = _actionsToValidate.ToList();
            _actionsToValidate = allActions.Except(actionsToExclude);
        }
        
        public void PrintValidatedResults(ITestOutputHelper outputHelper)
        {
            var results = ValidatedResults();
            outputHelper.WriteLine($"There are {results.All.Count} controller actions validated; of which {results.SuccessfulResults.Count} passed, and {results.FailedResults.Count} failed.");
            outputHelper.WriteLine("");
            outputHelper.WriteLine($"Assembly under test: {_assemblyName}");
            outputHelper.WriteLine("Your configurations default settings are:");
            outputHelper.WriteLine($" - Default AuthorizeAttribute: {DisplayMessageDefaultAuthorizeAttribute}");
            outputHelper.WriteLine($" - Default Policy name: {DisplayMessageDefaultAuthorizationPolicy()}");
            outputHelper.WriteLine("");
            
            results.All.ForEach(message => outputHelper.WriteLine($"{(message.IsValid ? "Passed" : "Failed")} : {message.Message}"));
            outputHelper.WriteLine("- the End - ");
        }

        public ValidatedResults ValidatedResults()
        {
            if (_validatedResults == null)
            {
                PrepareForTest();
                ValidateConfiguration();

                _validatedResults = new ValidatedResults(All_actions_validation_results());
            }

            return _validatedResults;
        }

        public void ValidateAndThrowOnError()
        {
            var validationResults = ValidatedResults();

            if (validationResults.FailedResults.Any())
            {
                var failedBecauseMessage = new StringBuilder();
                failedBecauseMessage.AppendLine($"Oops, sorry to bother you my friend but we don't like errors and found {validationResults.FailedResults.Count} to many")
                    .AppendLine("")
                    //todo.. attribute to display message , i e if policy set, display [Authorize (Policy = "name")]
                    // note: as in ValidationResultFactory.AuthorizeAttributeAndPolicyNameForDisplay()
                    //but: first fix.. needs to check if it is allowAnonymous first
                    //then, do the same in PrintValidatedResults
                    .AppendLine($"Assembly under test: {_assemblyName}")
                    .AppendLine("Your configurations default settings are:")
                    .AppendLine($" - Default AuthorizeAttribute: {DisplayMessageDefaultAuthorizeAttribute}")
                    .AppendLine($" - Default Policy name: {DisplayMessageDefaultAuthorizationPolicy()}")
                    .AppendLine("")
                    .AppendLine($" --- {validationResults.FailedResults.Count} validation errors described below --- ");
                
                //add all failed validation result messages
                var failedResultMessages = validationResults.FailedResults
                    .Select((result, index) => CreateNumberedMessage(index, result)).ToList();

                failedResultMessages.ForEach(message => failedBecauseMessage.AppendLine(message));

                failedBecauseMessage.AppendLine(" --- end of validation errors --- ");

                throw new XunitException($"{failedBecauseMessage}");
            }

            string CreateNumberedMessage(int index, ValidationResult validationMessage)
            {
                return $" ({index + 1}) {validationMessage.Message}";
            }
        }


        private string DisplayMessageDefaultAuthorizeAttribute => DefaultAuthorizeAttribute.Equals(null)
            ? "not set"
            : DefaultAuthorizeAttribute.GetType().Name;
        
        
        /// <summary>
        /// returns the Policy name if set, otherwise default message
        /// </summary>
        /// <returns></returns>
        private string DisplayMessageDefaultAuthorizationPolicy()
        {
            var defaultMessage = "not set";
            //this will return "not set" if AllowAnonymous is set as the default. .ok
            if (DefaultAuthorizeAttribute == null 
                || DefaultAuthorizeAttribute.GetType() != typeof(AuthorizeAttribute))
            {
                return defaultMessage;
            }
                
            var authorizeAttribute = (AuthorizeAttribute) DefaultAuthorizeAttribute;
            return string.IsNullOrEmpty(authorizeAttribute.Policy) 
                ? defaultMessage 
                : authorizeAttribute.Policy;

        }

        /// <summary>
        /// Initializes the test by merging configured rules into the default rules
        /// </summary>
        private void PrepareForTest()
        {
            //Init()
            
            AuthorizationValidationRules = new ValidationRulesContainer(_actionsToValidate, ValidationRulesConfigured);

            AuthorizationValidationRules.PopulateDefaultRules(DefaultAuthorizeAttribute);
            AuthorizationValidationRules.MergeDefaultWithConfiguredRules();
        }

        /// <summary>
        /// Asserts the default rules are created from controller actions in given Assembly (with the default authorization type and policy applied)
        /// And at least one configured rule, for controller or action, exists
        /// </summary>
        private void ValidateConfiguration()
        {
            //ConfigurationDefaults_are_valid(); // maybe not needed here/in builder, is triggered by builder also.. 
            Default_authorization_rules_are_created();
            And_configured_authorization_rules_exists();
        }

        
        /// <summary>
        /// Assert default rules are created
        /// </summary>
        private void Default_authorization_rules_are_created()
        {
            //assert
            if (!AuthorizationValidationRules.AllRules().Any())
            {
                throw new XunitException(
                    $"Something is wrong, we expect to have rules created automatically from all controller actions, is the assembly under test '{_assemblyName}' missing controllers?");
            }
        }
        
        /// <summary>
        /// Assert at least one rule is configured
        /// todo ? perhaps allow to set empty configuration, to only use the default rules for all
        /// Ex: UseOnlyDefaultAuthorization.
        /// </summary>
        private void And_configured_authorization_rules_exists()
        {
            //assert     
            if (!ValidationRulesConfigured.Any())
            {
                throw new XunitException("There are no configured validation rules, did you forget to create rules?");
            }
        }

        /// <summary>
        /// Validates all rules and collects the result in ValidationResults
        /// <br/>
        /// Printable validation/test results for all controller actions
        /// (xUnit throws are stored in the validation messages)
        /// </summary>
        /// <returns></returns>
        private List<ValidationResult> All_actions_validation_results()
        {
            var validationMessages = new List<ValidationResult>();

            // todo: test if it feels better to have each rule containing its own "Exception Message" after validation.
            // meaning.. move the Should.. and Catch into the validate in the DefaultRule. 

            foreach (var rule in AuthorizationValidationRules.AllRules())
            {
                //act
                var validationResult = rule.Validate();

                //assert
                try
                {
                    validationResult.IsValid.Should().Be(true, $" - {validationResult.Message}");
                    validationMessages.Add(validationResult); //store successful validation
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    validationResult.SetException(e);
                    validationMessages.Add(validationResult); //store failed validation
                }
            }

            return validationMessages;
        }

    }
}
