using System;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeWay.Ensure.ControllerAttributes.Internal.Rules
{
    /// <summary>
    /// The generic version of rules created by the user
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    internal abstract class ValidationRuleConfiguredByUserBase<TController> : IValidationRuleConfiguredByUser where TController : ControllerBase
    {
        /// <summary>
        /// Used by Controller Rule, should perhaps not be in base? why not.. :)
        /// </summary>
        /// <param name="attributeRequired"></param>
        protected ValidationRuleConfiguredByUserBase(Attribute attributeRequired)
        {
            AttributeRequired = attributeRequired;
        }
        
        /// <summary>
        /// used by Action Rule
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attributeRequired"></param>
        protected ValidationRuleConfiguredByUserBase(MethodInfo action, Attribute attributeRequired)
        {
            Action = action;
            AttributeRequired = attributeRequired;
        }

        public Attribute AttributeRequired { get; set; }

        public string ControllerName => typeof(TController).FullName; 
        public MethodInfo Action { get; set; }
        
        //endast här
        public bool IsControllerLevel => this.GetType().Name.Contains("Controller");
    }
}