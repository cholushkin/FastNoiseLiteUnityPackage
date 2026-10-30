using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Renders noise data (grayscale or color) onto a Texture2D assigned to this object's own
    /// Renderer. Accessing <c>Renderer.material</c> (rather than <c>sharedMaterial</c>) makes
    /// Unity instance the material automatically the first time, so examples never modify a
    /// shared material asset.
    ///
    /// Also supports a "checkerboard preview" mode that ignores noise data entirely and instead
    /// renders a small black/white checker pattern, scaled up with the current FilterMode. This
    /// isolates *texture filtering* (how the GPU samples between texels) from *noise
    /// interpolation* (how FastNoiseLite blends between lattice points) — Point filtering shows
    /// hard checker edges, Bilinear shows them blending smoothly, regardless of the noise algorithm.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class NoiseTextureVisualizer : MonoBehaviour
    {
        [Header("Texture")]
        [Min(2)] public int Resolution = 256;
        public FilterMode FilterMode = FilterMode.Point;

        [Header("Checkerboard")]
        [Tooltip("Ignore noise data and instead render a checker pattern, to visualize FilterMode independently of any noise algorithm.")]
        public bool CheckerboardPreview = false;
        [Min(1)] public int CheckerboardCells = 8;

        Texture2D _texture;
        Renderer _renderer;

        Renderer TargetRenderer => _renderer != null ? _renderer : (_renderer = GetComponent<Renderer>());

        /// <summary>Renders a 2D array of grayscale values (expected roughly -1..1) as a texture.</summary>
        public void Apply(float[,] noise)
        {
            if (CheckerboardPreview)
            {
                ApplyCheckerboard();
                return;
            }

            int width = noise.GetLength(0);
            int height = noise.GetLength(1);
            var colors = new Color32[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte v = (byte)(Mathf.Clamp01((noise[x, y] + 1f) * 0.5f) * 255f);
                    colors[y * width + x] = new Color32(v, v, v, 255);
                }
            }
            ApplyPixels(width, height, colors);
        }

        /// <summary>Renders a 2D array of pre-computed colors as a texture.</summary>
        public void Apply(Color[,] colors)
        {
            if (CheckerboardPreview)
            {
                ApplyCheckerboard();
                return;
            }

            int width = colors.GetLength(0);
            int height = colors.GetLength(1);
            var packed = new Color32[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    packed[y * width + x] = colors[x, y];
            ApplyPixels(width, height, packed);
        }

        void ApplyCheckerboard()
        {
            int cells = Mathf.Max(1, CheckerboardCells);
            var colors = new Color32[cells * cells];
            for (int y = 0; y < cells; y++)
                for (int x = 0; x < cells; x++)
                    colors[y * cells + x] = ((x + y) % 2 == 0) ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 255);
            ApplyPixels(cells, cells, colors);
        }

        void ApplyPixels(int width, int height, Color32[] colors)
        {
            EnsureTexture(width, height);
            _texture.SetPixels32(colors);
            _texture.Apply(false);
        }

        void EnsureTexture(int width, int height)
        {
            if (_texture != null && _texture.width == width && _texture.height == height)
            {
                _texture.filterMode = FilterMode;
                return;
            }

            if (_texture != null) DestroyTexture();

            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode,
                wrapMode = TextureWrapMode.Clamp,
                name = $"{name}_NoisePreview"
            };

            var material = TargetRenderer.material;
            if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", _texture);
            else if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", _texture);
        }

        void DestroyTexture()
        {
            if (_texture == null) return;
            if (Application.isPlaying) Destroy(_texture);
            else DestroyImmediate(_texture);
            _texture = null;
        }

        void OnDestroy() => DestroyTexture();
    }
}
