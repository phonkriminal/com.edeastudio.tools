using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using static edeastudio.Utils.Util;

namespace edeastudio.Tools.Editor
{

    /// <summary>
    /// The EDS skinned mesh baker editor.
    /// </summary>
    public class EDSSkinnedMeshBakerEditor : EditorWindow
    {
        /// <summary>
        /// The GUI SKIN PATH.
        /// </summary>
        private const string GUI_SKIN_PATH = "eSkin";
        /// <summary>
        /// The rect.
        /// </summary>
        Vector2 rect = new Vector2(400, 400);
        /// <summary>
        /// The E skin.
        /// </summary>
        GUISkin eSkin;
        /// <summary>
        /// The M logo.
        /// </summary>
        Texture2D m_Logo;

        /// <summary>
        /// The skinned mesh.
        /// </summary>
        [SerializeField]
        private SkinnedMeshRenderer skinnedMesh;
        /// <summary>
        /// The default GO.
        /// </summary>
        [SerializeField]
        private GameObject defaultGO;
        /// <summary>
        /// The default mesh.
        /// </summary>
        private Mesh defaultMesh;

        /// <summary>
        /// The mesh name.
        /// </summary>
        [SerializeField]
        private string meshName;
        /// <summary>
        /// The baked mesh.
        /// </summary>
        private Mesh bakedMesh;
        /// <summary>
        /// The mesh filter.
        /// </summary>
        private MeshFilter meshFilter;
        /// <summary>
        /// Can bake.
        /// </summary>
        private bool canBake = false;
        /// <summary>
        /// The toggle.
        /// </summary>
        private bool toggle = false;
        /// <summary>
        /// Is legal.
        /// </summary>
        private bool isLegal = false;
        /// <summary>
        /// Show the window.
        /// </summary>
        [MenuItem("edeaStudio/Tools/Mesh Baker", false, 2)]

        public static void ShowWindow()
        {
            GetWindow<EDSSkinnedMeshBakerEditor>();
        }

        /// <summary>
        /// On GUI.
        /// </summary>
        private void OnGUI()
        {

            m_Logo = Resources.Load("UI/Images/icon_v2") as Texture2D;
            Texture2D _logo = ScaleTexture(m_Logo, 48, 48);
            if (!eSkin)
            {
                eSkin = Resources.Load("eSkin") as GUISkin;
            }
            GUI.skin = eSkin;

            this.minSize = rect;
            this.titleContent = new GUIContent("Character", null, "Character Creator Mesh Baker");
            GUILayout.BeginHorizontal("EDS MESH BAKER", "window");
            GUILayout.Label(_logo, GUILayout.MaxHeight(48));
            //GUILayout.Space(5);
            GUILayout.EndHorizontal();

            #region HEADER
            /* GUILayout.BeginHorizontal();

             GUIContent content = new();
             content.image = _logo;
             content.text = "  EDS Mesh Baker";
             content.tooltip = "EDS Skinned and Mesh Baker Editor";
             GUILayout.Label(content);

             GUILayout.EndHorizontal();*/

            //EditorGUILayout.Separator();



            GUILayout.BeginVertical(eSkin.window);

            toggle = EditorGUILayout.Toggle("Is Skinned Mesh", toggle, "toggle");

            if (toggle)
            {

                skinnedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Skinned Mesh", skinnedMesh, typeof(SkinnedMeshRenderer), true);

                if (!skinnedMesh)
                {
                    EditorGUILayout.Separator();

                    EditorGUILayout.HelpBox("You must assign a skinned mesh renderer component", MessageType.Warning);
                }

                canBake = skinnedMesh;
            }
            else
            {
                defaultGO = (GameObject)EditorGUILayout.ObjectField("Game Object", defaultGO, typeof(GameObject), true);

                if (!defaultGO)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.HelpBox("You must assign a GameObject", MessageType.Warning);

                }
                else
                {
                    isLegal = defaultGO.TryGetComponent<MeshFilter>(out meshFilter);

                    if (!isLegal)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("You must assign a GameObject with the MeshFilter component.", MessageType.Error);
                    }
                    else
                    {
                        defaultMesh = meshFilter.sharedMesh;
                        if (!defaultMesh)
                        {
                            EditorGUILayout.Separator();
                            EditorGUILayout.HelpBox("You must assign a mesh renderer component", MessageType.Warning);
                        }
                    }
                }
                canBake = defaultGO & isLegal;
            }

            EditorGUILayout.Separator();

            meshName = EditorGUILayout.TextField("Mesh Name", meshName);

            if (meshName is null || meshName.Trim().Length == 0)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("You must assign a name to the new mesh", MessageType.Info);
                canBake &= false;
            }


            #endregion

            EditorGUILayout.Space(20);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Bake Mesh"))
            {
                if (canBake & toggle)
                {
                    BakeSkinnedMesh();
                }
                else if (canBake & !toggle)
                {
                    BakeMesh();
                }
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Reset"))
            {
                toggle = false;
                defaultGO = null;
                skinnedMesh = null;
                meshName = "";
            }

            EditorGUILayout.EndVertical();

        }

        /// <summary>
        /// Bake the mesh.
        /// </summary>
        private void BakeMesh()
        {
            bakedMesh = defaultMesh;

            Material material = defaultGO.GetComponent<Renderer>().sharedMaterial;
            //SaveMesh(bakedMesh, meshName, false, true);

            GameObject tempGameObject = new();
            tempGameObject.name = meshName;
            tempGameObject.AddComponent<MeshFilter>();
            tempGameObject.AddComponent<MeshRenderer>();
            tempGameObject.GetComponent<MeshFilter>().mesh = bakedMesh;
            tempGameObject.GetComponent<MeshRenderer>().material = material;

            ExportMesh(tempGameObject, meshName, false, true);
            Debug.Log($"Mesh {meshName} saved correctly.");
            DestroyImmediate(tempGameObject);
        }
        /// <summary>
        /// Bake skinned mesh.
        /// </summary>
        private void BakeSkinnedMesh()
        {
            bakedMesh = new Mesh();
            skinnedMesh.BakeMesh(bakedMesh);
            Material material = skinnedMesh.sharedMaterial;


            GameObject tempGameObject = new();
            tempGameObject.name = meshName;
            tempGameObject.AddComponent<MeshFilter>();
            tempGameObject.AddComponent<MeshRenderer>();
            tempGameObject.GetComponent<MeshFilter>().mesh = bakedMesh;
            tempGameObject.GetComponent<MeshRenderer>().material = material;

            ExportMesh(tempGameObject, meshName, false, true);
            Debug.Log($"Mesh {meshName} saved correctly.");
            DestroyImmediate(tempGameObject);
        }

        /// <summary>
        /// Exports the mesh.
        /// </summary>
        /// <param name="go">The go.</param>
        /// <param name="name">The name.</param>
        /// <param name="makeNewInstance">If true, make new instance.</param>
        /// <param name="optimizeMesh">If true, optimize mesh.</param>
        private void ExportMesh(GameObject go, string name, bool makeNewInstance, bool optimizeMesh)
        {

            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "fbx");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetPhysicalPath(path);
            Debug.Log(path);
            //Mesh meshToSave = makeNewInstance ? UnityEngine.Object.Instantiate(mesh) as Mesh : mesh;

            //if (optimizeMesh)
            //    MeshUtility.Optimize(meshToSave);

            ExportBinaryFBX(path, go);
            AssetDatabase.Refresh();

        }

        /// <summary>
        /// Save the mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <param name="name">The name.</param>
        /// <param name="makeNewInstance">If true, make new instance.</param>
        /// <param name="optimizeMesh">If true, optimize mesh.</param>
        private void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            Mesh meshToSave = makeNewInstance ? UnityEngine.Object.Instantiate(mesh) as Mesh : mesh;

            if (optimizeMesh)
                MeshUtility.Optimize(meshToSave);
            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();

        }
        /// <summary>
        /// Exports binary FBX.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="singleObject">The single object.</param>
        private static void ExportBinaryFBX(string filePath, UnityEngine.Object singleObject)
        {
            // Find relevant internal types in Unity.Formats.Fbx.Editor assembly
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName == "Unity.Formats.Fbx.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").GetTypes();
            Type optionsInterfaceType = types.First(x => x.Name == "IExportOptions");
            Type optionsType = types.First(x => x.Name == "ExportOptionsSettingsSerializeBase");
            Type optionsPositionType = types.First(x => x.Name == "ExportModelSettingsSerialize");
            // Instantiate a settings object instance
            MethodInfo optionsProperty = typeof(ModelExporter).GetProperty("DefaultOptions", BindingFlags.Static | BindingFlags.NonPublic).GetGetMethod(true);

            object optionsInstance = optionsProperty.Invoke(null, null);
            FieldInfo exportPositionField = optionsPositionType.GetField("objectPosition", BindingFlags.Instance | BindingFlags.NonPublic);
            exportPositionField.SetValue(optionsInstance, 1);

            // Change the export setting from ASCII to binary
            FieldInfo exportFormatField = optionsType.GetField("exportFormat", BindingFlags.Instance | BindingFlags.NonPublic);
            exportFormatField.SetValue(optionsInstance, 1);

            // Invoke the ExportObject method with the settings param
            MethodInfo exportObjectMethod = typeof(ModelExporter).GetMethod("ExportObject", BindingFlags.Static | BindingFlags.NonPublic, Type.DefaultBinder, new Type[] { typeof(string), typeof(UnityEngine.Object), optionsInterfaceType }, null);
            exportObjectMethod.Invoke(null, new object[] { filePath, singleObject, optionsInstance });
        }
    }

}