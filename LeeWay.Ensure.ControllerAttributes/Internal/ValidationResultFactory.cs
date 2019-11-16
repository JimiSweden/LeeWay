using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeeWay.Ensure.ControllerAttributes.Public;
using Microsoft.AspNetCore.Authorization;

namespace LeeWay.Ensure.ControllerAttributes.Internal
{
    /// <summary>
    /// Creates the validation result with messages formatted in human readable text
    /// </summary>
    internal static class ValidationResultFactory
    {
        /// <summary>
        /// Creates a result message with human readable text formatted for console output
        /// </summary>
        /// <param name="actionRule"></param>
        public static ValidationResult Create(ValidationRuleActionDefault actionRule, bool isValid)
        {   
            var requiredAttribute = AttributeForDisplay(actionRule.AttributeRequired);
            var requiredAttributeMessage = $"\t required attribute: \t{requiredAttribute}";

            var messageBuilder = new StringBuilder();
            messageBuilder
                .AppendLine($"{actionRule.ControllerName}.{actionRule.ActionNameForDisplayWithParameters()}");

            if (!isValid)
            {
                messageBuilder.AppendLine(ActualAttributesMessage(actionRule));
            }

            messageBuilder.AppendLine(requiredAttributeMessage);

            return new ValidationResult(
                isValid: isValid,
                message: messageBuilder.ToString());
        }

        private static string ActualAttributesMessage(ValidationRuleActionDefault actionRule)
        {
            var authorizeAttributesForDisplay = CustomAuthorizeAttributes(actionRule)
                .Select(AuthorizeAttributeAndPolicyNameForDisplay).ToList();

            var formattedAuthAttributes = string.Join(", ", authorizeAttributesForDisplay);

            //todo: cleanup. works but messy
            var anonymousAttributeForDisplay = authorizeAttributesForDisplay.Any()
                ? string.Empty
                : actionRule.ActionHasAllowAnonymousAttribute()
                  || actionRule.ControllerHasAllowAnonymousAttribute()
                    ? AttributeForDisplay(new AllowAnonymousAttribute())
                    : ""; //todo: why not string.empty here? 

            var actualAttributesFormatted = $"{formattedAuthAttributes}{anonymousAttributeForDisplay}";
            var actualAttributesMessage = $"\t actual attribute(s): \t{actualAttributesFormatted}";

            return actualAttributesMessage;
        }

        private static string AttributeForDisplay(Attribute attribute)
        {
            if (attribute.GetType() == typeof(AllowAnonymousAttribute))
            {
                return "[AllowAnonymous]";
            }

            if (attribute.GetType() == typeof(AuthorizeAttribute))
            {
                return AuthorizeAttributeAndPolicyNameForDisplay((AuthorizeAttribute)attribute);
            }

            return $"not an authorize attribute: {attribute.GetType().Name}";
        }

        private static string AuthorizeAttributeAndPolicyNameForDisplay(AuthorizeAttribute attribute)
        {
            var policyIfAvailable = string.IsNullOrEmpty(attribute.Policy)
                ? string.Empty
                : $" (Policy = {attribute.Policy})"; //space before is by intention

            return $"[{attribute.GetType().Name}{policyIfAvailable}]";
        }

        private static IReadOnlyList<AuthorizeAttribute> CustomAuthorizeAttributes(ValidationRuleActionDefault actionRule)
        {
            var attributesFromAction = actionRule.CustomAuthorizeAttributesFromAction();

            return attributesFromAction.Any() 
                ? attributesFromAction
                : actionRule.CustomAuthorizeAttributesFromController();
        }
    }
}