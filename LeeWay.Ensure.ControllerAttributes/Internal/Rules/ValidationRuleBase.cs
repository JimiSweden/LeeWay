using System;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeWay.Ensure.ControllerAttributes.Internal.Rules
{
    // todo: ? loosen up inheritance, to avoid the Validate

    /// <summary>
    /// The generic version of rules created by the user
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    internal abstract class ValidationRuleBase<TController> : IValidationRule where TController : ControllerBase
    {
        /// <summary>
        /// Used by Controller Rule, should perhaps not be in base? why not.. :)
        /// </summary>
        /// <param name="attributeRequired"></param>
        protected ValidationRuleBase(Attribute attributeRequired)
        {
            AttributeRequired = attributeRequired;
        }
        
        /// <summary>
        /// used by Action Rule
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attributeRequired"></param>
        protected ValidationRuleBase(MethodInfo action, Attribute attributeRequired)
        {
            Action = action;
            AttributeRequired = attributeRequired;
        }

        public Attribute AttributeRequired { get; set; }

        public string ControllerName => typeof(TController).Name;
        public MethodInfo Action { get; set; }
        
        public bool IsControllerLevel => this.GetType().Name.Contains("Controller");

        /// <summary>
        /// TODO: remove this. perhaps redo the base and not using the interface here?
        /// </summary>
        /// <returns></returns>
        public ValidationResult Validate()
        {
            throw new NotImplementedException();
        }

    }
}