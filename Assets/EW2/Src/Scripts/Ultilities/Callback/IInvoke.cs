using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Invoke
{

    public interface IInvoke
    {
        /// <summary>
        /// Cancels all Invoke calls on this MonoBehaviour.
        /// </summary>
        /// <returns><c>true</c> if this instance cancel invoke; otherwise, <c>false</c>.</returns>
        void CancelInvoke(object caller);

        /// <summary>
        /// Cancels all Invoke calls with name methodName on this behaviour.
        /// </summary>
        void CancelInvoke(object caller, Action method);

        /// <summary>
        /// Invokes the method methodName in time seconds.
        /// </summary>
        void Invoke(object caller, Action method, float time);

        /// <summary>
        /// Invokes the method methodName in time seconds, then repeatedly every repeatRate seconds.
        /// </summary>
        void InvokeRepeating(object caller, Action method, float time, float repeatRate);
    }
}