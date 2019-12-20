using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeeWay.Ensure.ControllerAttributes.Public;
using Microsoft.AspNetCore.Mvc;

namespace LeeWay.Ensure.ControllerAttributes.Internal.Rules
{
    internal sealed class ActionRuleConfiguredByUser<TController> : ValidationRuleConfiguredByUserBase<TController> where TController : ControllerBase
    {
        /// <summary>
        /// Simple action rule
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="attributeRequired"></param>
        public ActionRuleConfiguredByUser(string actionName, Attribute attributeRequired)
            : base(GetMethodInfo(actionName), attributeRequired)
        {
        }



        /// <summary>
        /// Action rule with parameter(s) to find correct action
        /// <br/>
        /// useful when more than one action has the same name
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="parameters"></param>
        /// <param name="attributeRequired"></param>
        public ActionRuleConfiguredByUser(string actionName, IEnumerable<MyParameterInfo> parameters, Attribute attributeRequired)
            : base(GetMethodInfo(actionName, parameters), attributeRequired)
        {
        }

        /// <summary>
        /// Returns First method found by name
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        private static MethodInfo GetMethodInfo(string actionName)
        {
            var controllerInfo = new ControllerInfo(typeof(TController));
            return controllerInfo.ActionFirstOrDefault(actionName);
        }

        /// <summary>
        /// Returns methodInfo found by actionName and parameters
        /// <br/>
        /// Throws exception if action was not found
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="parameterInfosFromConfig"></param>
        /// <returns></returns>
        private static MethodInfo GetMethodInfo(string actionName, IEnumerable<MyParameterInfo> parameterInfosFromConfig)
        {
            var allActions = ControllerInfo.Actions(typeof(TController));

            foreach (var methodInfo in allActions)
            {
                var parameterInfos = ParameterInfosFromAction(methodInfo);

                //'All' must be preceded with a check for 0 parameterInfos,
                // since All is always true if the list is empty (memory from HM =) )
                if (!parameterInfos.Any() )
                {
                    continue;
                }
                
                var isMatch = parameterInfos.All(param =>
                    parameterInfosFromConfig.Any(MatchParameterNameAndType(param)));

                if (isMatch)
                {
                    return methodInfo;
                }

            }

            //action not found >
            var parameterNames = parameterInfosFromConfig.Select(parameter => parameter.Name).ToArray();
            var parametersInText = string.Join(",", parameterNames);

            throw new ArgumentException(
                $"could not find action '{actionName}' with parameters {parametersInText}, check configured rules");
        }

        private static List<ParameterInfo> ParameterInfosFromAction(MethodInfo methodInfo)
        {
            var parameterInfos = methodInfo.GetParameters().ToList();
            return parameterInfos;
        }

        private static Func<MyParameterInfo, bool> MatchParameterNameAndType(ParameterInfo parameterInfo)
        {
            return myParameterInfo =>
                myParameterInfo.Name == parameterInfo.Name
                && myParameterInfo.Type == parameterInfo.ParameterType;
        }
    }
}