using System;
using System.Collections.Generic;
using LeeWay.Ensure.ControllerAttributes.Public;

namespace LeeWay.Ensure.ControllerAttributes.Internal
{
    internal interface IValidationRulesContainer
    {
        /// <summary>
        /// All default and configured rules
        /// </summary>
        List<IValidationRuleInternal> AllRules();

        /// <summary>
        /// Default attribute (authorization type and policy), will be used as required on all actions;
        /// <br/>
        /// unless controller or action is defined in the configured validation rules
        /// </summary>
        void PopulateDefaultRules(Attribute defaultAttribute);

        /// <summary>
        /// Merges rules by applying the configured controller rules and then the configured action rules
        /// <br/>
        /// - default rule  - configured controller rule  - configured action rule
        /// <br/>
        /// action rules have highest precedence, more exact precision, and will be the final rule.
        /// </summary>
        IValidationRulesContainer MergeDefaultWithConfiguredRules();

    }
}