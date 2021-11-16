using System;
using System.Collections.Generic;
using UnityEngine;

namespace Invoke
{
    public abstract class ManageInvokeCommand
    {
        public abstract ManageInvokeCommand DoCommand(Dictionary<object, List<CallbackAction>> invokes);
    }
}