using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static edeastudio.Utils.Util;

namespace edeastudio.Tools.Editor
{
    /// <summary>
    /// The EDS remove all components.
    /// </summary>
    public class EDSRemoveAllComponents : EditorWindow
    {
        //This is an editor script that is used to remove all components of a gameobject.
        //To use: Add this script to a gameobject
        //To clear multiple objects from components, mark multiple objects
        //and add the script to them

        /// <summary>
        /// The logo.
        /// </summary>
        Texture2D logo;
        /// <summary>
        /// The logo.
        /// </summary>
        Texture2D _logo;

        /// <summary>
        /// The es skin.
        /// </summary>
        GUISkin esSkin;

        /// <summary>
        /// The min rect.
        /// </summary>
        Vector2 minRect = new(300, 50);

        /// <summary>
        /// Check again.
        /// </summary>
        private bool checkAgain = false;

        /// <summary>
        /// The game object.
        /// </summary>
        public GameObject gameObject = null;

        /// <summary>
        /// The components.
        /// </summary>
        public List<Component> components = new();

        /// <summary>
        /// The index.
        /// </summary>
        int index = 0;

        /// <summary>
        /// Show the window.
        /// </summary>
        [MenuItem("edeaStudio/Tools/Remove All Components", false, 1)]

        public static void ShowWindow()
        {
            GetWindow<EDSRemoveAllComponents>();
        }

        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            logo = Resources.Load("UI/Images/icon_v2") as Texture2D;
            _logo = ScaleTexture(logo, 50, 50);
            CheckConditions();
        }


        /// <summary>
        /// Check the conditions.
        /// </summary>
        void CheckConditions()
        {
            if (Selection.activeObject)
            {
                gameObject = Selection.activeGameObject;
            }
            if (gameObject)
            {
                checkAgain = false;
            }
            if (!gameObject)
            {
                checkAgain = true;
            }
        }



        /// <summary>
        /// Get current components.
        /// </summary>
        void GetCurrentComponents()
        {
            gameObject.GetComponents<Component>(components);
        }



        /// <summary>
        /// On GUI.
        /// </summary>
        private void OnGUI()
        {
            if (!esSkin)
            {
                esSkin = Resources.Load("eSkin") as GUISkin;
            }

            GUI.skin = esSkin;
            //Texture2D preview;

            this.minSize = minRect;
            this.titleContent = new GUIContent("Components Cleaner", null, "Remove All Components from GameObject");

            GUILayoutOption[] layoutOptions = new GUILayoutOption[]
            {
                GUILayout.Width(minRect.x),
                GUILayout.Height(minRect.y)
            };

            GUILayout.BeginVertical(" Component Cleaner", "window");//, layoutOptions);

            GUILayout.Label(_logo, GUILayout.MaxHeight(48));
            GUILayout.Space(5);
            GUILayout.EndVertical();


            GUILayout.BeginVertical("", "window");  //, layoutOptions);

            if (checkAgain) CheckConditions();

            GUILayoutOption[] layoutOptionsBox = new GUILayoutOption[]
            {
                GUILayout.Width(minRect.x -20),
                GUILayout.Height(minRect.y - 20)
            };

            GUILayout.BeginVertical("box", layoutOptionsBox);


            if (!gameObject)
            {
                EditorGUILayout.HelpBox("Select a GameObject!", MessageType.Info);
                checkAgain = true;
            }

            gameObject = EditorGUILayout.ObjectField("GameObject ", gameObject, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;

            if (gameObject != null && gameObject.scene.name == null)
            {
                EditorGUILayout.HelpBox("GameObject cannot be a Prefab!", MessageType.Info);
            }

            GUILayout.EndVertical();


            GUILayout.BeginVertical("List of Components", "texturebox", layoutOptionsBox);

            GUILayout.Space(25);

            if (gameObject && gameObject.scene.name != null)
            {
                GetCurrentComponents();
                if (components.Count > 0)
                {
                    List<string> componentsList = new();
                    foreach (Component item in components)
                    {
                        string componentName = item.GetType().ToString();
                        //Debug.Log(componentName);
                        if (componentName != "UnityEngine.Transform" && !componentsList.Contains(componentName))
                        {
                            componentsList.Add(componentName);
                        }
                    }

                    string[] options = componentsList.ToArray();
                    index = EditorGUILayout.Popup(index, options);

                    if (GUILayout.Button("Clean", layoutOptionsBox))
                    {
                        Clean(index, options[index]);
                    }
                    //Debug.Log(componentsList.Count());
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void Clean(int index, string componentName)
        {

            if (EditorUtility.DisplayDialog("Remove Components?",
                    $"Are you sure you want to remove all components of type {componentName}?", "Remove", "Abort"))
            {
                foreach (var item in gameObject.GetComponents<Component>())
                {
                    if (item.GetType().ToString() == componentName)
                    {
                        DestroyImmediate(item);
                    }
                }
                Debug.Log("Clean");
            }
        }

    }
}