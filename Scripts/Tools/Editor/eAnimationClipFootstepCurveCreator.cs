using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using static edeastudio.Utils.Util;
using UnityEditor.Animations;

using Object = UnityEngine.Object;

namespace edeastudio.Tools
{
    /// <summary>
    /// The e animation clip footstep curve creator.
    /// </summary>
    public class eAnimationClipFootstepCurveCreator : EditorWindow
    {
        /// <summary>
        /// The logo.
        /// </summary>
        private Texture2D logo;
        /// <summary>
        /// The logo.
        /// </summary>
        private Texture2D _logo;
        /// <summary>
        /// The es skin.
        /// </summary>
        private GUISkin esSkin;
        /// <summary>
        /// Curve box style.
        /// </summary>
        private GUIStyle _curveBoxStyle;

        /// <summary>
        /// The scene reference model.
        /// </summary>
        public GameObject sceneReferenceModel;
        /// <summary>
        /// The animation clip.
        /// </summary>
        public AnimationClip animationClip;

        /// <summary>
        /// The source FBX.
        /// </summary>
        public GameObject sourceFBX;
        /// <summary>
        /// The importer mode.
        /// </summary>
        public ImporterMode importerMode = ImporterMode.sourceObject;
        /// <summary>
        /// The preview slider.
        /// </summary>
        public float previewSlider;
        /// <summary>
        /// The sample precision.
        /// </summary>
        public int samplePrecision = 12;
        /// <summary>
        /// Ignore list.
        /// </summary>
        public string ignoreList = "Idle";

        /// <summary>
        /// The left foot curve.
        /// </summary>
        public AnimationCurve leftFootCurve;
        /// <summary>
        /// The right foot curve.
        /// </summary>
        public AnimationCurve rightFootCurve;
        /// <summary>
        /// The combined curve.
        /// </summary>
        public AnimationCurve combinedCurve;

        /// <summary>
        /// Last slider.
        /// </summary>
        private float _lastSlider;
        /// <summary>
        /// Texture Button
        /// </summary>
        private Texture2D _buttonIcon;

        /// <summary>
        /// Show the window.
        /// </summary>
        [MenuItem("edeaStudio/Tools/Footstep Curve Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<eAnimationClipFootstepCurveCreator>();
            window.titleContent = new GUIContent("Footstep Curve Generator");
            window.Show();
        }

        /// <summary>
        /// On validate.
        /// </summary>
        private void OnValidate()
        {
            leftFootCurve ??= new AnimationCurve();
            rightFootCurve ??= new AnimationCurve();
            combinedCurve ??= new AnimationCurve();

        }

        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            leftFootCurve ??= new AnimationCurve();
            rightFootCurve ??= new AnimationCurve();
            combinedCurve ??= new AnimationCurve();
        }

        /// <summary>
        /// On GUI.
        /// </summary>
        private void OnGUI()
        {
            logo = Resources.Load("UI/Images/icon_v2") as Texture2D;
            _logo = ScaleTexture(logo, 48, 48);
            _buttonIcon = Resources.Load("EditorResources/delete") as Texture2D;

            if (!esSkin)
            {
                esSkin = Resources.Load("eSkin") as GUISkin;
            }

            GUI.skin = esSkin;

            _curveBoxStyle = esSkin.GetStyle("eCurveBox");

            this.titleContent = new GUIContent("Footstep Curve Generator", null, "Footstep Curve Generator");
            GUILayout.BeginVertical("  FOOTSTEPS CURVE GENERATOR", "window");
            GUILayout.Label(_logo, GUILayout.MaxHeight(48));

            GUILayout.Space(5);
            GUILayout.EndVertical();

            GUILayout.Label("Automatically generate Footstep curve on selected Animation Clip based on foot height during animation.", esSkin.GetStyle("eLabel"), GUILayout.MaxHeight(60));
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            sceneReferenceModel = (GameObject)EditorGUILayout.ObjectField("Scene Reference Model", sceneReferenceModel, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button(_buttonIcon, esSkin.FindStyle("eButton") ,GUILayout.Width(24), GUILayout.Height(24)))
            {
                sceneReferenceModel = null;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false) as AnimationClip;
            if (GUILayout.Button(_buttonIcon, esSkin.FindStyle("eButton"), GUILayout.Width(24), GUILayout.Height(24)))
            {
                animationClip = null;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            sourceFBX = (GameObject)EditorGUILayout.ObjectField("Source FBX", sourceFBX, typeof(GameObject), false) as GameObject;
            if (GUILayout.Button(_buttonIcon, esSkin.FindStyle("eButton"), GUILayout.Width(24), GUILayout.Height(24)))
            {
                sourceFBX = null;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            importerMode = (ImporterMode)EditorGUILayout.EnumPopup("Importer Mode", importerMode, GUILayout.MaxHeight(20));

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            if (!sceneReferenceModel && !animationClip && !sourceFBX)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("Please select a Scene Reference Model or Animation Clip or Source FBX.", MessageType.Warning);
                EditorGUILayout.EndVertical();
            }


            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label("Sample Precision", esSkin.label, GUILayout.MaxHeight(20));
            samplePrecision = EditorGUILayout.IntField(samplePrecision, GUILayout.MaxHeight(20));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Playback Preview", esSkin.label, GUILayout.MaxHeight(20));
            previewSlider = EditorGUILayout.Slider(previewSlider, 0, 1);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            ignoreList = EditorGUILayout.TextField("Ignore Animation Names", ignoreList, GUILayout.MaxHeight(20));

            EditorGUILayout.Separator();

            GUILayout.Label("Left Foot", esSkin.label, GUILayout.MaxHeight(20));
            EditorGUILayout.CurveField(leftFootCurve, GUILayout.MaxHeight(50));
            var rectL = GUILayoutUtility.GetLastRect();
            rectL.width *= previewSlider;
            GUI.Box(rectL, GUIContent.none, _curveBoxStyle);
            GUILayout.Label("Result Value: " + leftFootCurve.Evaluate(previewSlider), esSkin.label, GUILayout.MaxHeight(20));
            EditorGUILayout.Separator();

            GUILayout.Label("Right Foot", esSkin.label, GUILayout.MaxHeight(20));
            EditorGUILayout.CurveField(rightFootCurve, GUILayout.MaxHeight(50));
            var rectR = GUILayoutUtility.GetLastRect();
            rectR.width *= previewSlider;
            GUI.Box(rectR, GUIContent.none, _curveBoxStyle);
            GUILayout.Label("Result Value: " + rightFootCurve.Evaluate(previewSlider), esSkin.label, GUILayout.MaxHeight(20));
            EditorGUILayout.Separator();

            GUILayout.Label("Combined Feet Curve", esSkin.label, GUILayout.MaxHeight(20));
            EditorGUILayout.CurveField(combinedCurve, GUILayout.MaxHeight(50));
            var rectC = GUILayoutUtility.GetLastRect();
            rectC.width *= previewSlider;
            GUI.Box(rectC, GUIContent.none, _curveBoxStyle);
            GUILayout.Label("Result Value: " + combinedCurve.Evaluate(previewSlider), esSkin.label, GUILayout.MaxHeight(20));
            EditorGUILayout.Separator();

            if (sceneReferenceModel && importerMode == ImporterMode.sourceObject || sceneReferenceModel && animationClip && importerMode == ImporterMode.animationClip || sceneReferenceModel && sourceFBX && importerMode == ImporterMode.animationFBX)
            {
                if (GUILayout.Button("Generate Footstep Curve", GUILayout.MaxHeight(30)))
                {
                    if (importerMode == ImporterMode.sourceObject)
                    {
                        var rac = sceneReferenceModel.GetComponent<Animator>().runtimeAnimatorController;
                        var toIgnore = ignoreList.Split(',').Select(x => x.Trim()).ToList();
                        foreach (var anim in rac.animationClips)
                        {
                            if (toIgnore.Contains(anim.name))
                            {
                                Debug.Log($"<color=white>Skipped {anim.name} animation clip.</color>");
                                continue;
                            }

                            Debug.Log($"<color=green>{anim.name}.</color>");
                            GenerateFootstepCurve(anim);
                        }
                    }
                    else if (importerMode == ImporterMode.animationClip)
                    {
                        GenerateFootstepCurve(animationClip);
                    }
                    else if (importerMode == ImporterMode.animationFBX)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(sourceFBX);
                        var allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                        var toIgnore = ignoreList.Split(',').Select(x => x.Trim()).ToList();

                        var tempListAnimation = new List<AnimationClip>();

                        foreach (var asset in allAssets)
                        {
                            if (asset is AnimationClip && !asset.name.Contains("_preview_"))
                            {
                                tempListAnimation.Add((AnimationClip)asset as  AnimationClip);
                            }
                        }

                        Debug.Log(tempListAnimation.Count);

                        var orderedList =  tempListAnimation.OrderBy(x => x.name).ToList();
                     
                        var temp = new List<AnimationClip>();
                        

                        for (int i = 0; i < toIgnore.Count; i++)
                        {
                            foreach (var item in orderedList)
                            {
                                if (item.name.StartsWith(toIgnore[i]))
                                {
                                    temp.Add(item);
                                }
                            }
                        }
                        foreach (var item in temp)
                        {
                            Debug.Log($"<color=white>{item.name}</color>");
                            
                            if (tempListAnimation.Contains(item)) 
                            { 
                                tempListAnimation.Remove(item);
                                Debug.Log($"Item {item.name} Removed");
                            }
                        }
                        
                        AnimatorController animatorCtrl = new AnimatorController();

                        Object trc = AssetDatabase.LoadAssetAtPath<Object>("Assets/edeastudio/Resources/Animator/Temp.controller");


                        var _animator = sceneReferenceModel.GetComponent<Animator>();

                        AnimatorController originalAnimatorController = (AnimatorController)_animator.runtimeAnimatorController;

                        animatorCtrl = (AnimatorController)trc as AnimatorController;

                        foreach (var item in tempListAnimation)
                        {
                            animatorCtrl.AddMotion(item);
                        }

                        _animator.runtimeAnimatorController = animatorCtrl;

                        Debug.Log(_animator.runtimeAnimatorController.animationClips.Length);

                        foreach (var clip in animatorCtrl.animationClips)
                        {
                            GenerateFootstepCurve(clip);
                        }

                      /*  AnimationClip[] deleteAnimations = _animator.runtimeAnimatorController.animationClips;

                        foreach (var item in deleteAnimations)
                        {
                            if (Application.isPlaying)
                            {
                                Object.Destroy(item);
                            }
                            else
                            {
                                Object.DestroyImmediate(item);
                            }
                        }*/

                        _animator.runtimeAnimatorController = originalAnimatorController;

                    }

                    ignoreList = string.Empty;

                    if (previewSlider != _lastSlider)
                    {
                        animationClip.SampleAnimation(sceneReferenceModel, animationClip.length * previewSlider);
                    }

                }
            }

            EditorGUILayout.EndVertical();

            _lastSlider = previewSlider;

        }

        /// <summary>
        /// Generate footstep curve.
        /// </summary>
        /// <param name="clip">The clip.</param>
        private void GenerateFootstepCurve(AnimationClip clip)
        {            

            if (sceneReferenceModel == null)
            {
                Debug.Log("Source Animation Object is Null!");
                return;
            }

            var animator = sceneReferenceModel.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on the selected GameObject.");
                return;
            }

            leftFootCurve = new AnimationCurve();
            rightFootCurve = new AnimationCurve();
            combinedCurve = new AnimationCurve();

            for (int i = 0; i < samplePrecision; i++)
            {
                clip.SampleAnimation(sceneReferenceModel, i / (float)samplePrecision);

                var leftFootHeight = GetLowestHeightOnFoot(animator, true);
                var rightFootHeight = GetLowestHeightOnFoot(animator, false);

                leftFootCurve.AddKey(i / (clip.length * samplePrecision), leftFootHeight);
                rightFootCurve.AddKey(i / (clip.length * samplePrecision), rightFootHeight);
            }

            leftFootCurve = NormalizeCurve(leftFootCurve);
            rightFootCurve = NormalizeCurve(rightFootCurve);
            combinedCurve = CombineCurves(leftFootCurve, rightFootCurve);


            var assetPath = AssetDatabase.GetAssetPath(clip);
            var importer = (ModelImporter)AssetImporter.GetAtPath(assetPath) as ModelImporter;

            var so = new SerializedObject(importer);
            var clipProp = so.FindProperty("m_ClipAnimations");
            var animations = new ModelImporterClipAnimation[clipProp.arraySize];

            for (int i = 0; i < importer.clipAnimations.Length; i++)
            {
                animations[i] = importer.clipAnimations[i];
            }

            for (int i = 0; i < animations.Length; i++)
            {
                var anim = animations[i];
                if (clip.name == anim.name)
                {
                    var index = -1;
                    for (int j = 0; j < anim.curves.Length; j++)
                    {
                        if (anim.curves[j].name == "Footstep")
                        {
                            index = j;
                            break;
                        }
                    }

                    var footstepCurve = anim.curves.FirstOrDefault(x => x.name == "Footstep");
                    var isInset = index == -1;

                    footstepCurve.name = "Footstep";
                    footstepCurve.curve = combinedCurve;

                    if (isInset)
                    {
                        anim.curves = anim.curves.Append(footstepCurve).ToArray();
                    }
                    else
                    {
                        anim.curves[index] = footstepCurve;
                    }

                    var serializedCurves = clipProp.GetArrayElementAtIndex(i).FindPropertyRelative("curves");
                    for (int j = 0; j < anim.curves.Length; j++)
                    {
                        if (j >= serializedCurves.arraySize)
                        {
                            serializedCurves.InsertArrayElementAtIndex(j);
                        }

                        var c = serializedCurves.GetArrayElementAtIndex(j);
                        c.FindPropertyRelative("curve").animationCurveValue = anim.curves[j].curve;
                        c.FindPropertyRelative("name").stringValue = anim.curves[j].name;
                    }

                    break;
                }
            }

            so.ApplyModifiedProperties();
        }

        /// <summary>
        /// Get lowest height on foot.
        /// </summary>
        /// <param name="animator">The animator.</param>
        /// <param name="isLeft">If true, is left.</param>
        /// <returns>A float</returns>
        private float GetLowestHeightOnFoot(Animator animator, bool isLeft)
        {
            var toesBones = isLeft ? HumanBodyBones.LeftToes : HumanBodyBones.RightToes;
            var footBone = isLeft ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot;
            var footBottomHeight = isLeft ? animator.leftFeetBottomHeight : animator.rightFeetBottomHeight;

            var toesHeigth = animator.GetBoneTransform(toesBones)?.transform.position.y ?? Mathf.Infinity;
            var footHeight = animator.GetBoneTransform(footBone).transform.position.y - footBottomHeight;

            return Mathf.Min(toesHeigth, footHeight);
        }

        /// <summary>
        /// Normalize the curve.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns>An AnimationCurve</returns>
        private AnimationCurve NormalizeCurve(AnimationCurve curve, float threshold = .1f)
        {
            var rtnCurve = new AnimationCurve();
            var min = curve.keys.Min(x => x.value);
            var max = curve.keys.Max(x => x.value);
            var height = max - min;

            for (int i = 0; i < curve.keys.Length; i++)
            {
                var keyFrame = curve.keys[i];
                keyFrame.value /= height;

                if (i != 0 && i != curve.length - 1 && keyFrame.value < threshold)
                {
                    keyFrame.value = 0;
                }

                rtnCurve.AddKey(keyFrame);
            }

            ConvertCurveToLinear(rtnCurve);

            return rtnCurve;
        }

        /// <summary>
        /// Convert curve converts to linear.
        /// </summary>
        /// <param name="curve">The curve.</param>
        private void ConvertCurveToLinear(AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            }
        }

        /// <summary>
        /// Combine the curves.
        /// </summary>
        /// <param name="leftCurve">The left curve.</param>
        /// <param name="rightCurve">The right curve.</param>
        /// <returns>An AnimationCurve</returns>
        private AnimationCurve CombineCurves(AnimationCurve leftCurve, AnimationCurve rightCurve)
        {
            var rtnCurve = new AnimationCurve();

            for (int i = 0; i < samplePrecision; i++)
            {
                var time = i / (float)samplePrecision;
                rtnCurve.AddKey(new Keyframe(time, rightCurve.Evaluate(time) - leftCurve.Evaluate(time)));
            }

            ConvertCurveToLinear(rtnCurve);

            return rtnCurve;

        }

    }

    /// <summary>
    /// The importers modes.
    /// </summary>
    public enum ImporterMode
    {
        sourceObject= 0,
        animationClip = 1,
        animationFBX = 2
    }

}