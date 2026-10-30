using NUnit.Framework;
using UnityEngine;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class Noise3DExampleTests
    {
        [Test]
        public void BuildSliceTexture_XY_MatchesDirectSampling()
        {
            var settings = new NoiseSettings();
            float slicePosition = 0f;
            var texture = Noise3DExample.BuildSliceTexture(settings, Noise3DExample.SlicePlane.XY, slicePosition, 4);
            var noise = settings.CreateNoise();

            for (int b = 0; b < 4; b++)
                for (int a = 0; a < 4; a++)
                    Assert.AreEqual(noise.GetNoise(a, b, slicePosition), texture[a, b]);
        }

        [Test]
        public void BuildSliceTexture_XZ_MatchesDirectSampling()
        {
            var settings = new NoiseSettings();
            float slicePosition = 3f;
            var texture = Noise3DExample.BuildSliceTexture(settings, Noise3DExample.SlicePlane.XZ, slicePosition, 4);
            var noise = settings.CreateNoise();

            for (int b = 0; b < 4; b++)
                for (int a = 0; a < 4; a++)
                    Assert.AreEqual(noise.GetNoise(a, slicePosition, b), texture[a, b]);
        }

        [Test]
        public void BuildSliceTexture_YZ_MatchesDirectSampling()
        {
            var settings = new NoiseSettings();
            float slicePosition = -2f;
            var texture = Noise3DExample.BuildSliceTexture(settings, Noise3DExample.SlicePlane.YZ, slicePosition, 4);
            var noise = settings.CreateNoise();

            for (int b = 0; b < 4; b++)
                for (int a = 0; a < 4; a++)
                    Assert.AreEqual(noise.GetNoise(slicePosition, a, b), texture[a, b]);
        }

        [Test]
        public void BuildSliceTexture_DifferentSlicePositions_VaryAlongSlicedAxis()
        {
            var settings = new NoiseSettings();
            var textureA = Noise3DExample.BuildSliceTexture(settings, Noise3DExample.SlicePlane.XY, 0f, 4);
            var textureB = Noise3DExample.BuildSliceTexture(settings, Noise3DExample.SlicePlane.XY, 25f, 4);

            bool foundDifference = false;
            for (int b = 0; b < 4 && !foundDifference; b++)
                for (int a = 0; a < 4 && !foundDifference; a++)
                    if (!Mathf.Approximately(textureA[a, b], textureB[a, b]))
                        foundDifference = true;

            Assert.IsTrue(foundDifference);
        }
    }
}
