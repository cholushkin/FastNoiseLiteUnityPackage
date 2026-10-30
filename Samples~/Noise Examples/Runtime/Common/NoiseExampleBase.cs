using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Shared regeneration lifecycle for every Noise Examples MonoBehaviour: a "Live Update"
    /// toggle, a manual "Regenerate" action usable from the Inspector context menu or the custom
    /// editor button, and edit-mode regeneration that is deferred (rather than run inline inside
    /// OnValidate) so it is safe to create/destroy Unity objects such as Texture2D or Mesh.
    /// </summary>
    public abstract class NoiseExampleBase : MonoBehaviour
    {
        [Header("Regeneration")]
        [Tooltip("Automatically regenerate when a parameter changes in the Inspector.")]
        public bool LiveUpdate = true;

        bool _regeneratePending;

        protected virtual void OnEnable()
        {
            Regenerate();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!LiveUpdate) return;
            if (_regeneratePending) return;
            _regeneratePending = true;
            UnityEditor.EditorApplication.delayCall += () =>
            {
                _regeneratePending = false;
                // The object may have been destroyed between OnValidate and the deferred call.
                if (this != null && isActiveAndEnabled) Regenerate();
            };
        }
#endif

        /// <summary>Re-runs noise generation and updates whatever this example visualizes.</summary>
        [ContextMenu("Regenerate")]
        public void Regenerate()
        {
            Generate();
        }

        /// <summary>Implemented by each example to (re)build its visualization from current settings.</summary>
        protected abstract void Generate();
    }
}
