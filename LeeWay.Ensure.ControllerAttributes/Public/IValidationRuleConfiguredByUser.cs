using System;
using System.Reflection;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    /// <summary>
    /// Used for rules created by the user in "Configuration"
    /// for rules used in validation engine see <seealso cref="IValidationRuleInternal"/>
    /// </summary>
    public interface IValidationRuleConfiguredByUser
    {
        MethodInfo Action { get; }
        Attribute AttributeRequired { get; set; }
        string ControllerName { get; }

        /// <summary>
        /// Implemented in ValidationRuleBase, for selecting configured rule types in ValidationRulesContainer
        /// </summary>
        bool IsControllerLevel { get;  }

    }
}