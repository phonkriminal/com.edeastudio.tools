using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace edeastudio.Shared
{
    public class AssetAudioFXHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            AudioFXList obj = EditorUtility.InstanceIDToObject(instanceID) as AudioFXList;
            if (obj != null)
            {
                AudioFxListWindowEditor.Open(obj);
                return true;
            }
            return false;
        }

    }


    [CustomEditor(typeof(AudioFXList))]
    public class AudioFxListEditor : Editor
    {
        private const string GUI_SKIN_PATH = "eSkin";

        GUISkin eSkin;

        public override void OnInspectorGUI()
        {
            eSkin = Resources.Load(GUI_SKIN_PATH) as GUISkin;

            GUI.skin = eSkin;

            if (GUILayout.Button("Open Editor", eSkin.GetStyle("eButton")))
            {
                AudioFxListWindowEditor.Open((AudioFXList)target);
            }
        }
    }

}