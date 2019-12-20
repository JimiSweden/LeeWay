using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Internal;
using LeeWay.Ensure.ControllerAttributes.Internal.Rules;
using Microsoft.AspNetCore.Mvc;
using Xunit.Sdk;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    
    /// <summary>
    /// Used to configure validation rules for controllers and controller actions
    /// You need to set a default Attribute
    /// Attribute types supported: Authorize
    /// </summary>
    public sealed class ValidatorBuilder
    {
        private readonly string _assemblyName;
        private readonly IControllersFromAssembly _controllersFromAssembly;
        private readonly IValidator _validator;

        /// <summary>
        /// The builder helps you configure rules and add them to the validator that you call to execute a set of rules.
        /// - you would set this up once for each project where you want to enforce attributes in one or multiple test projects.
        /// <br/>
        /// Tip: don't use pure text as assembly name, instead use nameof,  like this <see cref="T:ValidatorBuilder(nameof(WebApi))" />, to avoid renaming related issues.
        /// <br/>
        /// Tip: defaultAttributeRequired;  normally you would set the most restrictive attribute 
        ///  to avoid giving to low privilege level on controllers/actions
        /// </summary>
        /// <param name="assemblyName">The name of the assembly under test, all controllers will be collected for validation</param>
        /// <param name="defaultAttributeRequired">AllowAnonymous, AuthorizeAttribute, AuthorizeAttribute(Policy = "YourPolicyName")</param>
        public ValidatorBuilder(string assemblyName, Attribute defaultAttributeRequired)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new XunitException($"Configuration error, Assembly name is missing");
            }

            _assemblyName = assemblyName;
            _controllersFromAssembly = new ControllersFromAssembly(_assemblyName);
            _validator = CreateValidator(defaultAttributeRequired);
        }

        private IValidator CreateValidator(Attribute defaultAttribute)
        {
            var actionsToValidate = _controllersFromAssembly.AllActions();
            var validator = new Validator(actionsToValidate, _assemblyName)
            {
                DefaultAuthorizeAttribute = defaultAttribute
            };

            return validator;
        }
        /// <summary>
        /// Excludes controllers where path contains parameter "pathContains"
        /// <br/>
        /// You should probably give the full namespace path to match as strict as possible
        /// <br/>
        /// TODO: this method should add excludes to a list (strings) and that list and the _validator.ExcludeActionsFromValidation should run on Build
        /// will make the order of this and rules independent. more fail safe. 
        /// TODO ? add multiple paths, nice to have and not much work needed. (will need a test to validate controller in paths are removed)
        /// </summary>
        /// <param name="pathContains"></param>
        /// <returns></returns>
        public ValidatorBuilder ExcludeControllersInPath(string pathContains)
        {
            var allActions = _controllersFromAssembly.AllActions().ToList();

            var actionsToExclude = allActions.Where(ContainsPath).ToList();
           
            _validator.ExcludeActionsFromValidation(actionsToExclude);

            return this;

            //locals >
            bool ContainsPath(MethodInfo action)
            {
                return action.DeclaringType.FullName.Contains(pathContains);
            }
        }




        /// <summary>
        /// NOTE: TODO: probably remove this; when decided to always require attribute in constructor
        ///  todo: perhaps allow to set "no" attribute as the default?
        /// <br/>
        /// Sets the default attribute of types 
        ///  AllowAnonymous, AuthorizeAttribute, AuthorizeAttribute(Policy = "YourPolicyName")
        /// <br/>
        /// Normally you would set the most restrictive attribute; 
        /// to avoid giving to low privilege level on controllers/actions
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public ValidatorBuilder WithDefaultAuthorizeAttribute(Attribute attribute)
        {
            throw new NotImplementedException("todo.. ");
            //_validator.DefaultAuthorizeAttribute = attribute;
            //return this;
        }

        /// <summary>
        /// Build the Validator with all rules ready to validate.
        /// </summary>
        public IValidator Build()
        {
            return _validator;
        }


        /// <summary>
        /// Controller rules will apply to all actions inside the controller,
        /// <br/>
        /// unless an action is specified in a separate action rule, then the action rule will take precedence
        /// </summary>
        /// <typeparam name="T">of type ControllerBase</typeparam>
        /// <param name="attributeRequired"></param>
        /// <returns></returns>
        public ValidatorBuilder AddControllerRule<T>(Attribute attributeRequired) where T: ControllerBase
        {
            var validationRuleController = new ControllerRuleConfiguredByUser<T>(attributeRequired);
            _validator.ValidationRulesConfiguredByUser.Add(validationRuleController);
            return this;
        }

        /// <summary>
        /// This rule type is the simplest to use.
        /// <br/>
        /// Note that if you have more than one action with the same name
        /// this rule will only be applied to the first action found
        /// in the order as found in the controller class. 
        /// <br/>
        /// If you need different rules on f ex two Get methods in the same controller
        /// you should use the overloads that takes parameterInfo(s),
        ///  or just name your methods explicitly (like : GetAll and GetById)
        /// </summary>
        public ValidatorBuilder AddActionRule<T>(string actionName, Attribute attributeRequired) where T : ControllerBase
        {
            var validationRuleAction = new ActionRuleConfiguredByUser<T>(actionName, attributeRequired);
            _validator.ValidationRulesConfiguredByUser.Add(validationRuleAction);
            return this;
        }

        /// <summary>
        /// This action rule is used when you need to match on a single parameter.
        /// <br/>
        /// (note: my preference and recommendation is to always name actions differently, but I know some don't and therefore I added this)
        /// <br/> 
        /// The parameterInfo is used to match the exact controller Action
        /// <br/>
        /// If more than one exists with the same name,
        /// or if you want to ensure the parameter explicitly by configuration
        /// <br/>
        /// for multiple parameters see <seealso cref="AddActionRule{T}(string,IEnumerable{MyParameterInfo},Attribute)" />
        /// </summary>
        public ValidatorBuilder AddActionRule<T>(string actionName,
            MyParameterInfo parameterInfo, Attribute attributeRequired) where T : ControllerBase
        {
            var parameterInfos = new List<MyParameterInfo>{ parameterInfo };
            return AddActionRule<T>(actionName, parameterInfos, attributeRequired);
        }

        /// <summary>
        /// This action rule is used when you need to match on multiple parameters
        /// <br/> 
        /// The parameterInfos are used to match the exact controller Action,
        /// if more than one exists with the same name,
        /// or if you want to ensure the parameters explicitly by configuration
        /// <br/>
        /// see also <seealso cref="AddActionRule{T}(string,MyParameterInfo,Attribute)" />
        /// </summary>
        public ValidatorBuilder AddActionRule<T>(string actionName, IEnumerable<MyParameterInfo> parameterInfos, Attribute attributeRequired) where T : ControllerBase
        {
            var validationRuleUserDefined = new ActionRuleConfiguredByUser<T>(actionName, parameterInfos, attributeRequired);
            _validator.ValidationRulesConfiguredByUser.Add(validationRuleUserDefined);
            return this;
        }
       
    }
}