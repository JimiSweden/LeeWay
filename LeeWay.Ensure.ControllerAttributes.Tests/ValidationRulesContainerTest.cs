using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using LeeWay.Ensure.ControllerAttributes.Internal;
using LeeWay.Ensure.ControllerAttributes.Public;
using LeeWay.Ensure.ControllerAttributes.Tests.Fakes;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using Xunit.Sdk;

// ReSharper disable InconsistentNaming

namespace LeeWay.Ensure.ControllerAttributes.Tests
{
    public class ValidationRulesContainerTest
    {
        private readonly string AssemblyNameDefault = Assembly.GetExecutingAssembly().GetName().Name;
        private IControllersFromAssembly _defaultControllersFromAssembly;
        private IEnumerable<MethodInfo> _defaultActionsFromAssemblyToValidate;

        public ValidationRulesContainerTest()
        {
            _defaultControllersFromAssembly = new ControllersFromAssembly(AssemblyNameDefault);
            _defaultActionsFromAssemblyToValidate = _defaultControllersFromAssembly.AllActions();
        }
        [Fact]
        public void Constructor_Throws_when_no_validation_rules_are_given()
        {
            //act
            Action containerAction = () => 
                new ValidationRulesContainer(
                    controllerActionsToValidate:_defaultActionsFromAssemblyToValidate,
                    validationRules: new List<IValidationRuleConfiguredByUser>()
                    );

            containerAction
                .Should()
                .Throw<XunitException>()
                .WithMessage("you must provide validation rules");
        }

        [Fact]
        public void Constructor_Throws_when_no_controller_actions_to_validate_are_given()
        {
            //act
            Action containerAction = () =>
                new ValidationRulesContainer(
                    controllerActionsToValidate: new List<MethodInfo>(), 
                    validationRules: new List<IValidationRuleConfiguredByUser> { Helper.ActionRule_withNoAttribute_Controller_hasAllowAnnonymous() }
                );

            containerAction
                .Should()
                .Throw<XunitException>()
                .WithMessage( "you must provide the controller actions to validate, what's the point of doing anything at all?(Johnossi https://open.spotify.com/track/6zu9oVexxZt6qmyGA4X7WR?si=zQ5BNkjFQQ6kqvgJ7O39RA)");
        }


        [Fact]
        public void Loads_controllers_from_given_assembly_and_adds_validation_rules()
        {
            var controllerRule = Helper.ControllerRule_with_policy_RequireAdmin();
            var actionRule = Helper.ActionRule_withNoAttribute_Controller_hasAllowAnnonymous();
            var expectedActionRules = new List<IValidationRuleConfiguredByUser> { actionRule };
            var expectedControllerRules = new List<IValidationRuleConfiguredByUser> { controllerRule };

            var validationRules = new List<IValidationRuleConfiguredByUser>
            {
                controllerRule, actionRule
            };
            
            //act
            var container = new ValidationRulesContainer(
                controllerActionsToValidate: _defaultActionsFromAssemblyToValidate,
                validationRules);


            //assert
            //note: this test also tests ControllersFromAssembly, could be moved "down", but ok now.
            container._controllerActionsToValidate
                .Count()
                .Should().BeGreaterThan(0,
                    " we load all actions from controllers in this assembly, are we missing controllers in assembly?");

            container._configuredActionMethodRules
                .Should().BeEquivalentTo(expectedActionRules);
            
            container._configuredControllerRules
                .Should().BeEquivalentTo(expectedControllerRules);
        }


        //todo: ? rewrite as class with "steps" , OR.. ? Specflow? 
        [Fact]
        public void Populates_default_rules_and_merges_with_configured_rules()
        {
            //arrange
            var controllerRule = Helper.ControllerRule_with_policy_RequireAdmin();
            var actionRule = Helper.ActionRule_withNoAttribute_Controller_hasAllowAnnonymous();

            var validationRules = new List<IValidationRuleConfiguredByUser>
            {
                controllerRule, actionRule
            };

            //arrange expects
            var expectedActionRulesFromController = ControllerInfo
                    //note: could not get type directly from controllerRule in an easy way, that is why it looks like this for now. 
                .Actions(typeof(FakeControllers.Controller_with_Policy_RequireAdmin))
                .Select(action => new ValidationRuleActionInternal(action, controllerRule.AttributeRequired));

            var expectedRules = new List<ValidationRuleActionInternal>
            {
                new ValidationRuleActionInternal(actionRule.Action, actionRule.AttributeRequired)
            };
            expectedRules.AddRange(expectedActionRulesFromController);


            //acts and asserts
            var rulesContainer = new ValidationRulesContainer(
                controllerActionsToValidate: _defaultActionsFromAssemblyToValidate,
                validationRules);
            
            rulesContainer.AllRules().Should().BeEmpty("nothing is initialized yet");

            
            //act
            rulesContainer.PopulateDefaultRules(defaultAttribute: new AuthorizeAttribute("DefaultPolicyName"));

            //assert
            var nrOfControllerActionsInAssembly = rulesContainer._controllerActionsToValidate.Count();

            rulesContainer.AllRules().Count
                .Should().Be(nrOfControllerActionsInAssembly, 
                "one rule per action is created");

            //act
            rulesContainer.MergeDefaultWithConfiguredRules();
            //assert
            rulesContainer.AllRules().Count.Should().Be(nrOfControllerActionsInAssembly,
                " since default rules are overwritten with configured rules the total count should not differ after merge");

            
            var actualRules = rulesContainer.AllRules();

            actualRules
                .Should()
                //note: Contain uses Equality comparer.
                .Contain(expectedRules
                , "configured rules should overwrite the defaults");
        }
    }
}
