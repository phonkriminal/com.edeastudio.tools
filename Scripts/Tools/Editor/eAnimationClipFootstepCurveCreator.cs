using System.Linq;
using UnityEditor;
using UnityEngine;
using static edeastudio.Utils.Util;


namespace edeastudio.Tools
{
    public class eAnimationClipFootstepCurveCreator : EditorWindow
    {
        private Texture2D logo;
        private Texture2D _logo;
        private GUISkin esSkin;
        private GUIStyle _curveBoxStyle;

        public GameObject sceneReferenceModel;
        public AnimationClip animationClip;

        public float previewSlider;
        public int samplePrecision = 12;
        public string ignoreList = "Idle";

        public AnimationCurve leftFootCurve;
        public AnimationCurve rightFootCurve;
        public AnimationCurve combinedCurve;

        private float _lastSlider;

        [MenuItem("edeaStudio/Tools/Footstep Curve Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<eAnimationClipFootstepCurveCreator>();
            window.titleContent = new GUIContent("Footstep Curve Generator");
            window.Show();
        }

        private void OnValidate()
        {
            leftFootCurve ??= new AnimationCurve();
            rightFootCurve ??= new AnimationCurve();
            combinedCurve ??= new AnimationCurve();

        }

        private void OnGUI()
        {
            logo = Resources.Load("UI/Images/icon_v2") as Texture2D;
            _logo = ScaleTexture(logo, 48, 48);


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
            sceneReferenceModel = (GameObject)EditorGUILayout.ObjectField("Scene Reference Model", sceneReferenceModel, typeof(GameObject), true) as GameObject;
            animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false) as AnimationClip;
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

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

            if (sceneReferenceModel && animationClip)
            {
                if (GUILayout.Button("Generate Footstep Curve", GUILayout.MaxHeight(30)))
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

                    // GenerateFootstepCurve(animationClip);
                }
                if (previewSlider != _lastSlider)
                {
                    animationClip.SampleAnimation(sceneReferenceModel, animationClip.length * previewSlider);
                }

            }

            EditorGUILayout.EndVertical();

            _lastSlider = previewSlider;
        }

        private void GenerateFootstepCurve(AnimationClip clip)
        {
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

        private float GetLowestHeightOnFoot(Animator animator, bool isLeft)
        {
            var toesBones = isLeft ? HumanBodyBones.LeftToes : HumanBodyBones.RightToes;
            var footBone = isLeft ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot;
            var footBottomHeight = isLeft ? animator.leftFeetBottomHeight : animator.rightFeetBottomHeight;

            var toesHeigth = animator.GetBoneTransform(toesBones)?.transform.position.y ?? Mathf.Infinity;
            var footHeight = animator.GetBoneTransform(footBone).transform.position.y - footBottomHeight;

            return Mathf.Min(toesHeigth, footHeight);
        }

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

        private void ConvertCurveToLinear(AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            }
        }

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

}