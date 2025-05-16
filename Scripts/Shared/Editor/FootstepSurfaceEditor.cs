using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace edeastudio.Shared
{

    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            FootstepSurface obj = EditorUtility.InstanceIDToObject(instanceID) as FootstepSurface;
            if (obj != null)
            {
                FootstepObjectEditorWindow.Open(obj);
                return true;
            }
            return false;
        }

    }

    [CustomEditor(typeof(FootstepSurface))]
    public class FootstepSurfaceEditor : Editor
    {
        private const string GUI_SKIN_PATH = "eSkin";

        GUISkin eSkin;

        public override void OnInspectorGUI()
        {
            eSkin = Resources.Load(GUI_SKIN_PATH) as GUISkin;

            GUI.skin = eSkin;

            if (GUILayout.Button("Open Editor", eSkin.GetStyle("eButton")))
            {
                FootstepObjectEditorWindow.Open((FootstepSurface)target);
            }
        }
    }

}