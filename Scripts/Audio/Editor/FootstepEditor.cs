
using UnityEditor;
using edeastudio.Editor;

namespace edeastudio.Components.Editor
{
    using Editor = UnityEditor.Editor;
    [CustomEditor(typeof(Footsteps))]
    public class FootstepEditor : eEditorBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            // Initialize properties
            // Example: myProperty = serializedObject.FindProperty("myProperty");
        }
    }

}