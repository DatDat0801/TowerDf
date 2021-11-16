using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invoke
{
    public class InvokeProxy : MonoBehaviour
    {
        public static IInvoke Iinvoke { get { return InvokeCallbackUtils.Instance; } }
    }
}