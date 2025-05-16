using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace edeastudio.editor
{

    public class AudioSFXGameobject : MonoBehaviour
    {
        [MenuItem("GameObject/edeaStudio/AudioSFX Player", false, 10)]
        static void CreateAudioSFXPlayerGameObject(MenuCommand menuCommand)
        {
            GameObject go = new("Audio SFX Player");
            go.AddComponent<edeastudio.Components.AudioSFXPlayer>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

    }

}