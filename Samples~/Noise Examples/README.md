# Noise Examples

12 ready-to-open scenes demonstrating the FastNoiseLite package. Each scene already contains a
configured GameObject — just open the scene and press Play (or, for most examples, just look at
the Scene view; many update live in Edit Mode). No manual setup steps are required to see each
example working; the steps below are only for when you want to *change* something.

Every example has a **"Regenerate Now"** button at the top of its Inspector, and a **Live Update**
checkbox — turn it off if you want to tweak several fields before triggering a rebuild, then hit
"Regenerate Now" (or right-click the component header → **Regenerate**).

## 01 — Noise Types (`01_NoiseTypes.unity`)

`NoiseTypesExample` on the `NoiseTypesExample` GameObject. Samples all six `NoiseType` values
(OpenSimplex2, OpenSimplex2S, Cellular, Perlin, ValueCubic, Value) side by side as a 3x2 grid of
tiles in one texture, using the same `Seed`/`Frequency` for every tile. Change `Settings.Seed` or
`Settings.Frequency` and hit Regenerate to see all six algorithms respond together. The
`NoiseTextureVisualizer` component next to it controls tile resolution and `FilterMode`.

## 02 — Fractal Noise (`02_FractalNoise.unity`)

`FractalNoiseExample`. Starts with `FractalType = FBm`. Try `Octaves` 1 → 8 to see detail build
up, `Lacunarity`/`Gain` to change how aggressively each octave adds detail, and switch
`FractalType` between `FBm`, `Ridged` and `PingPong` to see the different combination curves.
`FractalType = None` ignores `Octaves` entirely (single-pass noise).

## 03 — Cellular Noise (`03_CellularNoise.unity`)

`CellularNoiseExample`. Cycle `Settings.CellularReturnType` through all seven values with a fixed
`CellularDistanceFunction` to see cell-value mosaics vs. the various distance-based gradients, then
cycle `CellularDistanceFunction` through its four values to see how cell shapes change (Euclidean
rounded, Manhattan diamond-ish, Hybrid a blend). `CellularJitter` above 1 introduces visible
artifacts on purpose — that's expected, not a bug.

## 04 — Domain Warp (`04_DomainWarp.unity`)

`DomainWarpExample`. Left tile = plain noise, right tile = the same noise sampled at a
domain-warped coordinate. `DomainWarpAmplitude` controls warp strength (0 = tiles match). The
warp "mode" (normal/progressive/independent) is controlled by `Settings.FractalType`: `None`,
`FBm`, `Ridged` or `PingPong` → normal single warp; `DomainWarpProgressive` → progressive;
`DomainWarpIndependent` → independent. Switch `DomainWarpType` to compare OpenSimplex2 vs.
OpenSimplex2Reduced vs. BasicGrid warping.

## 05 — Heightmap Terrain (`05_HeightmapTerrain.unity`)

`HeightmapTerrainExample`. A real Unity `Mesh` built from 2D noise. `WidthVertices`/
`DepthVertices` control mesh resolution (keep it reasonable — e.g. under 200x200 — to avoid a slow
Edit Mode rebuild), `CellSize` controls world-space spacing between vertices, `HeightScale`
exaggerates/flattens the terrain. Orbit the Scene view camera around the generated mesh.

## 06 — 3D Noise Slicing (`06_Noise3D.unity`)

`Noise3DExample`. Samples true 3D noise and displays one 2D slice through it. Switch `Plane`
between `XY`/`XZ`/`YZ` and drag `SlicePosition` to scrub through the volume — watch the texture
change continuously, proving the data really is 3D rather than a repeated 2D pattern. Try
`Settings.RotationType3D` = `ImproveXYPlanes` specifically when slicing the `XY` plane to see it
reduce directional artifacts.

## 07 — Density Field (`07_DensityField.unity`)

`DensityFieldExample`. Select its GameObject to see `Gizmos.DrawCube` markers in the Scene view for
every grid cell where 3D noise exceeds `IsoLevel` — a minimal "is this point solid" preview with no
Marching Cubes. Raise `IsoLevel` toward 1 to shrink the solid region toward isolated noise peaks;
lower it toward -1 to fill nearly the whole grid. `GridSize` above roughly 24x24x24 will visibly
slow down the Gizmo draw — expected, this is a debug-visualization example, not an optimized one.

## 08 — Chunk Continuity (`08_ChunkContinuity.unity`)

`ChunkContinuityExample`. Generates a `ChunkGridCount` grid of separate mesh chunks, each sampled
in world space so adjacent chunks share exact edge values. Open the Console after Regenerating to
see the logged maximum boundary error across all shared edges (should read `0` or a number in the
`1e-6` range — floating point, not exactly zero, but visually seamless). Increase `ChunkGridCount`
to e.g. `(4, 4)` to see more chunks tile together with no visible seams.

## 09 — Island Generator (`09_IslandGenerator.unity`)

`IslandGeneratorExample`. Combines noise with a radial falloff mask and colors the resulting mesh
by height (sea/land/peak gradient, split at `SeaLevel`). Shrink `IslandRadius` for a smaller
island with more open sea around it; raise `FalloffPower` for steeper/cliffier coastlines vs. a
gentler, more gradual one.

## 10 — Resource Distribution (`10_ResourceDistribution.unity`)

`ResourceDistributionExample`. Three independently-seeded noise layers ("Ore", "Crystal",
"Vegetation") each thresholded into a color-coded map; a pixel's color comes from the first layer
(in array order) whose noise exceeds its own `Threshold`. Lower a layer's `Threshold` to make that
resource more common; reorder `Layers` in the Inspector to change which resource "wins" where two
would otherwise overlap.

## 11 — Seed Determinism (`11_SeedDeterminism.unity`)

`SeedDeterminismExample`. On Regenerate, builds two independent `FastNoiseLite` instances per seed
in `SeedsToTest` and compares their output at a fixed set of coordinates — read the `Report` field
(or the Console) for a `PASS`/`FAIL` line per seed. This should always read `PASS`; it exists to
make FastNoiseLite's determinism guarantee visible and testable rather than just assumed.

## 12 — Noise Benchmark (`12_NoiseBenchmark.unity`)

`NoiseBenchmarkExample`. On Regenerate, times every `NoiseType` over a 2D grid (`SideLength2D`)
and a 3D grid (`SideLength3D`), after `WarmupIterations` unmeasured warmup passes, for `Iterations`
measured passes — read `Report` (or the Console) for per-algorithm milliseconds and samples/sec.
Results are machine- and Editor/Player-dependent; run a Player build for realistic numbers rather
than trusting Editor timings alone.

## Running the tests

Open **Window > General > Test Runner**, select the **EditMode** tab, and click **Run All**. Tests
cover seed determinism, 2D/3D sampling, fractal configuration, domain warp reproducibility,
terrain mesh generation, chunk boundary continuity, island falloff and resource distribution —
they exercise the same `public static` build methods the examples themselves call, so they run
without opening any scene. This requires Unity's **Test Framework** package (`com.unity.test-framework`);
if it isn't already in your project, Package Manager will prompt you to add it the first time you
open Test Runner.
