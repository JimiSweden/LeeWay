using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using LeeWay.Ensure.ControllerAttributes.Internal;
using LeeWay.Ensure.ControllerAttributes.Public;
using LeeWay.Ensure.ControllerAttributes.Tests.Fakes;
using Microsoft.AspNetCore.Authorization;
using Xunit;

// ReSharper disable InconsistentNaming

namespace LeeWay.Ensure.ControllerAttributes.Tests
{
    public class ValidationRuleActionDefaultTest
    {
        /*       
            //perhaps todo later? (ok for now): consolidate validation rules? pass in parameters (lots of boiler code right now for the Arrange)
        
        # Invalid scenarios
        [x] - rule requires AllowAnonymous, action has Authorize  
        [x] - rule requires AllowAnonymous, action has none and Controller has Authorize  
        [x] - rule requires AllowAnonymous, action has none and Controller has none
        
        [x] - rule requires Authorize, action has AllowAnonymous
        [x] - rule requires Authorize, action has none and Controller has AllowAnonymous
        [x] - rule requires Authorize, action has none and Controller has none

        [x] - rule requires Authorize with policy, action has Authorize with no policy
        [x] - rule requires Authorize with policy, action has no attribute and Controller has Authorize with no policy
        [x] - rule requires Authorize with policy, action has no attribute and Controller has Authorize with wrong policy
        [x] - rule requires Authorize with policy, action has Authorize with wrong policy, and Controller has Authorize with correct policy
        [x] - rule requires Authorize with policy, action has Authorize with no policy, and Controller has Authorize with policy

        # Valid scenarios        
        [x] - rule requires AllowAnonymous, action has AllowAnonymous
        [x] - rule requires AllowAnonymous, action has none but Controller has AllowAnonymous
        [x] - rule requires Authorize, action has Authorize
        [x] - rule requires Authorize, action has none but Controller has Authorize
        [x] - rule requires Authorize with Policy, action has Authorize with Policy
        [x] - rule requires Authorize with Policy, action has none but Controller has Authorize with Policy

       */

        [Fact]
        public void Equality_compares_on_expected_properties()
        {
            //arrange
            var actionRule = Helper.ActionRule_withNoAttribute_Controller_hasAllowAnnonymous();

            var ruleOne = new ValidationRuleActionInternal(actionRule.Action, actionRule.AttributeRequired);
            var ruleTwo = new ValidationRuleActionInternal(actionRule.Action, actionRule.AttributeRequired);

            //assert
            ruleOne.Equals(ruleTwo).Should().BeTrue(" we care about values and not identity");
        }

        [Fact]
        public void CustomAuthorizeAttributesFromAction_returns_attribute_from_action()
        {
            var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));

            var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_Policy_RequireAdmin);
            var action = controllerInfo.ActionFirstOrDefault(actionName);

            var expectedAttribute = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);

            var rule = new ValidationRuleActionInternal(action, new AuthorizeAttribute()); 

            var actualAttribute = rule.CustomAuthorizeAttributesFromAction().First();
            actualAttribute.Should().BeEquivalentTo(expectedAttribute);
        }

        [Fact]
        public void CustomAuthorizeAttributesFromController_returns_attribute_from_controller()
        {
            var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_Policy_RequireAdmin));

            var actionName = nameof(FakeControllers.Controller_with_Policy_RequireAdmin.Action_with_Authorize_attribute);
            var action = controllerInfo.ActionFirstOrDefault(actionName);

            var expectedAttribute = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);

            var rule = new ValidationRuleActionInternal(action, new AuthorizeAttribute());

            var actualAttribute = rule.CustomAuthorizeAttributesFromController().First();
            actualAttribute.Should().BeEquivalentTo(expectedAttribute);
        }


        /*
         * a Controller rule will be applied on all actions,
         * therefore an action rule is "the same" as a controller rule
         */
        public class Given_Rule_Requires_AllowAnonymous
        {

            public class Then_Fail_when
            {
                private readonly Attribute _requiredAnonymousAttribute = new AllowAnonymousAttribute();

                [Fact]
                public void Action_has_no_attribute_and_Controller_has_no_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_no_attribute);
                    var actionWithNoAttribute = controllerInfo.ActionFirstOrDefault(actionName);
                    
                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithNoAttribute, _requiredAnonymousAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }

                [Fact]
                public void Action_has_Authorize_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_Authorize_attribute);
                    var actionWithAuthorize = controllerInfo.ActionFirstOrDefault(actionName);
                    
                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorize, _requiredAnonymousAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }


                [Fact]
                public void Action_has_no_attribute_and_Controller_has_Authorize_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_authorize_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_authorize_attribute.Action_with_no_attribute);
                    var actionWithAuthorizeFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    
                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizeFromController, _requiredAnonymousAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }
            }

            public class Then_Pass_when
            {
                private readonly Attribute _requiredAnonymousAttribute = new AllowAnonymousAttribute();
                [Fact]
                public void Action_has_AllowAnonymous_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_AllowAnonymous_attribute);
                    var actionWithAllowAnonymous = controllerInfo.ActionFirstOrDefault(actionName);
                    
                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAllowAnonymous, _requiredAnonymousAttribute);

                    //assert
                    isValid.Should().BeTrue();
                }

                [Fact]
                public void Action_has_no_attribute_but_Controller_has_AllowAnonymous_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_AllowAnonymous));
                    var actionName = nameof(FakeControllers.Controller_with_AllowAnonymous.Action_with_no_attribute);
                    var actionWithAllowAnonymousFromController = controllerInfo.ActionFirstOrDefault(actionName);


                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAllowAnonymousFromController, _requiredAnonymousAttribute);

                    //assert
                    isValid.Should().BeTrue();
                }
            }
        }

        public class Given_Rule_Requires_Authorize
        {
            public class Then_Fail_when
            {
                private readonly Attribute _requiredAuthorizeAttribute = new AuthorizeAttribute();

                [Fact]
                public void Action_has_AllowAnonymous_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_AllowAnonymous_attribute);
                    var actionWithAnonymous = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAnonymous, _requiredAuthorizeAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }

                [Fact]
                public void Action_has_no_attribute_and_Controller_has_AllowAnonymous()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_AllowAnonymous));
                    var actionName = nameof(FakeControllers.Controller_with_AllowAnonymous.Action_with_no_attribute);
                    var actionWithAnonymousFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAnonymousFromController, _requiredAuthorizeAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }

                
                [Fact]
                public void Action_has_no_attribute_and_Controller_has_no_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_no_attribute);
                    var actionWithNoAttributes = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithNoAttributes, _requiredAuthorizeAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }
            }

            public class Then_Pass_when
            {
                private readonly Attribute _requiredAuthorizeAttribute = new AuthorizeAttribute();

                [Fact]
                public void Action_has_Authorize_attribute()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_Authorize_attribute);
                    var actionWithAuthorize = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorize, _requiredAuthorizeAttribute);

                    //assert
                    isValid.Should().BeTrue();
                }

                
                [Fact]
                public void Action_has_no_attribute_but_Controller_has_Authorize()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_authorize_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_authorize_attribute.Action_with_no_attribute);
                    var actionWithAuthorizeFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizeFromController, _requiredAuthorizeAttribute);

                    //assert
                    isValid.Should().BeTrue();
                }
            }


        }

        public class Given_Rule_Requires_Authorize_Policy
        {
            public class Then_Fail_when
            {
                private readonly Attribute _requiredAuthorizePolicyAttribute = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);

                [Fact]
                public void Action_has_Authorize_attribute_missing_policy()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_Authorize_attribute);
                    var actionWithAuthorize = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorize, _requiredAuthorizePolicyAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }



                [Fact]
                public void Action_has_no_attribute_and_Controller_has_Authorize_attribute_missing_policy()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_authorize_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_authorize_attribute.Action_with_no_attribute);
                    var actionWithAuthorizeFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizeFromController, _requiredAuthorizePolicyAttribute);

                    //assert
                    isValid.Should().BeFalse();
                }

                [Fact]
                public void Action_has_no_attribute_and_Controller_has_Authorize_attribute_with_Wrong_policy()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_Policy_RequireAdmin));
                    var actionName = nameof(FakeControllers.Controller_with_Policy_RequireAdmin.Action_with_no_attribute);
                    var actionWithAuthorizeFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    var requiredAuthorizePolicy = new AuthorizeAttribute(PolicyNames.RequireAuthorizedUser);
                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizeFromController, requiredAuthorizePolicy);

                    //assert
                    isValid.Should().BeFalse();
                }



                [Fact]
                public void Action_has_Authorize_attribute_missing_policy_and_Controller_has_Authorize_attribute_with_policy()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_Policy_RequireAdmin));
                    var actionName = nameof(FakeControllers.Controller_with_Policy_RequireAdmin.Action_with_Authorize_attribute);
                    var actionWithAuthorizeFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    var requiredAuthorizePolicyAttribute = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizeFromController, requiredAuthorizePolicyAttribute);

                    //assert
                    isValid.Should().BeFalse(" - the attribute on the action has precedence over the controller attribute");
                }

                [Fact]
                public void Action_has_Authorize_attribute_with_Wrong_policy_and_Controller_has_Authorize_attribute_with_correct_policy()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_Policy_RequireAdmin));
                    var actionName = nameof(FakeControllers.Controller_with_Policy_RequireAdmin.Action_with_Policy_RequireUser);
                    var actionWithAuthorizeFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    var requiredAuthorizePolicyAttribute = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizeFromController, requiredAuthorizePolicyAttribute);

                    //assert
                    isValid.Should().BeFalse(" - the attribute on the action has precedence over the controller attribute");
                }
            }

            public class Then_Pass_when
            {
                private readonly Attribute _requiredAuthorizePolicyAttribute = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);

                
                [Fact]
                public void Action_has_Authorize_attribute_with_Policy()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_no_attribute));
                    var actionName = nameof(FakeControllers.Controller_with_no_attribute.Action_with_Policy_RequireAdmin);
                    var actionWithAuthorizePolicy = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizePolicy, _requiredAuthorizePolicyAttribute);

                    //assert
                    isValid.Should().BeTrue();
                }


                [Fact]
                public void Action_has_no_attribute_but_Controller_has_Authorize_attribute_with_Policy()
                {
                    //arrange
                    var controllerInfo = new ControllerInfo(typeof(FakeControllers.Controller_with_Policy_RequireAdmin));
                    var actionName = nameof(FakeControllers.Controller_with_Policy_RequireAdmin.Action_with_no_attribute);
                    var actionWithAuthorizePolicyFromController = controllerInfo.ActionFirstOrDefault(actionName);

                    //act
                    var isValid = ValidationRule_HasRequiredAttribute(actionWithAuthorizePolicyFromController, _requiredAuthorizePolicyAttribute);

                    //assert
                    isValid.Should().BeTrue();
                }
            }
        }


        /// <summary>
        /// invokes the private method HasRequiredAttribute on ValidationRuleActionInternal
        /// TODO: probably make HasRequiredAttribute public and make this cleaner.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attributeRequired"></param>
        /// <returns></returns>
        public static bool ValidationRule_HasRequiredAttribute(MethodInfo action, Attribute attributeRequired)
        {
            action.Should().NotBeNull("if action was not found, the error is in the setup/arrange, check controllerInfo typeOf");

            var typeOfValidationRule = typeof(ValidationRuleActionInternal);
            var defaultRuleInstance = Activator.CreateInstance(typeOfValidationRule, action, attributeRequired);

            MethodInfo hasRequiredAttribute = typeOfValidationRule
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(method => method.Name ==  "HasRequiredAttribute" && method.IsPrivate);

            var isValid = (bool)hasRequiredAttribute.Invoke(defaultRuleInstance, new object[] { });
            return isValid;
        }
    }
}
