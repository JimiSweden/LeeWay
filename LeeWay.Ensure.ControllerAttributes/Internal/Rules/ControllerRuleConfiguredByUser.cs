using System;
using Microsoft.AspNetCore.Mvc;

namespace LeeWay.Ensure.ControllerAttributes.Internal.Rules
{
    internal sealed class ControllerRuleConfiguredByUser<TController> : ValidationRuleConfiguredByUserBase<TController> where TController : ControllerBase
    {
        /// <summary>
        /// Creates a rule for a controller that is applied on all it's actions
        /// <br/>
        /// (configured action rules will overwrite the ones matching actions in this controller)
        /// </summary>
        /// <param name="attributeRequired"></param>
        public ControllerRuleConfiguredByUser(Attribute attributeRequired): base(attributeRequired)
        {
        }
    }
}