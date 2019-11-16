using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Public;
using Microsoft.AspNetCore.Mvc;
using Xunit.Sdk;

namespace LeeWay.Ensure.ControllerAttributes.Internal
{
    /// <summary>
    /// Used to get actions and information from all controllers in given assembly
    /// <br/>
    /// (all classes of type Controller/ControllerBase)
    /// </summary>
    internal sealed class ControllersFromAssembly : IControllersFromAssembly
    {
        private readonly List<Type> _allControllersInAssembly;
        
        public ControllersFromAssembly(string assemblyName)
        {
            var assembly = LoadAssembly(assemblyName);

            _allControllersInAssembly = assembly.GetTypes()
                //Note: typeof(Controller) can be used for view controllers, but api controllers are of type ControllerBase
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .ToList();

            //assert controllers are loaded
            if (!_allControllersInAssembly.Any())
            {
                throw new XunitException(
                    $" Could not find any controllers in assembly {assemblyName} " +
                    $"- if no controllers are loaded something is wrong in setup or you have not controllers in your project");
            }
        }

        private Assembly LoadAssembly(string assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new XunitException($"Could not load assembly  {assemblyName} - reason :  {e.Message}");
            }
        }

        /// <summary>
        /// All actions from all controllers in given assembly
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MethodInfo> AllActions()
        {
            var allActions = new List<MethodInfo>();
            foreach (var controller in _allControllersInAssembly)
            {
                var methods = controller.GetMethods()
                   .Where(m => m.DeclaringType == controller);

                allActions.AddRange(methods);
            }
            return allActions;
        }

        /// <summary>
        /// Get info for a single controller
        /// <br/>
        /// Use it to find actions on controller
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public ControllerInfo ControllerInfo(string controllerName)
        {
            var controller = _allControllersInAssembly.FirstOrDefault(c => c.Name == controllerName);
            return new ControllerInfo(controller);
        }

    }
}
