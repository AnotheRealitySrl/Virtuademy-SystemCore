#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.SDK.Core.SystemFramework
{
    /// <summary>
    /// <see cref="SystemsManagerController"/>'s purpose is to reference the systems that need to be created and initialized by the <see cref="SM" (Systems Manager)/>. 
    /// During Awake, each <see cref="BaseSystem"/> scriptable that is added to the <see cref="systems"/> is sent to the <see cref="SM"/> for creation and initialziation.
    /// </summary>
    public class SystemsManagerController : MonoBehaviour
    {
        [SerializeField] private bool loadOnAwake = false;

#if ODIN_INSPECTOR
        [InlineEditor]
#endif
        [SerializeField] private List<BaseSystem> systems = new();

        private void Awake()
        {
            if (loadOnAwake)
            {
                LoadAndSetup();
            }
        }

        public void LoadAndSetup()
        {
            // SM.LoadAndSetup never faults: failures are reported through SM.FailedSystems.
            _ = SM.LoadAndSetup(systems);
        }
    }
}