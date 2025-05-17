using System.Collections.Generic;
using edeastudio.Attributes;
using UnityEngine;
using edeastudio.Utils;

namespace edeastudio.Components
{
    /// <summary>
    /// The footsteps.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("edeaStudio/Footstep")]
    [eClassHeader("Footsteps", iconName = "icon_v2")]

    public class Footsteps : eMonoBehaviour
    {
        /// <summary>
        /// The footstep source.
        /// </summary>
        [eEditorToolbar("Audio Component")]
        [Space(5)]
        [SerializeField] AudioSource footstepSource;
        /// <summary>
        /// The audio source prefab.
        /// </summary>
        [SerializeField]
        private GameObject audioSourcePrefab;

        /// <summary>
        /// The animator.
        /// </summary>
        [SerializeField]
        private Animator _animator;
        /// <summary>
        /// Last footstep.
        /// </summary>
        private float _lastFootstep;

        /// <summary>
        /// The audio source object.
        /// </summary>
        private GameObject audioSourceObject;

        /// <summary>
        /// The left foot collider.
        /// </summary>
        private SphereCollider _leftFootCollider;
        /// <summary>
        /// The right foot collider.
        /// </summary>
        private SphereCollider _rightFootCollider;

        /// <summary>
        /// The footsteps.
        /// </summary>
        [eEditorToolbar("Audio Surface")]
        [Space(10)]
        [SerializeField] List<FootstepSurface> footsteps = new();

        /// <summary>
        /// The left foot transform.
        /// </summary>
        [eEditorToolbar("Feet Transform")]
        [Space(10)]
        [SerializeField] private Transform leftFootTransform;
        /// <summary>
        /// The right foot transform.
        /// </summary>
        [SerializeField] private Transform rightFootTransform;
        /// <summary>
        /// The footstep radius.
        /// </summary>
        [Range(0.1f, 1f)]
        [SerializeField] private float footstepRadius = 0.1f;
        /// <summary>
        /// The ground layer.
        /// </summary>
        [eLayerMask]
        public LayerMask groundLayer = 1 << 0;

        /// <summary>
        /// The current surface name.
        /// </summary>
        private string _currentSurfaceName = string.Empty;


        /// <summary>
        /// Walk sounds.
        /// </summary>
        private AudioClip[] walkSounds = new AudioClip[0];
        /// <summary>
        /// Run sounds.
        /// </summary>
        private AudioClip[] runSounds  = new AudioClip[0];
        /// <summary>
        /// Land sounds.
        /// </summary>
        private AudioClip[] landSounds = new AudioClip[0];
        /// <summary>
        /// Jump sounds.
        /// </summary>
        private AudioClip[] jumpSounds = new AudioClip[0];
        /// <summary>
        /// The slide sounds.
        /// </summary>
        private AudioClip[] slideSounds = new AudioClip[0];

        /// <summary>
        /// The footstep type.
        /// </summary>
        private FootstepType _footstepType = FootstepType.Walk;
        /// <summary>
        /// The Start method.
        /// </summary>
        private void Start()
        {
            if (footstepSource == null && audioSourcePrefab == null)
            {
                gameObject.AddComponent<AudioSource>();
                footstepSource = GetComponent<AudioSource>();
            }

            if (footstepSource == null && audioSourcePrefab != null)
            {
                audioSourceObject = Instantiate(audioSourcePrefab, transform);
                footstepSource = audioSourceObject.GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// On validate.
        /// </summary>
        private void OnValidate()
        {
            if (!_animator) { _animator = GetComponent<Animator>(); }

            if (!leftFootTransform)
            {
                // Corrected the code to use HumanBodyBones.LeftFoot directly with the Animator component
                leftFootTransform = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);

                if (leftFootTransform == null)
                {
                    Debug.LogWarning("Left foot transform not found. Please assign it in the inspector.");
                    return;
                }

            }

            if (leftFootTransform.GetComponent<SphereCollider>() == null)
            {
                _leftFootCollider = leftFootTransform.gameObject.AddComponent<SphereCollider>();
                _leftFootCollider.isTrigger = true;
                _leftFootCollider.radius = footstepRadius; // Adjust the radius as needed
            }
            else
            {
                _leftFootCollider = leftFootTransform.GetComponent<SphereCollider>();

            }

            if (!rightFootTransform)
            {
                rightFootTransform = _animator.GetBoneTransform(HumanBodyBones.RightFoot);

                if (rightFootTransform == null)
                {
                    Debug.LogWarning("Right foot transform not found. Please assign it in the inspector.");
                }
            }

            if (rightFootTransform.GetComponent<SphereCollider>() == null)
            {
                _rightFootCollider = rightFootTransform.gameObject.AddComponent<SphereCollider>();
                _rightFootCollider.isTrigger = true;
                _rightFootCollider.radius = footstepRadius; // Adjust the radius as needed
            }
            else
            {
                _rightFootCollider = rightFootTransform.GetComponent<SphereCollider>();
            }
        }
        /// <summary>
        /// The update method.
        /// </summary>
        private void Update()
        {
            UpdateColliders();
            UpdateFootsteps();
        }

        /// <summary>
        /// Get surface name.
        /// </summary>
        /// <returns>A string</returns>
        private string GetSurfaceName()
        {
            if (_leftFootCollider)
            {
                if (Physics.Raycast(leftFootTransform.position, Vector3.down, out RaycastHit hit, 10f, groundLayer))
                {
                    var rtnString = hit.collider.gameObject.GetComponent<MeshCollider>().material.name;
                    rtnString = rtnString.Replace(" (Instance)", string.Empty);
                    Debug.Log("Footstep surface: " + rtnString);
                    return rtnString;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Get clips for surface.
        /// </summary>
        /// <param name="surfaceName">The surface name.</param>
        /// <returns>A bool</returns>
        private bool GetClipsForSurface(string surfaceName)
        {
            if (footsteps.Count == 0 || string.IsNullOrEmpty(surfaceName))
            {
                Debug.LogWarning("No footstep surface assigned.");
                return false;
            }

            if (_currentSurfaceName != surfaceName)
            {
                walkSounds?.Initialize();
                runSounds?.Initialize();
                landSounds?.Initialize();
                jumpSounds?.Initialize();
                slideSounds?.Initialize();

                walkSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.WalkSounds;
                runSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.RunSounds;
                landSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.LandSounds;
                jumpSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.JumpSounds;
                slideSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.SlideSounds;

                Debug.Log(walkSounds.Length);

                _currentSurfaceName = surfaceName;
            }

            return true;
        }

        /// <summary>
        /// Update the colliders.
        /// </summary>
        private void UpdateColliders()
        {
            if (!_leftFootCollider || !_rightFootCollider)
            {
                return;
            }

            if (_leftFootCollider.radius != footstepRadius)
            {
                _leftFootCollider.radius = footstepRadius;
            }
            if (_rightFootCollider.radius != footstepRadius)
            {
                _rightFootCollider.radius = footstepRadius;
            }
        }
        /// <summary>
        /// Update the footsteps.
        /// </summary>
        private void UpdateFootsteps()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            var _footstep = _animator.GetFloat("Footstep");

            if (Mathf.Abs(_footstep) < 0.00001f)
            {
                _footstep = 0;
            }

            if (_lastFootstep > 0 && _footstep < 0 || _lastFootstep < 0 && _footstep > 0)
            {
                PlayFootstep();
            }

            _lastFootstep = _footstep;

        }

        /// <summary>
        /// Play the footstep.
        /// </summary>
        public void PlayFootstep()
        {
            if (!GetClipsForSurface(GetSurfaceName())) return;

            AudioClip clip = null;

            switch (_footstepType)
            {
                case FootstepType.Walk:
                    if (walkSounds.Length > 0)
                    {
                        clip = walkSounds[Random.Range(0, walkSounds.Length)];
                    }
                    break;
                case FootstepType.Run:
                    if (runSounds.Length > 0)
                    {
                        clip = runSounds[Random.Range(0, runSounds.Length)];
                    }
                    else
                    {
                        if (walkSounds.Length > 0)
                        {
                            clip = walkSounds[Random.Range(0, walkSounds.Length)];
                        }
                    }
                        break;
                case FootstepType.Jump:
                    if (jumpSounds.Length > 0)
                    {
                        clip = jumpSounds[Random.Range(0, jumpSounds.Length)];
                    }
                    break;
                case FootstepType.Land:
                    if (landSounds.Length > 0)
                    {
                        clip = landSounds[Random.Range(0, landSounds.Length)];
                    }
                    break;
                case FootstepType.Slide:
                    if (slideSounds.Length > 0)
                    {
                        clip = slideSounds[Random.Range(0, slideSounds.Length)];
                    }
                    break;
                default:
                    if (walkSounds.Length > 0)
                    {
                        clip = walkSounds[Random.Range(0, walkSounds.Length)];
                    }
                    break;
            }

            if (clip && footstepSource)
            {
                footstepSource.PlayOneShot(clip);
                Debug.Log($"Play Footstep Clip ({clip.name})." );

            }
        }
        /// <summary>
        /// On draw gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_leftFootCollider)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(leftFootTransform.position, _leftFootCollider.radius);
            }

            if (_rightFootCollider)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rightFootTransform.position, _rightFootCollider.radius);
            }
        }

        /// <summary>
        /// The footstep types.
        /// </summary>
        public enum FootstepType
        {
            Walk = 0,
            Run = 1,
            Jump = 2,
            Land = 3, 
            Slide = 4
        }
    }

}