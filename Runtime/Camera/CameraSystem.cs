using System;

using UnityEngine;
using UnityEngine.Events;

using Virtuademy.SDK.Core.SystemFramework;

namespace Virtuademy.SDK.Core.Cameras
{
    /// <summary>
    /// Abstract base for the camera rig system. Concrete implementations
    /// (e.g. Cinemachine-backed) live in the project / platform layer.
    ///
    /// Two ways to populate <see cref="RigInstance"/>:
    ///   1. <see cref="RegisterRig"/> — caller hands in a rig GameObject.
    ///      Used for Desktop/Mobile/WebGL (rig is inside CC prefab) and AR.
    ///   2. <see cref="EnsureInstance"/> + <see cref="rigAlreadyInScene"/> —
    ///      the system searches the loaded scenes for the rig. Used for VR
    ///      where the rig is pre-placed in the world scene.
    ///
    /// Subclasses override <see cref="OnRigRegistered"/>/<see cref="OnRigUnregistered"/>
    /// (to wire concrete sub-references), <see cref="OnBind"/>/<see cref="OnUnbind"/>
    /// (to wire follow target), and the camera-operation virtuals.
    /// </summary>
    public abstract class CameraSystem : BaseSystem, ICameraSystem
    {
        #region Inspector variables

        [Header("Initialization")]
        [SerializeField, Tooltip("If true, EnsureInstance searches the loaded scenes for the rig (VR pattern). Otherwise the caller is expected to RegisterRig.")]
        protected bool rigAlreadyInScene = false;

        #endregion

        #region Properties

        public GameObject RigInstance { get; protected set; }
        public virtual UnityEngine.Camera MainCamera { get; protected set; }
        public Transform FollowTarget { get; protected set; }

        #endregion

        #region Events

        public UnityEvent<Transform> OnFollowTargetChanged { get; } = new();

        #endregion

        #region Interface implementation

        public virtual void RegisterRig(GameObject rigInstance)
        {
            if (rigInstance == null)
            {
                throw new ArgumentNullException(nameof(rigInstance));
            }

            // Already registered with the same instance: just make sure it's active.
            if (RigInstance == rigInstance)
            {
                if (!RigInstance.activeSelf)
                {
                    RigInstance.SetActive(true);
                }
                return;
            }

            // Swap: release the old rig before installing the new one.
            if (RigInstance != null)
            {
                UnregisterRig();
            }

            RigInstance = rigInstance;
            if (!RigInstance.activeSelf)
            {
                RigInstance.SetActive(true);
            }
            ResolveSubReferences();
            OnRigRegistered();
        }

        public virtual void UnregisterRig()
        {
            if (FollowTarget != null)
            {
                Unbind();
            }
            OnRigUnregistered();
            RigInstance = null;
            MainCamera = null;
        }

        public virtual void EnsureInstance()
        {
            // Already registered: just reactivate if hibernated.
            if (RigInstance != null)
            {
                if (!RigInstance.activeSelf)
                {
                    RigInstance.SetActive(true);
                }
                return;
            }

            if (!rigAlreadyInScene)
            {
                // Desktop / Mobile / WebGL / AR path: rig comes from RegisterRig.
                // Nothing to do here; the host will register the rig explicitly.
                return;
            }

            // VR path: discover the rig in any loaded scene.
            GameObject found = null;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount && found == null; i++)
            {
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                found = FindRigInRoots(scene.GetRootGameObjects());
            }
            if (found == null)
            {
                throw new Exception("Camera rig not found in any loaded scene.");
            }
            RegisterRig(found);
        }

        public virtual void HibernateInstance()
        {
            if (RigInstance != null && RigInstance.activeSelf)
            {
                RigInstance.SetActive(false);
            }
        }

        public void Bind(Transform followTarget)
        {
            FollowTarget = followTarget;
            OnBind(followTarget);
            OnFollowTargetChanged?.Invoke(followTarget);
        }

        public void Unbind()
        {
            OnUnbind();
            FollowTarget = null;
            OnFollowTargetChanged?.Invoke(null);
        }

        #endregion

        #region Override points

        /// <summary>
        /// Subclasses override to discover the concrete sub-references
        /// (Cinemachine brain, virtual cameras, etc.) from the rig
        /// instance. Called by <see cref="RegisterRig"/>.
        /// Default resolves <see cref="MainCamera"/> via GetComponentInChildren.
        /// </summary>
        protected virtual void ResolveSubReferences()
        {
            if (RigInstance == null) return;
            MainCamera = RigInstance.GetComponentInChildren<UnityEngine.Camera>(true);
        }

        /// <summary>Hook called after a rig is registered + sub-references resolved.</summary>
        protected virtual void OnRigRegistered() { }

        /// <summary>Hook called before a rig is unregistered.</summary>
        protected virtual void OnRigUnregistered() { }

        /// <summary>Subclass binding hook (wire VCam tracking targets, etc).</summary>
        protected virtual void OnBind(Transform followTarget) { }

        /// <summary>Subclass unbinding hook.</summary>
        protected virtual void OnUnbind() { }

        public virtual void RealignAfterTeleport(Vector3 position, Quaternion rotation) { }
        public virtual void SetFirstPerson(bool firstPerson) { }
        public virtual void EnableRotationInput(bool enable) { }
        public virtual void EnableZoomInput(bool enable) { }
        public virtual void ChangeCameraSpeed(float x, float y) { }

        #endregion

        #region Helpers (VR scene-search path)

        protected virtual GameObject FindRigInRoots(GameObject[] roots)
        {
            // Default: pick the root whose hierarchy contains a Camera component.
            // Subclasses override to match a more specific marker (e.g. a
            // Virtuademy_CinemachineManager root).
            foreach (GameObject root in roots)
            {
                if (root == null) continue;
                UnityEngine.Camera cam = root.GetComponentInChildren<UnityEngine.Camera>(true);
                if (cam != null)
                {
                    return root;
                }
            }
            return null;
        }

        #endregion
    }
}
