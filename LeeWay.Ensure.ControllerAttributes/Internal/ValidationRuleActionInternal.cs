using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using LeeWay.Ensure.ControllerAttributes.Public;
using Microsoft.AspNetCore.Authorization;

namespace LeeWay.Ensure.ControllerAttributes.Internal
{
    /// <summary>
    /// Used internally for creating a set of default rules for all controller actions in the assembly being validated,
    /// <br/>
    /// and for creating "copies" of the configured rules before validating the complete set.
    /// <br/>
    /// Configured rules matching default rules will replace the default ones. 
    /// <br/>
    /// The action rules are the only rule types validated,
    /// configured controller rules will result in action rules for all of its actions.
    /// </summary>
    internal sealed class ValidationRuleActionInternal : ValueObject, IValidationRuleInternal
    {
        /// <summary>
        /// Controller action and the required attribute
        /// <br/>
        /// at the moment Attribute should be of type Authorize or AllowAnonymous
        /// <br/>
        /// supported attribute types might grow in the future
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attributeRequired"></param>
        public ValidationRuleActionInternal(MethodInfo action, Attribute attributeRequired)
        {
            //todo ? : add validation of Attribute type to constraint only supported
            Action = action ?? throw new ArgumentNullException(nameof(action), "action can not be null");
            AttributeRequired = attributeRequired ?? throw new ArgumentNullException(nameof(attributeRequired), "attribute can not be null");
        }
        
        public Attribute AttributeRequired { get; set; }

        public string ControllerName => Action.DeclaringType.Name;

        public MethodInfo Action { get; }

        public ValidationResult ValidationResult { get; set; }

        /// <summary>
        /// todo (perhaps): make the Validate functions into classes/handlers that can listen to this.
        /// - like dependency injection in Startup ? For ''ControllerRule' use 'Validate TController'
        /// </summary>
        /// <returns></returns>
        public ValidationResult Validate()
        {
            var validationResult = ValidationResultFactory.Create(this, HasRequiredAttribute());
            ValidationResult = validationResult;

            //todo ? perhaps return this, to allow caller to call Validate().ValidationResult 
            return validationResult; //set internal message?
        }


        /// <summary>
        /// This is the core of the attribute validation, checking if required attribute is met.
        /// <br/>
        /// Note to self: this was previously public and called in ValidationResultFactory.Create,
        /// but passing this result as parameter makes the code easier to reason about
        /// and encapsulates the actual validation.
        /// <br/>
        /// Separating the result message responsibility from executing validation is good.
        /// </summary>
        /// <returns></returns>
        private bool HasRequiredAttribute()
        {
            var requiredType = AttributeRequired.GetType();
            if (requiredType == typeof(AllowAnonymousAttribute))
            {
                //fail if authorize attribute on action
                return HasNoAuthorizeAttribute() && HasAllowAnonymousAttribute();
            }

            //fail if not supported attribute (i e other than Anonymous and Authorize)
            if (requiredType != typeof(AuthorizeAttribute))
            {
                return false;
            }

            var attributes = CustomAuthorizeAttributesFromAction().ToList();
            if (!attributes.Any())
            {   //only add attribute from controller if action has none; because policy can differ
                attributes.AddRange(CustomAuthorizeAttributesFromController());
            }
                
            var hasRequiredAttribute = attributes.Any(attribute => attribute.Equals(AttributeRequired));

            return hasRequiredAttribute;
        }

        private bool HasNoAuthorizeAttribute()
        {
            return !CustomAuthorizeAttributesFromAction().Any();
        }

        public IReadOnlyList<AuthorizeAttribute> CustomAuthorizeAttributesFromAction()
        {
            return CustomAuthorizeAttributesFromMemberInfo(Action);
        }
        
        public IReadOnlyList<AuthorizeAttribute> CustomAuthorizeAttributesFromController()
        {
            return CustomAuthorizeAttributesFromMemberInfo(Action.DeclaringType);
        }

        private IReadOnlyList<AuthorizeAttribute> CustomAuthorizeAttributesFromMemberInfo(MemberInfo memberInfo)
        {
            // <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false.
            var authorizeAttributes = memberInfo
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
                .Select(attr => attr.As<AuthorizeAttribute>());

            return authorizeAttributes.ToList();
        }

        private bool HasAllowAnonymousAttribute()
        {
            var isValid = ActionHasAllowAnonymousAttribute()
                          || ControllerHasAllowAnonymousAttribute();

            return isValid;
        }

        public bool ControllerHasAllowAnonymousAttribute()
        {

            return Action.DeclaringType.GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: false).Any();
        }

        public bool ActionHasAllowAnonymousAttribute()
        {
            return Action.GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: false).Any();
        }
        
        public string ActionNameForDisplayWithParameters()
        {
            var parameterInfos = Action.GetParameters().Select(info => new MyParameterInfo(info))
                .Select(info => $"{info.Type.Name} {info.Name}");

            var parameterText = string.Join(", ", parameterInfos);

            var displayText = $"{Action.Name}({parameterText})";
            return displayText;
        }

        /// <summary>
        /// Equality is checked on properties in here
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Action;
            yield return AttributeRequired;
            yield return ValidationResult;
        }
    }
}