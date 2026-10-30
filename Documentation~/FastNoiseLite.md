# FastNoiseLite — API Reference & Notes

This folder holds extended documentation that isn't meant to ship as a runtime asset (the `~`
suffix excludes it from Unity's asset database). For install and quick-start instructions, see the
package [README](../README.md). For sample-specific walkthroughs, see
[`Samples~/Noise Examples/README.md`](../Samples~/Noise%20Examples/README.md).

## Core type

Everything lives on a single class, `FastNoiseLite` (global namespace, no `using` required).
Create an instance, configure it with the `Set*` methods below, then sample it with `GetNoise`.
Configuration is mutable state on the instance — it is not thread-safe to mutate and sample the
same instance concurrently, but reading via `GetNoise`/`DomainWarp` from multiple threads on an
already-configured, unchanging instance is fine.

### Construction

| Member | Notes |
|---|---|
| `FastNoiseLite(int seed = 1337)` | Default constructor. |
| `FastNoiseLite(int seed, float frequency, int octaves, float lacunarity, float gain)` | Convenience constructor, not part of upstream FastNoiseLite. Configures `NoiseType.OpenSimplex2` + `FractalType.FBm` in one call, mirroring the HLSL `fnlCreateState(seed, frequency, octaves, lacunarity, gain)` signature used by Terraxel's generated compute shaders. |

### Configuration

| Method | Default | Notes |
|---|---|---|
| `SetSeed(int)` | 1337 | |
| `SetFrequency(float)` | 0.01 | |
| `SetNoiseType(NoiseType)` | OpenSimplex2 | `OpenSimplex2`, `OpenSimplex2S`, `Cellular`, `Perlin`, `ValueCubic`, `Value` |
| `SetRotationType3D(RotationType3D)` | None | `None`, `ImproveXYPlanes`, `ImproveXZPlanes` — reduces directional artifacts when sampling a 2D plane through 3D noise |
| `SetFractalType(FractalType)` | None | `None`, `FBm`, `Ridged`, `PingPong` affect `GetNoise`; `DomainWarpProgressive`, `DomainWarpIndependent` affect `DomainWarp` only |
| `SetFractalOctaves(int)` | 3 | |
| `SetFractalLacunarity(float)` | 2.0 | frequency multiplier per octave |
| `SetFractalGain(float)` | 0.5 | amplitude multiplier per octave |
| `SetFractalWeightedStrength(float)` | 0.0 | keep in `0..1` to preserve `-1..1` output bounds |
| `SetFractalPingPongStrength(float)` | 2.0 | only affects `FractalType.PingPong` |
| `SetCellularDistanceFunction(CellularDistanceFunction)` | EuclideanSq | `Euclidean`, `EuclideanSq`, `Manhattan`, `Hybrid` |
| `SetCellularReturnType(CellularReturnType)` | Distance | `CellValue`, `Distance`, `Distance2`, `Distance2Add`, `Distance2Sub`, `Distance2Mul`, `Distance2Div` |
| `SetCellularJitter(float)` | 1.0 | values above 1 cause visible artifacts |
| `SetDomainWarpType(DomainWarpType)` | OpenSimplex2 | `OpenSimplex2`, `OpenSimplex2Reduced`, `BasicGrid` |
| `SetDomainWarpAmp(float)` | 1.0 | max warp distance from the original position |

### Sampling

| Method | Returns |
|---|---|
| `GetNoise(float x, float y)` | noise in roughly `-1..1` |
| `GetNoise(float x, float y, float z)` | noise in roughly `-1..1` |
| `DomainWarp(ref float x, ref float y)` | warps the position in place; follow with `GetNoise(x, y)` |
| `DomainWarp(ref float x, ref float y, ref float z)` | warps the position in place; follow with `GetNoise(x, y, z)` |

## Determinism

For a fixed seed and fixed configuration, `GetNoise`/`DomainWarp` are pure functions of their
input coordinates — same inputs always produce the same outputs, on any platform, across runs.
This is exercised by the Noise Examples sample's `SeedDeterminismExample` and its corresponding
EditMode tests.

## Performance notes

`GetNoise` and `DomainWarp` are marked `AggressiveOptimization`; internal helpers are marked
`AggressiveInlining`. Fractal noise (`Octaves` > 1) costs roughly `O(Octaves)` noise evaluations
per sample. See `NoiseBenchmarkExample` in the Samples for a way to measure this on your target
hardware.
