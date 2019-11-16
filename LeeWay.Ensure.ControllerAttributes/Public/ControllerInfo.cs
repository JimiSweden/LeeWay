using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    /// <summary>
    /// Used to find actions (MethodInfo) on controllers
    /// </summary>
    public sealed class ControllerInfo
    {
        public Type Controller;

        public ControllerInfo(Type controller)
        {
            Controller = controller;
        }

        public IEnumerable<MethodInfo> Actions()
        {
            var methods = Controller.GetMethods().Where(m => m.DeclaringType == Controller);
            return methods;
        }

        /// <summary>
        /// Returns all actions from a given controller
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> Actions(Type controller) 
        {
            var methods = controller.GetMethods().Where(m => m.DeclaringType == controller);
            return methods;
        }

        /// <summary>
        /// Returns the first found Action matching action name.
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public MethodInfo ActionFirstOrDefault(string actionName)
        {
            var method = Controller.GetMethods().FirstOrDefault(m => m.Name == actionName);
            return method;
        }

    }
}