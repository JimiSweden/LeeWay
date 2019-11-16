using System;
using System.Reflection;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    /// <summary>
    /// Used for rules created by the user AND for rules created internally (default population)
    /// </summary>
    public interface IValidationRule
    {
        MethodInfo Action { get; }
        Attribute AttributeRequired { get; set; }
        string ControllerName { get; }
        
        ValidationResult Validate();

        /// <summary>
        /// Implemented in ValidationRuleBase, for selecting configured rule types in ValidationRulesContainer
        /// <br/>
        /// TODO : remove dependency / (separate or merge rules?)
        /// </summary>
        bool IsControllerLevel { get;  }

    }
}