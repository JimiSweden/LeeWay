using System.Collections.Generic;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Public;


namespace LeeWay.Ensure.ControllerAttributes.Internal
{
    internal interface IControllersFromAssembly
    {
        /// <summary>
        /// All actions from all controllers in given assembly
        /// </summary>
        /// <returns></returns>
        IEnumerable<MethodInfo> AllActions();

        /// <summary>
        /// Get info for a single controller in given assembly
        /// <br/>
        /// Use it to find actions on controller
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        ControllerInfo ControllerInfo(string controllerName);
    }
}