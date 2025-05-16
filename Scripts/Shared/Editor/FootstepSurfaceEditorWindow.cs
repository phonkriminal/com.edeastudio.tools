using UnityEditor;
using UnityEngine;
using static edeastudio.Utils.Util;

namespace edeastudio.Shared
{

    public class FootstepObjectEditorWindow : ExtendedEditorWindow
    {
        private const string GUI_SKIN_PATH = "eSkin";
        private const string LOGO_PATH = "EditorResources/Footstep Icon";
        GUISkin eSkin;
        public static void Open(FootstepSurface dataObject)
        {
            FootstepObjectEditorWindow window = GetWindow<FootstepObjectEditorWindow>("Footstep Surface Editor");
            window.serializedObject = new SerializedObject(dataObject);
        }

        private void OnGUI()
        {
            Texture2D logo = Resources.Load(LOGO_PATH) as Texture2D;
            Texture2D _logo = ScaleTexture(logo, 50, 50);
            eSkin = Resources.Load("eSkin") as GUISkin;
            GUIStyle headerStyle = eSkin.GetStyle("window");
            GUI.skin = eSkin;

            GUILayout.BeginHorizontal();

            GUIContent content = new();
            content.image = _logo;
            content.text = "  FOOTSTEP SURFACE EDITOR";
            content.tooltip = "Footstep Surface Editor";
            GUILayout.Label(content, headerStyle);

            GUILayout.EndHorizontal();

            currentProperty = serializedObject.FindProperty("gameData");

            EditorGUILayout.BeginVertical(GUI.skin.box);

            DrawSelectedPropertyPanel();

            EditorGUILayout.EndVertical();

            Apply();

        }
        void DrawSelectedPropertyPanel()
        {
            EditorGUILayout.BeginVertical("box");

            DrawField("surfaceName", true);
            DrawField("surfaceMaterial", true);
            DrawField("surfaceTexture", true);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("walkSounds", true);
            GUI.skin = eSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("runSounds", true);
            GUI.skin = eSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("jumpSounds", true);
            GUI.skin = eSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("landSounds", true);
            GUI.skin = eSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("slideSounds", true);
            GUI.skin = eSkin;
            EditorGUILayout.EndVertical();
        }
    }
}