using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Core.Coroutine
{
    public class CoroutineComponent : GameComponent.GameComponent
    {
        private CoroutineBehavior _behavior;
        protected override void Awake()
        {
            _behavior = GetCoroutineBehaviour();
        }

        private CoroutineBehavior GetCoroutineBehaviour()
        {
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            if (roots == null)
            {
                var o = new GameObject();
                o.GetOrAddComponent<CoroutineBehavior>();
            }
            foreach (var root in roots)
            {
                if (root.name == "CoroutineComponent")
                {
                    return root.GetOrAddComponent<CoroutineBehavior>();
                }
            }

            var r = new GameObject();
            return r.GetOrAddComponent<CoroutineBehavior>();
        }

        public void StartCoroutine(IEnumerator enumerator)
        {
            _behavior.StartCoroutine(enumerator);
        }
        
        public void StopCoroutine(IEnumerator enumerator)
        {
            _behavior.StopCoroutine(enumerator);
        }

    }
}
