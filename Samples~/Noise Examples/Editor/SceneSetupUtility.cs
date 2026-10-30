using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QCat.FastNoiseLite.Samples.Editor
{
    /// <summary>
    /// Builds the 12 Noise Examples scenes and their shared preview material using Unity's own
    /// Editor APIs, so the generated .unity/.mat assets are authored by Unity itself rather than
    /// hand-written YAML. Paths are derived from this script's own location (via CallerFilePath)
    /// so generation works no matter where Unity placed the imported sample on disk.
    /// </summary>
    public static class SceneSetupUtility
    {
        static string GetThisFilePath([CallerFilePath] string path = null) => path;
        static readonly string s_ThisFilePath = GetThisFilePath();

        static string SampleRootAbsolute => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(s_ThisFilePath), ".."));
        static string ScenesFolderAbsolute => Path.Combine(SampleRootAbsolute, "Scenes");
        static string MaterialsFolderAbsolute => Path.Combine(SampleRootAbsolute, "Materials");

        static string ScenesFolderAssetPath => ToAssetPath(ScenesFolderAbsolute);
        static string MaterialsFolderAssetPath => ToAssetPath(MaterialsFolderAbsolute);

        [MenuItem("Tools/FastNoiseLite/Generate Noise Example Scenes")]
        public static void GenerateAllScenes()
        {
            EnsureAssetFolder(ScenesFolderAssetPath);
            EnsureAssetFolder(MaterialsFolderAssetPath);

            var previewMaterial = GetOrCreatePreviewMaterial();

            BuildQuadScene<NoiseTypesExample>("01_NoiseTypes", previewMaterial, 10f);
            BuildQuadScene<FractalNoiseExample>("02_FractalNoise", previewMaterial, 10f);
            BuildQuadScene<CellularNoiseExample>("03_CellularNoise", previewMaterial, 10f);
            BuildQuadScene<DomainWarpExample>("04_DomainWarp", previewMaterial, 10f);
            BuildMeshScene<HeightmapTerrainExample>("05_HeightmapTerrain");
            BuildQuadScene<Noise3DExample>("06_Noise3D", previewMaterial, 10f);
            BuildEmptyScene<DensityFieldExample>("07_DensityField", new Vector3(0, 8, -16));
            BuildEmptyScene<ChunkContinuityExample>("08_ChunkContinuity", new Vector3(16, 20, -20));
            BuildMeshScene<IslandGeneratorExample>("09_IslandGenerator");
            BuildQuadScene<ResourceDistributionExample>("10_ResourceDistribution", previewMaterial, 10f);
            BuildEmptyScene<SeedDeterminismExample>("11_SeedDeterminism", new Vector3(0, 1, -10));
            BuildEmptyScene<NoiseBenchmarkExample>("12_NoiseBenchmark", new Vector3(0, 1, -10));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[SceneSetupUtility] Generated all 12 Noise Examples scenes.");
        }

        static Material GetOrCreatePreviewMaterial()
        {
            string path = MaterialsFolderAssetPath + "/NoisePreview.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Texture")
                ?? Shader.Find("Standard");
            var material = new Material(shader) { name = "NoisePreview" };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        static void BuildQuadScene<T>(string sceneName, Material previewMaterial, float scale) where T : Component
        {
            var scene = NewScene();

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = typeof(T).Name;
            quad.transform.localScale = new Vector3(scale, scale, 1f);
            quad.GetComponent<Renderer>().sharedMaterial = previewMaterial;

            var example = quad.AddComponent<T>();
            ((NoiseExampleBase)(object)example).Regenerate();

            FrameCamera(new Bounds(Vector3.zero, new Vector3(scale, scale, 0.1f)));
            SaveScene(scene, sceneName);
        }

        static void BuildMeshScene<T>(string sceneName) where T : Component
        {
            var scene = NewScene();

            var go = new GameObject(typeof(T).Name);
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            var example = go.AddComponent<T>();
            ((NoiseExampleBase)(object)example).Regenerate();

            var mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var bounds = mesh != null ? mesh.bounds : new Bounds(Vector3.zero, Vector3.one * 10f);
            FrameCamera(bounds);
            SaveScene(scene, sceneName);
        }

        static void BuildEmptyScene<T>(string sceneName, Vector3 cameraPosition) where T : Component
        {
            var scene = NewScene();

            var go = new GameObject(typeof(T).Name);
            var example = go.AddComponent<T>();
            ((NoiseExampleBase)(object)example).Regenerate();

            Camera.main.transform.position = cameraPosition;
            Camera.main.transform.LookAt(Vector3.zero);
            SaveScene(scene, sceneName);
        }

        static Scene NewScene()
        {
            return EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        static void FrameCamera(Bounds bounds)
        {
            var cam = Camera.main;
            if (cam == null) return;
            float radius = Mathf.Max(bounds.extents.magnitude, 1f);
            cam.transform.position = bounds.center + new Vector3(0f, radius * 0.6f, -radius * 1.8f);
            cam.transform.LookAt(bounds.center);
        }

        static void SaveScene(Scene scene, string sceneName)
        {
            string path = $"{ScenesFolderAssetPath}/{sceneName}.unity";
            EditorSceneManager.SaveScene(scene, path);
        }

        static string ToAssetPath(string absolutePath)
        {
            absolutePath = absolutePath.Replace('\\', '/');
            string projectRoot = Path.GetFullPath(Application.dataPath + "/..").Replace('\\', '/');
            if (!absolutePath.StartsWith(projectRoot))
                throw new System.InvalidOperationException($"Path '{absolutePath}' is outside the current project ('{projectRoot}'); scenes/materials must be generated from a project that has this sample imported under Assets/.");
            return absolutePath.Substring(projectRoot.Length + 1);
        }

        static void EnsureAssetFolder(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath)) return;

            var parts = assetFolderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
