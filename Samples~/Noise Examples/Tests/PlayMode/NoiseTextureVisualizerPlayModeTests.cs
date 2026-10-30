using System.Collections;
using NUnit.Framework;
using QCat.FastNoiseLite.Samples;
using UnityEngine;
using UnityEngine.TestTools;

namespace QCat.FastNoiseLite.Samples.Tests
{
    /// <summary>
    /// Covers "texture generation and lifecycle": in Play Mode, Apply(...) must create a real
    /// Texture2D sized to the requested resolution, resizing must replace (not leak) the previous
    /// texture, and destroying the GameObject must not leave the texture alive.
    /// </summary>
    public class NoiseTextureVisualizerPlayModeTests
    {
        [UnityTest]
        public IEnumerator Apply_CreatesTextureOfRequestedResolution()
        {
            var go = new GameObject("NoiseTextureVisualizerPlayModeTest", typeof(MeshRenderer));
            go.AddComponent<MeshFilter>().sharedMesh = new Mesh();
            var visualizer = go.AddComponent<NoiseTextureVisualizer>();
            yield return null;

            var noise = new float[16, 16];
            visualizer.Apply(noise);

            var material = go.GetComponent<Renderer>().material;
            var texture = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : material.GetTexture("_MainTex");

            Assert.IsNotNull(texture);
            Assert.AreEqual(16, texture.width);
            Assert.AreEqual(16, texture.height);

            Object.Destroy(go);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Apply_WithDifferentResolution_ReplacesPreviousTexture()
        {
            var go = new GameObject("NoiseTextureVisualizerPlayModeTest", typeof(MeshRenderer));
            go.AddComponent<MeshFilter>().sharedMesh = new Mesh();
            var visualizer = go.AddComponent<NoiseTextureVisualizer>();
            yield return null;

            visualizer.Apply(new float[8, 8]);
            var material = go.GetComponent<Renderer>().material;
            var firstTexture = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : material.GetTexture("_MainTex");

            visualizer.Apply(new float[32, 32]);
            var secondTexture = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : material.GetTexture("_MainTex");

            Assert.AreNotSame(firstTexture, secondTexture);
            Assert.IsTrue(firstTexture == null || !ReferenceEquals(firstTexture, secondTexture));
            Assert.AreEqual(32, secondTexture.width);

            Object.Destroy(go);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnDestroy_DestroysOwnedTexture()
        {
            var go = new GameObject("NoiseTextureVisualizerPlayModeTest", typeof(MeshRenderer));
            go.AddComponent<MeshFilter>().sharedMesh = new Mesh();
            var visualizer = go.AddComponent<NoiseTextureVisualizer>();
            yield return null;

            visualizer.Apply(new float[8, 8]);
            var material = go.GetComponent<Renderer>().material;
            var texture = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : material.GetTexture("_MainTex");

            Object.Destroy(go);
            yield return null;

            Assert.IsTrue(texture == null);
        }
    }
}
