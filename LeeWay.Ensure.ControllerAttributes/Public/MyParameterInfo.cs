using System;
using System.Reflection;

namespace LeeWay.Ensure.ControllerAttributes.Public
{
    /// <summary>
    /// Simplified version of ParameterInfo
    /// <br/>
    /// Used in action rules where matching action on signature is required
    /// <br/>
    /// (i.e. when multiple actions in a controller have the same name)
    /// </summary>
    public sealed class MyParameterInfo
    {
        public MyParameterInfo()
        {
        }

        public MyParameterInfo(ParameterInfo parameterInfo)
        {
            Name = parameterInfo.Name;
            Type = parameterInfo.ParameterType;
        }
        public string Name { get; set; }
        public Type Type { get; set; }

    }
}