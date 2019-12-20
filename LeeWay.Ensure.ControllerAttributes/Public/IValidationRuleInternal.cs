using System;
using System.Reflection;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    /// <summary>
    /// Used for rules created internally (default population)
    /// <br/>
    /// for rules created by user in configuration see <seealso cref="IValidationRuleConfiguredByUser"/>
    /// </summary>
    public interface IValidationRuleInternal
    {
        MethodInfo Action { get; }
        Attribute AttributeRequired { get; set; }
        string ControllerName { get; }
        
        ValidationResult Validate();

    }
}