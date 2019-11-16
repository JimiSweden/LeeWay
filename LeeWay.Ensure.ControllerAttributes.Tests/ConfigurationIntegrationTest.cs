using System.Collections.Generic;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Public;
using LeeWay.Ensure.ControllerAttributes.Tests.Fakes;
using LeeWay.Ensure.ControllerAttributes.Tests.Fakes.Controllers;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using Xunit.Abstractions;

namespace LeeWay.Ensure.ControllerAttributes.Tests
{
    /// <summary>
    /// This Test is used to validate the complete setup,
    /// a sanity check to assert the complete picture is ok.
    /// as a user would configure the rules for authorization
    /// </summary>
    public class ConfigurationIntegrationTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly string _assemblyName ;

        /// <summary>
        /// Here we use the TestOutputHelper to print validation results in the test output console,
        ///  to see the settings and validation result for all controller actions
        /// </summary>
        /// <param name="outputHelperHelper">Xunit.Abstractions.ITestOutputHelper</param>
        public ConfigurationIntegrationTest(ITestOutputHelper outputHelperHelper)
        {
            _outputHelper = outputHelperHelper;
            _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        }
        
        /// <summary>
        /// see summary and comments in WebApi test
        /// </summary>
        [Fact]
        public void ValidatorBuilder_integration_test_with_example_configuration()
        {
            var defaultAuthorizeAttribute = new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin);

            var validatorBuilder = new ValidatorBuilder(_assemblyName, defaultAuthorizeAttribute);
            
            var excludeControllersInPathFakeControllers = typeof(FakeControllers).FullName;
            
            var validator = validatorBuilder
                
                .ExcludeControllersInPath(pathContains: $"{excludeControllersInPathFakeControllers}")

                .AddControllerRule<AccountController>(new AllowAnonymousAttribute())

                .AddActionRule<AccountController>(
                    nameof(AccountController.Get),
                    new AuthorizeAttribute("MyPolicyToGetAll"))

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

                .Build();


            //get all results to do what you want with
            var validatedResults = validator.ValidatedResults();

            //Debugger.Break();
            //execute tests and fail on error
            //TODO? rename to ValidateAndThrowOnError ? 
            validator.ValidateAndThrowOnError();


            //print all results in test output no matter if validation is successful or not.
            validator.PrintValidatedResults(_outputHelper);

        }
    }
}
