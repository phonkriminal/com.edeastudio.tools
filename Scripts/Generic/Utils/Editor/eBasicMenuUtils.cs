using UnityEditor;
using UnityEngine;
using edeastudio.Components;
using edeastudio.Utils;

namespace edeastudio.Utils.Editor
{
    /// <summary>
    /// The e basic menu utils.
    /// </summary>
    public partial class eBasicMenuUtils
    {
        /// <summary>
        /// The path.
        /// </summary>
        public const string path = "edeaStudio/Components/";

        /// <summary>
        /// Add destroy object.
        /// </summary>
        [MenuItem(path + "Destroy Object")]
        public static void AddDestroyObject()
        {
            var currentObject = Selection.activeGameObject;
            if (currentObject)
            {
                currentObject.AddComponent<EDSDestroyObject>();
            }
        }

        /// <summary>
        /// Add rotate object.
        /// </summary>
        [MenuItem(path + "Rotate Object")]
        public static void AddRotateObject()
        {
            var currentObject = Selection.activeGameObject;
            if (currentObject)
            {
                currentObject.AddComponent<edeastudio.Components.EDSRotateObject>();
            }
        }

        /// <summary>
        /// Add material flicker FX.
        /// </summary>
        [MenuItem(path + "Emissive Material Flicker FX")]
        public static void AddMaterialFlickerFX()
        {
            var currentObject = Selection.activeGameObject;
            if (currentObject)
            {
                currentObject.AddComponent<edeastudio.Components.EDSEmissiveMaterialFlickerEffect>();
            }
        }

        /// <summary>
        /// Add material animate.
        /// </summary>
        [MenuItem(path + "Color Material Animate")]
        public static void AddMaterialAnimate()
        {
            var currentObject = Selection.activeGameObject;
            if (currentObject)
            {
                currentObject.AddComponent<edeastudio.Components.EDSMaterialColorAnimate>();
            }
        }

        /// <summary>
        /// Add light flicker FX.
        /// </summary>
        [MenuItem(path + "Light Flicker FX")]
        public static void AddLightFlickerFX()
        {
            var currentObject = Selection.activeGameObject;
            if (currentObject)
            {
                currentObject.AddComponent<edeastudio.Components.EDSLightFlickerEffect>();
            }
        }

        /// <summary>
        /// Add the footstep.
        /// </summary>
        [MenuItem(path + "Footstep")]
        public static void AddFootstep()
        {
            var currentObject = Selection.activeGameObject;
            if (currentObject)
            {
                currentObject.AddComponent<edeastudio.Components.Footsteps>();
            }
        }
        /// <summary>
        /// Add sound looping.
        /// </summary>
        [MenuItem(path + "Sound Looping")]
        public static void AddSoundLooping()
        {
            var currentObject = Selection.activeGameObject;
            if (currentObject)
            {
                currentObject.AddComponent<edeastudio.Components.SoundLooping>();
            }
        }

    }

}