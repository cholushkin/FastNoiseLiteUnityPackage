# FastNoiseLite for Unity

A Unity Package Manager (UPM) wrapper around [FastNoiseLite](https://github.com/Auburn/FastNoiseLite),
Auburn/Jordan Peck's fast, portable, single-file noise generation library. This package vendors
the noise implementation as-is (plus one small additive constructor, see below) with a proper
Unity assembly definition, and offers a full set of interactive examples as an optional UPM Sample.

## What's included

- **FastNoiseLite 1.1.1** — the complete upstream `FastNoiseLite.cs`: OpenSimplex2, OpenSimplex2S,
  Cellular, Perlin, Value and Value Cubic noise types; FBm/Ridged/PingPong fractal layering; and
  OpenSimplex2/OpenSimplex2Reduced/BasicGrid domain warping.
- One additive constructor not present upstream — `FastNoiseLite(int seed, float frequency, int octaves, float lacunarity, float gain)` —
  added for convenience when porting from HLSL `fnlCreateState(...)` call sites (originally added
  for the Terraxel voxel terrain project). It only *adds* an overload; no existing upstream member
  was changed or removed.
- An optional **Noise Examples** sample: 12 scenes demonstrating every noise type, fractal mode,
  cellular configuration, domain warping, heightmap terrain, 3D slicing, density fields, chunk
  continuity, island generation, resource distribution, seed determinism and benchmarking.

## Upstream project & attribution

- Upstream repository: https://github.com/Auburn/FastNoiseLite
- Upstream license: MIT (Copyright (c) 2023 Jordan Peck and Contributors)
- This package's own structure, assembly definitions and Samples content: Copyright (c) 2026 Max Cholushkin, MIT.

See [LICENSE.md](LICENSE.md) for the full license text and [CHANGELOG.md](CHANGELOG.md) for release notes.

## Supported Unity version

Developed and verified against **Unity 6000.4.8f1**. The package manifest declares a minimum of
`6000.4`. Earlier Unity versions have not been tested against this package and compatibility is
not claimed for them — the runtime code itself has no engine dependencies (see
[Assembly definitions](#assembly-definitions) below), so it will likely work on older LTS versions
too, but that has not been verified here.

## Installing via Git URL

In Unity: **Window > Package Manager > "+" > Add package from git URL...**, then enter:

```
https://github.com/cholushkin/FastNoiseLiteUnityPackage.git
```

Or add it directly to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.qcat.fastnoise-lite": "https://github.com/cholushkin/FastNoiseLiteUnityPackage.git"
  }
}
```

To pin a specific commit or tag, append it after `#`, e.g.
`https://github.com/cholushkin/FastNoiseLiteUnityPackage.git#1.1.1`.

Installing the package this way only brings in `Runtime/` — no example scripts, scenes or
materials are added to your project until you explicitly import the sample (below).

## Local package development

To iterate on this package itself (e.g. from inside a consuming project without round-tripping
through Git), add it as a local path dependency instead:

```json
{
  "dependencies": {
    "com.qcat.fastnoise-lite": "file:../relative/path/to/FastNoiseLite"
  }
}
```

or use **Add package from disk...** in the Package Manager and pick this folder's `package.json`.

## Basic usage

```csharp
FastNoiseLite noise = new FastNoiseLite();
noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

float[,] noiseData = new float[128, 128];
for (int x = 0; x < 128; x++)
{
    for (int y = 0; y < 128; y++)
    {
        noiseData[x, y] = noise.GetNoise(x, y);
    }
}
```

`GetNoise` returns values roughly in the `-1..1` range. See the Noise Examples sample for fractal
layering, domain warping, 3D sampling and more advanced usage.

## Importing the optional Samples

1. **Window > Package Manager**, select **FastNoiseLite** in the list.
2. Open the **Samples** section.
3. Click **Import** next to **Noise Examples**.

Unity copies the sample into your project under
`Assets/Samples/FastNoiseLite/1.1.1/Noise Examples/` (the exact path includes the installed
package version and may vary slightly between Unity versions — Unity manages this location, do
not hardcode it). The sample is self-contained: all 12 scenes, their scripts and materials are
available immediately after import, with no further manual setup. See
[`Samples~/Noise Examples/README.md`](Samples~/Noise%20Examples/README.md) for details on every
example.

## Assembly definitions

| Assembly | Location | Depends on |
|---|---|---|
| `QCat.FastNoiseLite` | `Runtime/` | nothing (no UnityEngine reference) |
| `QCat.FastNoiseLite.Samples` | `Samples~/Noise Examples/Runtime/` | `QCat.FastNoiseLite` |
| `QCat.FastNoiseLite.Samples.Editor` | `Samples~/Noise Examples/Editor/` | `QCat.FastNoiseLite`, `QCat.FastNoiseLite.Samples` |
| `QCat.FastNoiseLite.Samples.Tests` | `Samples~/Noise Examples/Tests/EditMode/` | `QCat.FastNoiseLite`, `QCat.FastNoiseLite.Samples`, `UnityEngine.TestRunner`, `UnityEditor.TestRunner` |

Only the first assembly is part of the core package consumers get by default. The Samples
assemblies do not exist in your project at all until you import the sample, and are not referenced
by the core runtime assembly.

## Testing

The Noise Examples sample includes EditMode tests (seed determinism, 2D/3D sampling, fractal
configuration, domain warp reproducibility, terrain mesh generation, chunk boundary continuity,
island falloff, resource distribution, texture lifecycle). They require Unity's **Test Framework**
package (`com.unity.test-framework`), which ships with the Unity Editor by default. The core
`QCat.FastNoiseLite` runtime assembly has no such dependency and works whether or not the Test
Framework package is present — tests only exist once the sample is imported.

## License

MIT — see [LICENSE.md](LICENSE.md).
