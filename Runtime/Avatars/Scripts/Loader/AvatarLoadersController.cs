using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Virtuademy.SDK.Core.Avatars
{
    /// <summary>
    /// Class that handles all the possible ways of loading the avatars.
    /// Checks in list order which avatar loader can istantiate the avatar with a given IAvatarConfig and returns that avatar loader
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarLoadersController", menuName = "Avatar/AvatarLoadersController", order = 1)]
    public class AvatarLoadersController : ScriptableObject
    {
        #region Public variables
        [Tooltip("ORDERED List of avatar loaders used to load the avatar")]
        public List<AvatarLoaderBase> avatarLoaders = new List<AvatarLoaderBase>();

        [SerializeField]
        private AvatarData defaultAvatarData;

        private AvatarLoaderBase currentLoader;
        #endregion

        #region Public methods
        public async Task<AvatarData> LoadAvatar(IAvatarConfig config)
        {
            foreach (AvatarLoaderBase avatarLoader in avatarLoaders)
            {
                if (avatarLoader.CheckAvatarConfig(config))
                {
                    // Each load owns its loader. The shared field only says which one reports
                    // progress: destroying through it, as this did, made a load that finished
                    // first destroy the loader of another one still running.
                    AvatarLoaderBase loader = Instantiate(avatarLoader);
                    currentLoader = loader;
                    try
                    {
                        AvatarData avatarData = await loader.LoadAvatar(config);
                        return avatarData;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error loading avatar: {e.Message}");
                        continue;
                    }
                    finally
                    {
                        if (currentLoader == loader)
                        {
                            currentLoader = null;
                        }
                        Destroy(loader);
                    }
                }
            }
            return defaultAvatarData;
        }

        internal float GetLoadingProgress()
        {
            if (currentLoader != null)
            {
                return currentLoader.GetLoadingProgress();
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
