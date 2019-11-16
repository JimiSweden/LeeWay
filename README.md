# LeeWay - freedom by constraints.
Made to ensure Your code behaves as You intended.
<br/>
With easy to use unit test configurations.

*C# Web API Controller Authorization Attributes xUnit unit test*

## LeeWay.Ensure.ControllerAttributes
Use it to Ensure Controllers and Controller Actions has correct Attributes
Currently supported attributes are AllowAnonymousAttribute and AuthorizeAttribute (with or without policy)

    note: this is not yet added as a nuget package but will be

### Supported plattforms
**LeeWay.Ensure.ControllerAttributes targets .NET Standard 2.0**
and hence supports the following plattforms **(including later versions such as .NET Core 3)**
- .NET Core 2.0
- .NET Framework 4.6.1
*https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md#platform-support*


    *the demo project "WebApi" and its test project targets netcoreapp3.0*


### Example of how to configure the validation for Authorize attributes
Note that the below code is found in the demo project "WebApi.ConfigurationTests" 

- The project "WebApi" is the "production assembly" under test.
- WebApi.Controllers are configured with different attributes etc to support the example configuration below

```csharp
//add project dependency to LeeWay.Ensure.ControllerAttributes;

 public class WebApi_Ensure
    {
        private readonly ITestOutputHelper _outputHelper;

        /// <summary>
        /// This is an example for how to configure the test rules
        /// <br/>
        /// Here we use the TestOutputHelper to print validation results in the test output console,
        ///  to see the settings and validation result for all controller actions
        /// </summary>
        /// <param name="outputHelperHelper">Xunit.Abstractions.ITestOutputHelper</param>
        public WebApi_Ensure(ITestOutputHelper outputHelperHelper)
        {
            _outputHelper = outputHelperHelper;
        }

        /// <summary>
        /// This is how you configure the rules for authorization attributes
        /// (Authorize and AllowAnonymous)
        /// </summary>
        [Fact]
        public void Controllers_and_actions_have_correct_Authorization()
        {
            /*
             * create the validator builder
             * - with the assembly under test; in this case 'WebApi'
             * - with the default required attribute
             *  (the attribute that will be required on all controller actions without explicit rules configured)
             */
            var assemblyName = nameof(WebApi);
            var defaultAttributeRequired = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);
            
            var validatorBuilder = new ValidatorBuilder(assemblyName, defaultAttributeRequired);

            /*
             * It is possible to exclude controllers from validation with path
             * This can be used to exclude a folder/namespace path or single controller (FullName)
             * read the summary of the method
             */
            var excludeControllersInPath = typeof(ExcludedFromValidationController).FullName;

            /*
             * The builder returns a validator that is used to trigger validation of configured rules
             * From the validator you can
             * - get all validation results as objects
             * - print all results in unit test output
             * - throw on failure (prints all invalid results in test output)
             */
            var validator = validatorBuilder
                
                //exclude a controller from validation
                .ExcludeControllersInPath(excludeControllersInPath)

                //add a controller rule
                .AddControllerRule<AccountController>(new AllowAnonymousAttribute())

                // add an action rule;
                // note that action rules takes presence over controller rules, always.
                // If you have more than one action with the same name
                // this rule will only be applied to the first one found in the controller
                .AddActionRule<AccountController>(
                    nameof(AccountController.Get),
                    new AuthorizeAttribute("MyPolicyToGetAll"))

                //add an action rule matching on parameters
                // my recommendation is that you don't do this, instead name the actions differently =)
                .AddActionRule<AccountController>(
                    nameof(AccountController.Get),
                    new MyParameterInfo
                    {
                        Name = "id",
                        Type = typeof(int)
                    },
                    new AuthorizeAttribute("MyPolicyToGetSingle"))

                .AddActionRule<AccountController>(
                    nameof(AccountController.Delete),
                    new AuthorizeAttribute("MyPolicyToDelete"))

                //note: this action name, GetWithSimpleRule, can match two actions in the controller
                // actions will always (?) be ordered as they appear in the controller (top down)
                .AddActionRule<AccountController>(
                    nameof(AccountController.GetWithSimpleRule),
                    new AuthorizeAttribute("MyPolicyToGetWithSimpleRule"))

                .AddActionRule<AccountController>(
                    nameof(AccountController.Post),
                    new List<MyParameterInfo>
                    {
                        new MyParameterInfo {Name = "command", Type = typeof(AccountController.CreateSomethingCommand)}
                    },
                    new AuthorizeAttribute("MyPolicyToPost"))


                .AddControllerRule<UserController>(new AuthorizeAttribute(PolicyNames.RequireAuthorizedUser))


                .AddControllerRule<PublicController>(new AllowAnonymousAttribute())

                .AddActionRule<PublicController>(
                    nameof(PublicController.Get),
                    new MyParameterInfo
                    {
                        Name = "id",
                        Type = typeof(int)
                    },
                    new AuthorizeAttribute("MyPolicy"))

                .AddActionRule<PublicController>(
                    nameof(PublicController.Get),
                    new AuthorizeAttribute("MyPolicy"))

                .AddActionRule<PublicController>(
                    nameof(PublicController.Delete),
                    new AuthorizeAttribute("DeletePolicy"))

                
                //configuration is completed
                .Build();

            //run the tests and throw on failed validation, prints all failed results
            validator.ValidateAndThrowOnError();
            
            //print all results in test output no matter if validation is successful or not.
            validator.PrintValidatedResults(_outputHelper);

            //get the results and do what you like with them
            var results = validator.ValidatedResults();
        }
    }
```