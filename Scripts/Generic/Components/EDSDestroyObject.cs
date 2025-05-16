using System;
using System.Collections;
using edeastudio.Attributes;
using UnityEngine;
using UnityEngine.Events;
using edeastudio.Utils;
using static edeastudio.Utils.Util;


namespace edeastudio.Components
{
    /// <summary>
    /// The EDS destroy object.
    /// </summary>
    [AddComponentMenu("edeaStudio/Destroy Object")]
    [eClassHeader("Destroy Object", iconName = "icon_v2")]
    public class EDSDestroyObject : eMonoBehaviour
    {
        /// <summary>
        /// Destroy time.
        /// </summary>
        [eEditorToolbar("Timing")]
        [Header("Timing")]
        [SerializeField]
        private float destroyTime = 0f;
        /// <summary>
        /// Wait time.
        /// </summary>
        [SerializeField]
        private float waitTime = 1f;
        /// <summary>
        /// Delay fade time.
        /// </summary>
        [SerializeField]
        private float delayFadeTime = 1f;
        /// <summary>
        /// Fade time.
        /// </summary>
        [SerializeField]
        private float fadeTime = 1f;
        /// <summary>
        /// The fx start time.
        /// </summary>
        [SerializeField]
        private float fxStartTime = 0f;
        /// <summary>
        /// The materials.
        /// </summary>
        [eEditorToolbar("Materials")]
        [Header("Materials")]
        [SerializeField]
        private Material[] materials;
        /// <summary>
        /// The mesh renderers.
        /// </summary>
        [SerializeField]
        private SkinnedMeshRenderer[] meshRenderers;
        /// <summary>
        /// The fx prefab.
        /// </summary>
        [SerializeField]
        private GameObject fxPrefab;
        /// <summary>
        /// The fx object.
        /// </summary>
        private GameObject fxObject;
        /// <summary>
        /// The alpha.
        /// </summary>
        private readonly float _alpha;
        /// <summary>
        /// Can fade.
        /// </summary>
        private bool canFade = false;
        /// <summary>
        /// Can destroy.
        /// </summary>
        private bool canDestroy = false;
        [Serializable]
        public class OnStartFadehHandler : UnityEvent { }
        [Serializable]
        public class OnFadeHandler : UnityEvent { }
        [Serializable]
        public class OnDestroyHandler : UnityEvent { }

        /// <summary>
        /// On fade.
        /// </summary>
        [eEditorToolbar("Events")]
        public OnStartFadehHandler onFade = new();
        /// <summary>
        /// On start fade.
        /// </summary>
        public OnFadeHandler onStartFade = new();
        /// <summary>
        /// On destroy.
        /// </summary>
        public OnDestroyHandler onDestroy = new();

        /// <summary>
        /// On fade action.
        /// </summary>
        private UnityAction OnFadeAction;
        /// <summary>
        /// On start fade action.
        /// </summary>
        private UnityAction OnStartFadeAction;
        /// <summary>
        /// On destroy action.
        /// </summary>
        private UnityAction OnDestroyAction;

        /// <summary>
        /// Gets or sets the destroy time.
        /// </summary>
        public float DestroyTime { get { return destroyTime; } set { destroyTime = value; } }
        // Start is called before the first frame update


        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            OnFadeAction += OnFade;
            OnStartFadeAction += OnStartFade;
            OnDestroyAction += OnDestroy;
        }

        /// <summary>
        /// On disable.
        /// </summary>
        private void OnDisable()
        {
            OnFadeAction -= OnFade;
            OnStartFadeAction -= OnStartFade;
            OnDestroyAction -= OnDestroy;
        }
        void Start()
        {
            if (meshRenderers.Length == 0) return;

            materials = new Material[meshRenderers.Length];


            for (int i = 0; i < meshRenderers.Length; i++)
            {
                materials[i] = meshRenderers[i].material;
            }


            _ = StartCoroutine(IEFadeColor(waitTime, delayFadeTime));

        }

        private void StartFX()
        {
            if (fxPrefab && fxObject == null)
            {
                fxObject = Instantiate(fxPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject, destroyTime);

        }

        private void OnFade()
        {
            onFade?.Invoke();
        }

        private void OnStartFade()
        {
            onStartFade?.Invoke();
        }

        private void OnDestroy()
        {
            if (materials.Length != 0)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    UnityEngine.Color color = materials[i].color;
                    color.a = 1f;
                    materials[i].color = color;
                    SetMaterialTransparent(materials[i], false);
                }
            }
            onDestroy?.Invoke();
        }

        private void Update()
        {
            if (materials.Length != 0 && canFade)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    UnityEngine.Color color = materials[i].color;
                    color.a = Mathf.Lerp(color.a, _alpha, fadeTime * Time.deltaTime);
                    materials[i].color = color;

                    double transparency = Math.Round(color.a, 2);

                    if (Mathf.Approximately((float)transparency, 0.00f))
                    {
                        canDestroy = true;
                    }
                }
            }

            if (canDestroy)
            {
                Invoke(nameof(StartFX), fxStartTime);
            }

        }



        IEnumerator IEFadeColor(float wait, float delay)
        {

            yield return new WaitForSeconds(wait);

            OnStartFadeAction.Invoke();

            yield return new WaitForSeconds(delay);

            for (int i = 0; i < materials.Length; i++)
            {
                SetMaterialTransparent(materials[i], true);
            }

            OnFadeAction?.Invoke();

            canFade = true;
        }
    }
}
