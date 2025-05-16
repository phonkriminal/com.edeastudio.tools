using UnityEditor;
using UnityEngine;
using static edeastudio.Utils.Util;

namespace edeastudio.Shared
{
    public class AudioFxListWindowEditor : ExtendedEditorWindow
    {
        private const string GUI_SKIN_PATH = "eSkin";
        private const string LOGO_PATH = "EditorResources/AudioClipList Icon";
        GUISkin eSkin;
        public static void Open(AudioFXList dataObject)
        {
            AudioFxListWindowEditor window = GetWindow<AudioFxListWindowEditor>("Audio FX List Editor");
            window.serializedObject = new SerializedObject(dataObject);
        }

        private void OnGUI()
        {
            Texture2D logo = Resources.Load(LOGO_PATH) as Texture2D;
            Texture2D _logo = ScaleTexture(logo, 50, 50);
            eSkin = Resources.Load(GUI_SKIN_PATH) as GUISkin;
            GUIStyle headerStyle = eSkin.GetStyle("window");
            GUI.skin = eSkin;
            GUILayout.BeginHorizontal();
            GUIContent content = new();
            content.image = _logo;
            content.text = "  AUDIO FX LIST EDITOR";
            content.tooltip = "Audio FX List Editor";
            GUILayout.Label(content, headerStyle);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            currentProperty = serializedObject.FindProperty("audioElements");
            if (currentProperty == null)
            {
                Debug.LogError("currentProperty is null");
                return;
            }
            DrawSelectedPropertyPanel();

            EditorGUILayout.EndVertical();
            Apply();

        }
        void DrawSelectedPropertyPanel()
        {
            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("audioElements", false);
            GUI.skin = eSkin;
            EditorGUILayout.EndVertical();
        }
    }
}