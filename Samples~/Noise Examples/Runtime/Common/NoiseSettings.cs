using System;
using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Serializable mirror of every FastNoiseLite configuration knob, shared by all
    /// Noise Examples so each example only needs to expose the parameters it actually
    /// demonstrates. Not every field is relevant to every noise/fractal/domain-warp
    /// combination — see the FastNoiseLite documentation for which settings apply to which mode.
    /// </summary>
    [Serializable]
    public class NoiseSettings
    {
        [Header("General")]
        public int Seed = 1337;
        public float Frequency = 0.02f;
        public global::FastNoiseLite.NoiseType NoiseType = global::FastNoiseLite.NoiseType.OpenSimplex2;
        public global::FastNoiseLite.RotationType3D RotationType3D = global::FastNoiseLite.RotationType3D.None;

        [Header("Fractal")]
        public global::FastNoiseLite.FractalType FractalType = global::FastNoiseLite.FractalType.None;
        [Min(1)] public int Octaves = 3;
        public float Lacunarity = 2.0f;
        [Range(0f, 1f)] public float Gain = 0.5f;
        [Range(0f, 1f)] public float WeightedStrength = 0.0f;
        public float PingPongStrength = 2.0f;

        [Header("Cellular")]
        public global::FastNoiseLite.CellularDistanceFunction CellularDistanceFunction = global::FastNoiseLite.CellularDistanceFunction.EuclideanSq;
        public global::FastNoiseLite.CellularReturnType CellularReturnType = global::FastNoiseLite.CellularReturnType.Distance;
        [Range(0f, 1f)] public float CellularJitter = 1.0f;

        [Header("Domain Warp")]
        public global::FastNoiseLite.DomainWarpType DomainWarpType = global::FastNoiseLite.DomainWarpType.OpenSimplex2;
        public float DomainWarpAmplitude = 30.0f;

        /// <summary>Builds a FastNoiseLite instance configured for GetNoise(...) sampling.</summary>
        public global::FastNoiseLite CreateNoise()
        {
            var noise = new global::FastNoiseLite(Seed);
            noise.SetFrequency(Frequency);
            noise.SetNoiseType(NoiseType);
            noise.SetRotationType3D(RotationType3D);
            noise.SetFractalType(FractalType);
            noise.SetFractalOctaves(Octaves);
            noise.SetFractalLacunarity(Lacunarity);
            noise.SetFractalGain(Gain);
            noise.SetFractalWeightedStrength(WeightedStrength);
            noise.SetFractalPingPongStrength(PingPongStrength);
            noise.SetCellularDistanceFunction(CellularDistanceFunction);
            noise.SetCellularReturnType(CellularReturnType);
            noise.SetCellularJitter(CellularJitter);
            return noise;
        }

        /// <summary>
        /// Builds a FastNoiseLite instance configured for DomainWarp(...) followed by GetNoise(...)
        /// on the same instance. FastNoiseLite's DomainWarp(...) reads the same Frequency as
        /// GetNoise(...), so both the warp and the final sample share this instance's Frequency.
        /// </summary>
        public global::FastNoiseLite CreateDomainWarpNoise()
        {
            var noise = CreateNoise();
            noise.SetDomainWarpType(DomainWarpType);
            noise.SetDomainWarpAmp(DomainWarpAmplitude);
            return noise;
        }
    }
}
