using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Demonstrates sampling 3D noise as a volumetric density field, visualized directly with
    /// Gizmos (no Marching Cubes or other mesh extraction here, on purpose).
    /// </summary>
    public class DensityFieldExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings();
        public Vector3Int GridSize = new Vector3Int(16, 16, 16);
        public float CellSize = 1f;
        public float IsoLevel = 0f;
        public Color AboveIsoColor = Color.cyan;

        float[,,] _density;

        public static float[,,] BuildDensityField(NoiseSettings settings, Vector3Int gridSize, float cellSize)
        {
            var noise = settings.CreateNoise();
            var result = new float[gridSize.x, gridSize.y, gridSize.z];
            for (int z = 0; z < gridSize.z; z++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int x = 0; x < gridSize.x; x++)
                    {
                        result[x, y, z] = noise.GetNoise(x * cellSize, y * cellSize, z * cellSize);
                    }
                }
            }
            return result;
        }

        protected override void Generate()
        {
            _density = BuildDensityField(Settings, GridSize, CellSize);
        }

        void OnDrawGizmos()
        {
            if (_density == null) return;

            int sizeX = _density.GetLength(0);
            int sizeY = _density.GetLength(1);
            int sizeZ = _density.GetLength(2);

            Gizmos.color = AboveIsoColor;
            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        if (_density[x, y, z] <= IsoLevel) continue;
                        Vector3 worldPos = transform.position + new Vector3(x, y, z) * CellSize;
                        Gizmos.DrawCube(worldPos, Vector3.one * CellSize * 0.5f);
                    }
                }
            }
        }
    }
}
