using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Public;
using Xunit.Sdk;

namespace LeeWay.Ensure.ControllerAttributes.Internal
{
    /// <summary>
    /// This is a "container/factory" for all validation rules, defaults and configured
    /// </summary>
    internal sealed class ValidationRulesContainer : IValidationRulesContainer, IDisposable
    {
        public readonly IEnumerable<MethodInfo> _controllerActionsToValidate;

        /*
         * Note to self: controller rules and acton rules could be in one, but lets keep like this
         * because the action rules, as they are more specific, must be applied / merged after controller rules
         * */
        public readonly List<IValidationRule> _configuredControllerRules;
        public readonly List<IValidationRule> _configuredActionMethodRules;

        private List<IValidationRule> AuthorizationValidationRules = new List<IValidationRule>();

        /// <summary>
        /// </summary>
        /// <param name="controllerActionsToValidate"></param>
        /// <param name="validationRules"></param>
        public ValidationRulesContainer(IEnumerable<MethodInfo> controllerActionsToValidate, List<IValidationRule> validationRules)
        {
            if (controllerActionsToValidate == null || !controllerActionsToValidate.Any())
            {
                throw new XunitException("you must provide the controller actions to validate, what's the point of doing anything at all?(Johnossi https://open.spotify.com/track/6zu9oVexxZt6qmyGA4X7WR?si=zQ5BNkjFQQ6kqvgJ7O39RA)");
            }
            
            if (validationRules == null || !validationRules.Any())
            {
                throw new XunitException("you must provide validation rules"); 
            }

            _controllerActionsToValidate = controllerActionsToValidate;

            _configuredControllerRules = validationRules.Where(rule => rule.IsControllerLevel).ToList();
            _configuredActionMethodRules = validationRules.Where(rule => !rule.IsControllerLevel).ToList();
        }

        /// <summary>
        /// All default and configured rules
        /// </summary>
        public List<IValidationRule> AllRules()
        {
            return AuthorizationValidationRules;
        }

        /// <summary>
        /// Default attribute (authorization type and policy), will be used as required on all actions;
        /// <br/>
        /// unless controller or action is defined in the configured validation rules
        /// </summary>
        public void PopulateDefaultRules(Attribute defaultAttribute)
        {
            if (AuthorizationValidationRules.Any()) //if already executed
            {
                return;
            }

            var actions = _controllerActionsToValidate;

            foreach (var action in actions)
            {
                    
                AuthorizationValidationRules.Add(
                    //default action rule with default Attribute from builder
                    new ValidationRuleActionDefault(action, defaultAttribute)
                );
            }
        }

        /// <summary>
        /// Merges rules by applying the configured controller rules and then the configured action rules
        /// <br/>
        /// - default rule  - configured controller rule  - configured action rule
        /// <br/>
        /// action rules have highest precedence, more exact precision, and will be the final rule.
        /// </summary>
        public IValidationRulesContainer MergeDefaultWithConfiguredRules()
        {
            if (!AuthorizationValidationRules.Any())
            {
                throw new InvalidOperationException(
                    $"Default rules must be populated before merging - in " +
                    $"{nameof(ValidationRulesContainer)}.{nameof(MergeDefaultWithConfiguredRules)}()");
            }

            _configuredControllerRules.ForEach(ReplaceActionRulesForController);

            _configuredActionMethodRules.ForEach(ReplaceActionRule);

            return this;
        }

        /// <summary>
        /// Replaces all default actions rules for the controller
        /// </summary>
        /// <param name="controllerRuleUserDefined"></param>
        private void ReplaceActionRulesForController(IValidationRule controllerRuleUserDefined)
        {
            bool MatchControllerName(IValidationRule rule) => rule.ControllerName == controllerRuleUserDefined.ControllerName;

            MergeRules(controllerRuleUserDefined, MatchControllerName);
        }

        private void ReplaceActionRule(IValidationRule actionRuleUserDefined)
        {
            bool MatchControllerAndAction(IValidationRule rule) => 
                rule.Action.DeclaringType.FullName == actionRuleUserDefined.Action.DeclaringType.FullName 
                && rule.Action.ToString() == actionRuleUserDefined.Action.ToString();

            MergeRules(actionRuleUserDefined, MatchControllerAndAction);
        }

        /// <summary>
        /// Replaces the default validation rules with the ones from users configured rules
        /// (sets the configured Attribute required)
        /// </summary>
        /// <param name="validationRuleUserDefined"></param>
        /// <param name="matchRule"></param>
        private void MergeRules(IValidationRule validationRuleUserDefined, Func<IValidationRule, bool> matchActionAndOrController)
        {
            var rulesToReplace = AuthorizationValidationRules.Where(matchActionAndOrController).ToList();

            var newRules = rulesToReplace.Select(oldRule =>
                new ValidationRuleActionDefault(
                    oldRule.Action, 
                    validationRuleUserDefined.AttributeRequired
                ));

            rulesToReplace.ForEach(rule => AuthorizationValidationRules.Remove(rule));
            AuthorizationValidationRules.AddRange(newRules);
        }

        public void Dispose()
        {
            AuthorizationValidationRules.Clear();
        }
    }
}