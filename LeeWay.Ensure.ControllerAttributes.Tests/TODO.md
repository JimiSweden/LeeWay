#Inspiration of other validation projects - perhaps to use in a combination with this project.

    ## creating rules and validations for objects - https://github.com/wolf8196/FluentObjectValidator
    - example:  Property(x => x.Title).IsRequired().HasRule(x => x.Length <= 10);

    ## similar to FluentObjectValidator (to read and be inspired of) - https://github.com/twistedtwig/AutoValidator
    
    ## inspiration for appsettings validation https://github.com/tidm/ApplicationIntegrityValidator

	## Really nice: https://github.com/JeremySkinner/fluentvalidation
	A small validation library for .NET that uses a fluent interface and lambda expressions for building validation rules.	
	NOTE: this has 122 contributors and lots of functionality - perhaps something to use as foundation?
	https://fluentvalidation.net/
	- Probably use this to validate appsettings
		example: Conditions https://fluentvalidation.net/start#conditions
		The When and Unless methods can be used to specify conditions that control when the rule should execute. 
		For example, this rule on the CustomerDiscount property will only execute when IsPreferredCustomer is true:
				RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).When(customer => customer.IsPreferredCustomer);


#Naming
Brand( aka "Umbrella" name): LeeWay
Product/Sub Brand : LeeWay Ensure
- subname : LeeWay.Ensure.ControllerAttributes ; validates Authorize and AllowAnonymous attributes
- subname : LeeWay.Ensure.RuntimeEnvironment; (not implemented; ensure correct environment in appsettings during startup, if overwritte in build etc)
- subname : LeeWay.Ensure.Appsettings; (not implemented; ensure properties in appsettings, prop name and optional value or regex)

 
 	
# TODO: first this.. 
[] - (allways) cleanup and make more readable when possible 

[] fix summary and help texts : https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/how-to-use-the-xml-documentation-features
 (partly done)
 
[ ] IValidationRule, ValidationRuleActionDefault, ValidationRuleBase
    [] cleanup , remove unused props in validation rules
    [] perhaps break dependencies 
    [] loosen up inheritance, to avoid the Validate and perhaps other unused props.

[] ValidationBuilder
	[] test to implement Action-based configuration (in Builder.Add... methods)
	- AddAction(Action<ActionRuleBuilder> actionRuleBuilder)
	: use like this: 
		builder.AddAction(rule => 
		{
			//perhaps have standard attributes as predefined methods , i e RequireAuthorizeAttribute
			rule.RequireAttribute(new AuthorizeAttribute())
			rule.ActionName("action name")
			rule.MatchParameters('enumerable of methodinfo')
		})

[] in ConfigurationIntegrationTest
	[] cleanup
	[] read todos and validate ideas.



[] in ValidationRuleActionDefault
  [] add validation of Attribute type to constraint only supported	( in constructor)
  [] maybe? make default rule class transform to the type, controller/action, authorize/anonymous
   
[] in  ValidationRuleActionDefaultTest.ValidationRule_HasRequiredAttribute
	[] probably make HasRequiredAttribute public and make test cleaner.
	[] (later.) refactoring consolidate validation rules

[] in Validator
	[] test if it feels better to have each rule containing its own "Exception Message" after validation.
       - meaning.. move the Should.. and Catch into "Validate" in the DefaultRule. 

[] 	ValidationResult 
	[] - need information of the validated controller and action,
    OR. keep the ValidationResult in the validation rule where everything is
	(now action is added to ValidationResult, but ValidationResult should probably live inside the rule insted)

[] ValidationResult , (probably) create Success and Fail sub classes

[] ValidatorBuilder , (probably) Allow default attribute (DefaultAuthorizeAttribute) to be "none"



#TODO: (after github published)
[] nuget package as a dotnetstandad project
	new projects created.
	[] - try to remove dependencies as much as possible, like FluentAssertions in the main project
	[] - Let WebApi project live as a demo app. 
	- Being called by WebApiTests using the main project


[] Add as middleware for authorization validation during startup
   to abort the application during startup if configurations are missing or invalid
   note, this perhap should have a toggle for enable / disable in case we want to allow the configuration missmatch and still run the app (without need to recompile)
  	Make it possible to configure rules in json, in appsettings or in a separate file, to allow "hot swap" of rules

[] Think about, wait and implement on request 
	[] maybe: allow a controller to be excluded from validation?
	[] maybe later: validation rules should probably work for all types of System.Attributes later.. but keep simple for now.
	[] maybe later- AuthorizationValidator > Validate functions can be made to Handlers
			a Handler will Act on the matching Rule that is being validated
	[] probably/perhaps allow to set "no" attribute as the default? (in ValidatorBuilder.WithDefaultAuthorizeAttribute)


[] maybe - Investigate/Lab on this
	A comment from ValidationRuleBase.GetMethodInfo(actionname. parameterInfosFromConfig)
	TODO ? 
	Note: if this would work correctly, getting the real action from controller
	then both
	- the real action with its attributes
	- and the required attribute
	exists in this rule, this would then make it possible to execute validation from this rule
	without merging..
	todo: BUT!! what about the ones created from controller rule?
	- should work but would be harder to implement
	-  as it would need to know if any action rules for the same controller has been

[] Specflow - as configuration?.
- because it would be nice to read the confiuration and easy to talk about with non-programming people
 	- Given (defaults)
	- when rules, 
	- then.. validation


[] (maybe) add posibility to use as middleware in startup, to hinder the application from starting if configuration is not matching controllers policies
	https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.0

## Appsettings validation
(note my comment about using https://fluentvalidation.net/ for this)
[] add separate project for asserting values in appsettings file during startup matches rules (naming standards etc) for the environment
- to make sure transformed appsettings are not pointing to incorrect environment, ex in dev all paths should contain dev, in prod no path should contain dev, test, qa etc..
- also: make it possible to add json file as configuration (to enable "hot" swapping rules without rebuilding test project
[] create a bundle of the appsettings validation and controller authorization validation

- RuntimeEnvironment (ensure correct environment in appsettings during startup, if overwritte in build etc)
- Appsettings (ensure properties in appsettings, prop name and optional value or regex)


## maybe maybe..
[] maybe: add separate project for enforcing APIs to take maximum one(or n) parameter(s) 
    -  and to enforce parameter to be of type T or interface or class : fex where T : CommandBase

 - make it possible to add json file as configuration (to enable "hot" swapping rules without rebuilding test project
  (especially valid use for the appsettings validation)

### "perhaps later "
 [] extend controller validator with possiblity to set defaults on path to controller
 - f ex webapi.controllers.admin.MyAdminController , where path containing "admin" could have one default configuration

## Done 

(done until 2019-12-20)
- bugfix , multiple controllers using the same name in the assembly works now.
- (cleanup) separate interface for user configured rules from the internal validation rule


(done until 2019-11-24)
- [x] update WebApi to dotnet core 3.0
- [x] refactor ValidatedResults (use constructor to populate with all results)
- [x] fix sorting of validated results , order by message (message starts with controller name and action name)
- [x] add assembly name to printed results and execute tests
- [x] sealed classes not intended for inheritence, and internal where not set already.

(done until 2019-11-23)

- [partly done] - fix summary and help texts
- [x]- add Readme in root.
- [x] Set a product name and name projects to reflect what they do, (see at top)
- - because the AuthorizeAttribute validation could be extended to include other attributes on controller the project is named ControllerAttributes to "allow" growth. perhaps a good desicion and perhaps not =)


(done until 2019-11-11)
- [x] - make sure there is a small test validating each of the required attributes, 
	like if Anonymous on Class and a method is set with Authorize but is not configured for that.
	note: do it in "ValidationRuleDefaultTest"

- [x] copy the "integration test" - ImplementationConfigurationForUserOfAuthorizationValidationModule
	into the test project (also copy all controllers/actions from WebApi to Fake/Controllers)
	named "ConfigurationIntegrationTest"

- [x] fix messages in rules passing validation, should only be 
	"pass : AccountController.GetWithSimpleRule(Int32 id) - attributes: [AllowAnonymous] "
	
- [x]remove the old implementation and tests targeting that.
- [x] Defect - when required 'AllowAnonymous' 
	a rule with Authorize passes
- [x]	fix "ouput in result" - "AllowAnonymous" is not printed in actual attributes
- [x]  add new tests similar to the old ControllerActionsHaveValidAuthorization

- [x] add default authorization policy to use
- [x] add validation rules for controller levels
- [x] add validation rules for action levels
         
- [x] execute the validation
- [x] collect results for all rules
- [x] throw in xunit if at least one error exists, but first after collection
- [x] print all errors in output console
	