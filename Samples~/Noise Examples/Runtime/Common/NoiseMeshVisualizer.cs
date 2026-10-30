using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Builds a grid mesh from a heightfield and assigns it to this object's MeshFilter.
    /// <see cref="BuildGridMesh"/> is a pure, static function so it is independently unit
    /// testable without a scene (see HeightmapTerrainExample, IslandGeneratorExample and the
    /// corresponding EditMode tests).
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class NoiseMeshVisualizer : MonoBehaviour
    {
        Mesh _mesh;
        MeshFilter _filter;

        MeshFilter TargetFilter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

        public void Apply(Mesh mesh)
        {
            if (_mesh != null && _mesh != mesh)
            {
                if (Application.isPlaying) Destroy(_mesh);
                else DestroyImmediate(_mesh);
            }
            _mesh = mesh;
            TargetFilter.sharedMesh = _mesh;
        }

        void OnDestroy()
        {
            if (_mesh == null) return;
            if (Application.isPlaying) Destroy(_mesh);
            else DestroyImmediate(_mesh);
            _mesh = null;
        }

        /// <summary>
        /// Builds a width*height vertex grid (cellSize apart) with per-vertex height taken from
        /// <paramref name="heights"/> and scaled by <paramref name="heightScale"/>. Vertex colors
        /// are white unless <paramref name="colorByHeight"/> evaluates <paramref name="gradient"/>
        /// against the normalized (0..1) height.
        /// </summary>
        public static Mesh BuildGridMesh(float[,] heights, float cellSize, float heightScale,
            bool colorByHeight = false, Gradient gradient = null)
        {
            int width = heights.GetLength(0);
            int height = heights.GetLength(1);

            var vertices = new Vector3[width * height];
            var uvs = new Vector2[width * height];
            var colors = colorByHeight ? new Color[width * height] : null;

            float minH = float.MaxValue, maxH = float.MinValue;
            if (colorByHeight)
            {
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        float h = heights[x, y];
                        if (h < minH) minH = h;
                        if (h > maxH) maxH = h;
                    }
                if (maxH - minH < 1e-6f) maxH = minH + 1e-6f;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;
                    float h = heights[x, y];
                    vertices[i] = new Vector3(x * cellSize, h * heightScale, y * cellSize);
                    uvs[i] = new Vector2(x / (float)(width - 1), y / (float)(height - 1));
                    if (colorByHeight) colors[i] = gradient != null
                        ? gradient.Evaluate((h - minH) / (maxH - minH))
                        : Color.Lerp(Color.black, Color.white, (h - minH) / (maxH - minH));
                }
            }

            int quadsX = width - 1;
            int quadsY = height - 1;
            var triangles = new int[quadsX * quadsY * 6];
            int t = 0;
            for (int y = 0; y < quadsY; y++)
            {
                for (int x = 0; x < quadsX; x++)
                {
                    int i00 = y * width + x;
                    int i10 = i00 + 1;
                    int i01 = i00 + width;
                    int i11 = i01 + 1;

                    triangles[t++] = i00; triangles[t++] = i01; triangles[t++] = i10;
                    triangles[t++] = i10; triangles[t++] = i01; triangles[t++] = i11;
                }
            }

            var mesh = new Mesh { name = "NoiseGridMesh" };
            if (vertices.Length > 65535) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            if (colorByHeight) mesh.colors = colors;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
