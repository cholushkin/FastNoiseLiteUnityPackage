using UnityEditor;
using UnityEngine;

namespace QCat.FastNoiseLite.Samples.Editor
{
    /// <summary>
    /// Adds a "Regenerate Now" button above every Noise Examples component's default Inspector,
    /// so users don't need to know about the right-click "Regenerate" context menu item to
    /// manually refresh an example when <see cref="NoiseExampleBase.LiveUpdate"/> is off.
    /// </summary>
    [CustomEditor(typeof(NoiseExampleBase), true)]
    public class NoiseExampleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var example = (NoiseExampleBase)target;
            if (GUILayout.Button("Regenerate Now"))
            {
                example.Regenerate();
            }

            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}
