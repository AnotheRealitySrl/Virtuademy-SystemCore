using Virtuademy.SDK.Core.SystemFramework;

using UnityEngine;
using UnityEngine.Events;

namespace Virtuademy.SDK.Core.Cameras
{
    /// <summary>
    /// Camera system: owns the application's main camera rig independently
    /// from the character controller.
    ///
    /// Two binding modes:
    ///   - <see cref="RegisterRig"/>: caller hands in an existing rig
    ///     GameObject. Used for Desktop / Mobile / WebGL / AR where the rig
    ///     lives inside the CC prefab (or an AR rig), and the host system
    ///     explicitly announces which one is the active camera.
    ///   - Scene search (when the rig is already placed in a loaded scene,
    ///     typical for VR): the system finds the rig itself via
    ///     <c>EnsureInstance</c> + <c>rigAlreadyInScene</c>.
    ///
    /// Lifecycle:
    ///   - <see cref="EnsureInstance"/> reactivates a hibernated rig or
    ///     (VR-only) discovers it in scene. No-op when no rig is yet
    ///     registered (Desktop/Mobile pre-bind state).
    ///   - <see cref="HibernateInstance"/> SetActive(false) on the rig.
    ///
    /// Follow target binding is decoupled from rig registration:
    ///   - <see cref="Bind"/> wires a follow target (typically the CC's
    ///     HeadReference) into the registered rig's tracking machinery.
    ///   - <see cref="Unbind"/> detaches without releasing the rig.
    /// </summary>
    public interface ICameraSystem : ISystem
    {
        /// <summary>The currently-active rig camera.</summary>
        UnityEngine.Camera MainCamera { get; }

        /// <summary>The transform the camera currently follows; null when unbound.</summary>
        Transform FollowTarget { get; }

        /// <summary>Fired after Bind/Unbind so dependents can refresh references.</summary>
        UnityEvent<Transform> OnFollowTargetChanged { get; }

        /// <summary>
        /// Register an existing rig GameObject as the active camera rig.
        /// Replaces any previously-registered rig. The system does NOT own
        /// the rig's lifetime — the caller (CC, AR controller, ...) keeps it.
        /// </summary>
        void RegisterRig(GameObject rigInstance);

        /// <summary>Release the currently-registered rig without destroying it.</summary>
        void UnregisterRig();

        /// <summary>
        /// Reactivate a hibernated rig. For VR only: if no rig is registered
        /// and <c>rigAlreadyInScene</c> is set, search the loaded scenes.
        /// Otherwise no-op: the caller is expected to RegisterRig.
        /// </summary>
        void EnsureInstance();

        /// <summary>
        /// Soft teardown: SetActive(false) on the rig so its Update/render
        /// stops, but keep all references intact for cheap reactivation.
        /// </summary>
        void HibernateInstance();

        /// <summary>
        /// Bind the camera to a follow target (e.g. the CC's HeadReference).
        /// Concrete implementations route this into their tracking machinery
        /// (Cinemachine virtual camera targets, VirtuademyCamera3D distance
        /// target, etc).
        /// </summary>
        void Bind(Transform followTarget);

        /// <summary>Detach from the current follow target; safe to call when unbound.</summary>
        void Unbind();

        /// <summary>
        /// Resync the camera to a freshly-teleported follow target. Implementations
        /// typically reset the orbital horizontal axis so the camera doesn't
        /// snap to a stale orientation when the host (CC) teleports.
        /// </summary>
        void RealignAfterTeleport(Vector3 position, Quaternion rotation);

        /// <summary>First-person vs third-person mode switch.</summary>
        void SetFirstPerson(bool firstPerson);

        /// <summary>Enable/disable rotation input on the camera.</summary>
        void EnableRotationInput(bool enable);

        /// <summary>Enable/disable zoom input on the camera.</summary>
        void EnableZoomInput(bool enable);

        /// <summary>Per-axis camera speed tuning.</summary>
        void ChangeCameraSpeed(float x, float y);
    }
}
