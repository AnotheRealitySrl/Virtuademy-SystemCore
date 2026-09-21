#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System;

using UnityEngine;
using UnityEngine.Events;

using System.Threading.Tasks;

using Virtuademy.SDK.Core.SystemFramework;

using Virtuademy.SDK.Core;

namespace Virtuademy.SDK.Core.CharacterController
{
    [CreateAssetMenu(menuName = "Virtuademy/SDK-CharacterController/CharacterControllerBaseSystemConfig", fileName = "CharacterControllerBaseSystemConfig")]
    public class CharacterControllerSystem : BaseSystem, ICharacterControllerSystem
    {
        #region Inspector variables

        [Header("Initialization")]
        [SerializeField, Tooltip("Create a character controller instance on system init")]
        protected bool createCharacterControllerInstanceOnInit = true;

        [SerializeField, Tooltip("Is the character conroller already in scene or should be instantiated from a prefab?")]
        private bool characterControllerAlreadyInScene;

        [Header("Character controller instantiation")]
#if ODIN_INSPECTOR
        [HideIf(nameof(characterControllerAlreadyInScene))]
#endif
        [SerializeField, Tooltip("Reference to the character controller prefab")]
        protected CharacterControllerBase characterControllerPrefab;

#if ODIN_INSPECTOR
        [HideIf(nameof(characterControllerAlreadyInScene))]
#endif
        [SerializeField, Tooltip("Spawn position and rotation of the character controller")]
        protected Pose spawnPose;

        #endregion

        private int interactionCount = 0;

        // True when CharacterControllerInstance was Instantiate()d from the
        // prefab by this system (we own its lifetime). False when it was
        // found pre-placed in a loaded scene (the scene owns it; destroying
        // it would permanently remove a scene authoring node).
        private bool ownsInstance;

        #region Properties

        public CharacterControllerBase CharacterControllerInstance { get; protected set; }

        #endregion

        #region Unity Events

        public UnityEvent<CharacterBase> OnCharacterControllerSetupComplete { get; } = new();

        #endregion

        #region Interface implementation

        public override Task Init()
        {
            if (createCharacterControllerInstanceOnInit)
            {
                EnsureInstance();
            }
            return base.Init();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Ensures a character controller instance exists. Idempotent: if one already exists, this is a no-op.
        /// Scene managers call this on Load() so they own the CC lifecycle (instead of relying on system init).
        /// Honors <see cref="characterControllerAlreadyInScene"/>: when true, finds the CC pre-placed in the
        /// current scene (VR case); when false, instantiates from <see cref="characterControllerPrefab"/>.
        /// </summary>
        public virtual void EnsureInstance()
        {
            if (CharacterControllerInstance)
            {
                // Reactivate if hibernated; otherwise no-op.
                if (!CharacterControllerInstance.gameObject.activeSelf)
                {
                    CharacterControllerInstance.gameObject.SetActive(true);
                }
                return;
            }

            if (characterControllerAlreadyInScene)
            {
                // Include inactive: a previous DestroyCharacterControllerInstance
                // may have left the scene-placed CC disabled (we don't destroy
                // authored content). Pick it up, reactivate.
                CharacterControllerBase[] found = FindObjectsByType<CharacterControllerBase>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (found != null && found.Length > 0)
                {
                    CharacterControllerBase characterController = found[0];
                    if (!characterController.gameObject.activeSelf)
                    {
                        characterController.gameObject.SetActive(true);
                    }
                    CreateCharacterControllerInstance(characterController);
                }
                else
                {
                    throw new Exception("Character controller not found in scene");
                }
            }
            else
            {
                if (characterControllerPrefab)
                {
                    CreateCharacterControllerInstance(characterControllerPrefab);
                }
                else
                {
                    throw new Exception("Character controller prefab not specified");
                }
            }
        }

        public virtual void CreateCharacterControllerInstance(CharacterControllerBase characterController)
        {
            // Destroys the old character controller instance
            if (CharacterControllerInstance)
            {
                DestroyCharacterControllerInstance();
            }

            // Checks if the referenced character controller is already in scene
            bool fromPrefab = string.IsNullOrEmpty(characterController.gameObject.scene.name);
            CharacterControllerInstance = fromPrefab
                ? Instantiate(characterController, spawnPose.position, spawnPose.rotation).GetComponent<CharacterControllerBase>()
                : characterController;
            ownsInstance = fromPrefab;
        }

        public virtual void DestroyCharacterControllerInstance()
        {
            if (CharacterControllerInstance)
            {
                // SetActive(false) is synchronous: stops Update on every
                // MonoBehaviour in the CC hierarchy this frame. Without
                // this, end-of-frame Destroy() leaves one tick where
                // components like VirtuademyCamera3D.Update dereference
                // transforms that have already been marked destroyed by
                // the avatar/networking teardown earlier in Unload, NRE'ing.
                CharacterControllerInstance.gameObject.SetActive(false);

                // Only destroy if we instantiated from prefab. A scene-placed
                // CC is authored content; destroying it would permanently
                // remove it from its scene (re-entering that scene would not
                // bring it back) and break the next EnsureInstance() find.
                if (ownsInstance)
                {
                    Destroy(CharacterControllerInstance.gameObject);
                }
            }

            CharacterControllerInstance = null;
            ownsInstance = false;
        }

        /// <summary>
        /// Soft scene teardown of the CC: disables the GameObject so its
        /// Update/Physics stop, but keeps <see cref="CharacterControllerInstance"/>
        /// and any subsystem-cached references (e.g. CharacterControllerProSystem's
        /// virtuademyCinemachine / cinemachineBrain) intact. The next
        /// <see cref="EnsureInstance"/> call is a no-op except for reactivating
        /// the GameObject — much cheaper and safer than the full
        /// Destroy/Recreate cycle, which would tear down the cinemachine sub-
        /// hierarchy without recreating it.
        /// </summary>
        public virtual void HibernateInstance()
        {
            if (CharacterControllerInstance && CharacterControllerInstance.gameObject.activeSelf)
            {
                CharacterControllerInstance.gameObject.SetActive(false);
            }
        }

        public virtual void MoveCharacter(Pose newPose)
        {
            CharacterControllerInstance.transform.SetPositionAndRotation(newPose.position, newPose.rotation);
        }

        public virtual void ActivateReactionAnimation(string reactionName) { }

        public virtual void EnableCharacterMovement(bool value, InputSettings settings = null, bool setAsDefaultSettings = false) { }
        public virtual void DisableMovementAndRotation() { }

        public virtual void EnableCharacterJump(bool value, InputSettings settings = null, bool setAsDefaultSettings = false) { }

        public virtual void EnableCameraRotation(bool value, InputSettings settings = null, bool setAsDefaultSettings = false) { }

        public virtual void EnableCameraZoom(bool value, InputSettings settings = null, bool setAsDefaultSettings = false) { }

        public virtual void SetFirstPersonCameraMode() { }

        public virtual void SetThirdPersonCameraMode() { }

        public virtual Task GoToInteractState(Transform targetTransform, float maxZoom = 0.0001f, float minZoom = 1f, float maxYRotation = 85f, float minYRotation = -85f, float maxXRotation = 180f, float minXRotation = -180f, bool cameraInteraction = false) => Task.CompletedTask;

        public virtual Task MoveCameraToPoint(Transform targetTransform, float maxZoom = 0.0001f, float minZoom = 1f, float maxYRotation = 85f, float minYRotation = -85f, float maxXRotation = 180f, float minXRotation = -180f, bool cameraInteraction = false) => Task.CompletedTask;

        public virtual void ChangeCameraSpeed(float x, float y) { }
        public virtual Task GoToSetMovementState() => Task.CompletedTask;

        public virtual void EnableCharacterGravity(bool enable) { }

        public virtual void CreateDefaultSettings(InputSettings settings, bool setDeafultActive = true) { }
        public virtual void SetDefaultSettingsAsActive() { }
        public virtual void DisableAllButCamera(InputSettings settings) { }


        public virtual int ManageCounterCharacterInteraction(bool activate)
        {
            if (activate)
            {
                interactionCount++;
            }
            else
            {
                interactionCount--;
            }
            return interactionCount;
        }

        public virtual InputSettings GetCurrentSettings()
        {
            return null;
        }
        #endregion
    }

}
