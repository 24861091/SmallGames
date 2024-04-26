using System;
using System.Collections;
using UnityEngine;

namespace Code.Core.Coroutine
{
    public class CoroutineBehavior : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        public new void StartCoroutine(IEnumerator enumerator)
        {
            base.StartCoroutine(enumerator);
        }

        public new void StopCoroutine(IEnumerator enumerator)
        {
            base.StopCoroutine(enumerator);
        }
    }
}
