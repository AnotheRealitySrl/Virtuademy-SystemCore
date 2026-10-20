
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;


using SPACS.Utilities;

namespace Virtuademy.SDK.Core.SystemFramework
{

    /// <summary>
    /// System manager instance.
    /// </summary>
    public class SM
    {
        #region Readiness

        public static UnityEvent OnAllSystemsSetupsDone = new UnityEvent();

        public static bool IsReady { get; private set; } = false;

        /// <summary>
        /// The systems whose initialization threw during the last <see cref="LoadAndSetup"/>,
        /// with their subsystems, which were not initialized. Empty when every system came up.
        /// </summary>
        /// <remarks>
        /// <see cref="IsReady"/> still becomes true when this is not empty: the setup has finished,
        /// and whoever waits for it (<see cref="DoOnceWhenReady"/>) must be told so. Until
        /// 2026-10 one throwing system stopped the sequence where it was, readiness never came,
        /// and the boot hung with only the exception in the log to say why.
        /// </remarks>
        public static IReadOnlyList<ISystem> FailedSystems => failedSystems;

        /// <summary>A system's initialization threw. Raised once per failed system, before readiness.</summary>
        public static UnityEvent<ISystem, Exception> OnSystemSetupFailed = new UnityEvent<ISystem, Exception>();

        private static readonly List<ISystem> failedSystems = new();

        public static void DoOnceWhenReady(UnityAction callback)
        {
            if (callback == null)
            {
                Debug.LogWarning("[SM] Trying to execute null callback!");
                return;
            }

            // In case of valid callback, manage its execution.
            if (IsReady)
            {
                callback.Invoke();
            }
            else
            {
                OnAllSystemsSetupsDone.AddListenerOnce(callback);
            }
        }

        #endregion


        #region Systems

        public static List<ISystem> CurrentSystems { get; set; } = new();

        /// <summary>
        /// Instantiated (if required) and initializes a list of systems
        /// </summary>
        /// <param name="systems"></param>
        /// <returns>
        /// Completes when every system has been set up, and never faults: a system that throws is
        /// logged, listed in <see cref="FailedSystems"/> and reported through
        /// <see cref="OnSystemSetupFailed"/>, and the systems after it are still set up.
        /// </returns>
        public static async Task LoadAndSetup(List<BaseSystem> systems)
        {
            IsReady = false;
            failedSystems.Clear();

            CurrentSystems = new List<ISystem>();
            try
            {
                for (int i = 0; i < systems.Count; i++)
                {
                    BaseSystem system = systems[i];
                    if (system != null)
                    {

                        BaseSystem systemInstance = system.RequiresNewInstance ? ScriptableObject.Instantiate(system) : system;
                        CurrentSystems.Add(systemInstance);
                        if (system.AutoInitAtStartup)
                        {
                            _ = await InitSystem(systemInstance, null);
                        }
                    }
                    else
                    {
                        // Warning! The system in position [i] is not valid!
                        Debug.LogWarning($"[SystemManager] System not valid in SystemManagerController, index [{i}].");
                    }
                }
            }
            catch (Exception e)
            {
                // InitSystem contains each system's own failure; this is for the loop itself
                // (an Instantiate that throws), which must not cost the readiness signal either.
                Debug.LogException(e);
            }

            //while (CurrentSystems.Exists(x => x.AutoInitAtStartup && !x.IsInit))
            //{
            //    Debug.Log($"System {string.Join("", CurrentSystems.Where(x => x.AutoInitAtStartup && !x.IsInit).Select(x => "|" + x.ToString() + "|").ToList())} not initialized yet");
            //    await Task.Yield();
            //}

            if (failedSystems.Count > 0)
            {
                Debug.LogError($"[SystemManager] {failedSystems.Count} system(s) failed to initialize: "
                               + string.Join(", ", failedSystems));
            }

            IsReady = true;

            OnAllSystemsSetupsDone?.Invoke();
        }

        /// <summary>
        /// Initializes a system
        /// </summary>
        /// <param name="systemToInitialize"></param>
        /// <param name="parentSystem"></param>
        /// <returns></returns>
        private static async Task<ISystem> InitSystem(ISystem systemToInitialize, ISystem parentSystem)
        {
            try
            {
                await systemToInitialize.InitInternal(parentSystem);
            }
            catch (Exception e)
            {
                // Its subsystems are skipped: they are initialized with this one as their parent.
                Debug.LogError($"[SystemManager] {systemToInitialize} failed to initialize; its subsystems are skipped.",
                               systemToInitialize as UnityEngine.Object);
                Debug.LogException(e, systemToInitialize as UnityEngine.Object);
                failedSystems.Add(systemToInitialize);
                try
                {
                    OnSystemSetupFailed?.Invoke(systemToInitialize, e);
                }
                catch (Exception listenerException)
                {
                    Debug.LogException(listenerException);
                }
                return systemToInitialize;
            }

            foreach (ISystem subSystem in systemToInitialize.SubSystems)
            {
                //if (subSystem.AutoInitAtStartup)
                //{
                if (subSystem == null)
                {
                    Debug.LogError("Found null subsystem inside system: " + systemToInitialize, systemToInitialize as UnityEngine.Object);
                    continue;
                }
                BaseSystem systemInstance = subSystem.RequiresNewInstance ? ScriptableObject.Instantiate(subSystem as BaseSystem) : subSystem as BaseSystem;
                CurrentSystems.Add(systemInstance);
                _ = await InitSystem(systemInstance, systemToInitialize);
                //}
            }
            return systemToInitialize;
        }

        /// <summary>
        /// Get an instantiated system of type T/>
        /// </summary>
        /// <remarks>
        /// <b>The constraint is <c>class</c> and not <c>ISystem</c> on purpose.</b> The lookup has
        /// always been "the registered instance assignable to T", and an instance is assignable to a
        /// contract whether or not that contract happens to derive from <see cref="ISystem"/>. The
        /// authoring package's contracts deliberately do not: a world is authored against interfaces
        /// that say what a thing does, not against the framework that hosts them. Requiring
        /// <c>ISystem</c> here would have forced the framework back into every one of them.
        /// <para>
        /// The looser constraint means <c>GetSystem&lt;Something&gt;()</c> for a type nothing
        /// registers now compiles and returns null, where it used to be a compile error. That is the
        /// price, and it is the same failure a wrong-but-legal type argument always had.
        /// </para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetSystem<T>() where T : class
        {
            ISystem returnSystem = CurrentSystems.Find(s => s.GetType() == typeof(T) || typeof(T).IsAssignableFrom(s.GetType()));
            return returnSystem as T;
        }
        /// <summary>
        /// Get an instantiated system of type T/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISystem GetSystem(ISystem system)
        {
            ISystem returnSystem = CurrentSystems.Find(s => s.GetType() == system.GetType() || system.GetType().IsAssignableFrom(s.GetType()));
            return returnSystem;
        }

        #endregion
    }

}
