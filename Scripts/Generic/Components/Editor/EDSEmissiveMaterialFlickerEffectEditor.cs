using UnityEditor;
using UnityEngine;
using static edeastudio.Utils.Util;

namespace edeastudio.Components.Editor
{
    using Editor = UnityEditor.Editor;

    [SelectionBase]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EDSEmissiveMaterialFlickerEffect))]

    public class EDSEmissiveMaterialFlickerEffectEditor : Editor
    {
        SerializedProperty material;
        SerializedProperty minIntensity;
        SerializedProperty maxIntensity;
        SerializedProperty smoothing;
        SerializedProperty updateGI;

        private void OnEnable()
        {
            material = serializedObject.FindProperty("material");
            minIntensity = serializedObject.FindProperty("minIntensity");
            maxIntensity = serializedObject.FindProperty("maxIntensity");
            smoothing = serializedObject.FindProperty("smoothing");
            updateGI = serializedObject.FindProperty("updateGI");
        }

        public override void OnInspectorGUI()
        {
            EDSEmissiveMaterialFlickerEffect materialFlicker = (EDSEmissiveMaterialFlickerEffect)target;
            Texture2D logo = Resources.Load("icon_v2") as Texture2D;
            Texture2D _logo = ScaleTexture(logo, 50, 50);
            GUISkin esSkin = Resources.Load("eSkin") as GUISkin;
            GUI.skin = esSkin;

            #region HEADER
            /* GUILayout.BeginHorizontal();

             GUIContent content = new();
             content.image = _logo;
             content.text = "  MATERIAL FLICKER FX";
             content.tooltip = "EDS Emissive Material Flicker Effect";
             GUILayout.Box(content);

             GUILayout.EndHorizontal();*/

            serializedObject.Update();

            EditorGUILayout.HelpBox("This Script provide to apply a Flicker FX to the associated material. Material must be emissive", MessageType.Info);
            EditorGUILayout.PropertyField(material);
            if (!materialFlicker.material)
            {
                EditorGUILayout.HelpBox("This Script needs a material.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.PropertyField(minIntensity);
                EditorGUILayout.PropertyField(maxIntensity);
                EditorGUILayout.PropertyField(smoothing);
                EditorGUILayout.PropertyField(updateGI);
            }
            serializedObject.ApplyModifiedProperties();



            #endregion
        }
    }

}