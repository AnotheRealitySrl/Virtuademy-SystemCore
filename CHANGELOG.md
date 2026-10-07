# Release notes

## Unreleased

### Fixed
- The URP Volume fading sample did not compile: it imported `Virtuademy.SDK.Utilities`, which no
  longer exists, could not see `IFadeManager` after the namespace rename, and did not implement
  `FadeToBackground`, `FadeFromBackground` and `SetTargetCamera`. It now carries the implementation
  the application's imported copy already had.
- The QueryStringParser sample named `StringExtensions` ambiguously; it now names
  `SPACS.Utilities.StringExtensions`.

### Removed
- `HttpSystemConfig.asset` and every reference to it from the QueryStringParser sample: `HttpSystem`
  was removed in 2026-04, so the scene's systems list opened with a missing script.

## v16.0.0

### Added
- Added `ICameraSystem` (`Virtuademy.SDK.Core.Cameras`): rig registration and hibernation, follow-target binding, realignment after teleport, first-person toggle and rotation-input gating.
- Added `IsEmbedded` to `IApplicationManager`, so systems can tell an embedded host from a standalone run.
- `ICharacterControllerSystem` gained members for the camera and layer work above.

### Changed
- **Renamed.** The package id becomes `com.anotherealitysrl.virtuademy-systemcore` and the three
  assemblies become `Virtuademy.SystemCore`, `.Editor` and `.I2Loc`. The name `Virtuademy.SDK.Core`
  went to the contracts package in the same wave, which is why the two moves are one change: for
  one commit the old and the new name would collide. This package is the application framework —
  systems, managers, UI kit — and is on its way to being private, so it takes the name that says so.

  **Namespaces are untouched.** The types still declare `Virtuademy.SDK.Core.*`, and no file in this
  package declares `Virtuademy.SystemCore`. The asmdefs were edited field by field rather than by
  substitution, because in three of them the assembly name and the root namespace are the same string.
- The `displayName` is now `Virtuademy-SystemCore`, hyphenated like every sibling package.
- The player layer now follows mesh display, so avatar interactions can be enabled and disabled coherently.

### Note
- Major bump: the additions to `IApplicationManager` and `ICharacterControllerSystem` are breaking for any external implementer of those interfaces, even though nothing was removed.

## v15.0.0

### Added

- Added UI toolkit localization types

### Changed

- Refactor Api classes to static classes instead of systems
- Renamed AvatarConfigControllers

## v14.0.0

### Added

- Added cinemachine
- Added new input settings for character

## v13.2.0

### Changed

### Added

- ApplicationManagement: added support, sample and documentation for external apps integrated into Virtuademy.
- Utilities: added querystring parsers which retrieve the query parameters in Android and WebGL apps.
- New ApiSystem module which contains a base system providing base feature for implementing a reflectis-connected API.
- New HTTP and WebSocket modules, merged from their own modules.
- Networking: add new `SetCustomDomain` method.

### Fixed

- SystemFramework: fixed `CurrentSystems` property initialization in `SM`.
- Utilities: improvements on custom type utility.

## v13.1.0

### Added

- Transitions: added `AddTransitionProvider` and `RemoveTransitionProvider` methods in `MultipleTransitionProvider`.

## v13.0.0

### Changed

- Avatars: changed `GetAvatarLoader` method into `LoadAvatar` in `IAvatarConfigController`.

### Added

- Avatars: added `GetLoadingProgress()` method in `IAvatarConfigController`.
- Utilities: added `ForceRefresh` method in `CacheVariable`.
- Network: added `GetSharedNetworkTime()` network in `INetworkingSystem`.

### Fixed

- Fade: fixed a missing reference in `CanvasFadeSystem` configuration.
- Avatars: improved avatar loading logic.
- SystemFramework: added null check on systems initialization.
- Utilities: fixed issue in `ImageDownloaded`.
- Transition: fix on transform interpolator transition provider.

## v12.1.1

### Added

- Fixed import documentation static method's tooltip for better clarity in menu bar.

## v12.1.0

### Added

- Visual scripting: added base unit class that create new custom type istances.

## v12.0.0

### Changed

- Changed package name, from Reflectis-SDK to Reflecits-SDK-Core, and updated namespaces according to new package name.
- Revised package's folder structure to match Unity guidelines. Removed also the multiple asmdefs, one for each module.
- Utilities: replaced `Pointer_stringify` with `UTF8ToString` in jslib utilities.
- ClientModels: added `isMultiplayer` parameter in `PingMyOnlinePresence` method in `IClientModelSystem`.
- ClientModels: changed type parameter in `UpdateSavedAssets` to string.
- Utilities: removed API utilities (`ApiResponse`, `JsonArrayHelper`). They are moved to their own HTTP module.
- ClientModels: moved MaxCCU from `CMWorld` to `CMWorldConfig`.
- ClientModels: changed `CMTemplateObj` in `SpawnedObjectData`, removed `Erasable` and `Label` properties from it.

### Removed

- Removed ClientModels, Help, ObjectSpawner, ApplicationManagement and SceneObjects modules, in order to put them in CreatorKit.
  ApplicationManagement has been split in two different parts, the one not related directly with CreatorKit has been maintained.
- ClientModels: removed dependecies to spawned object from `IClientModelSystem`.
- ClientModels: removed network, text and voice app ids from `CMWorld`.

### Added

- ObjectSpawner: added max fov parameter to spawn position data in `SpawnData`.
- WebSocket: implemented web socket listeners. Added callbacks to `IWebSocketSystem`.
- VoiceChat: added `DisconnectFromService` method definition in `ITextChatSystem`.
- VoiceChat: Added `DisconnectFromService` method definition in `IVoiceChatSystem` and base implementation in `VoiceChatSystenBase`.
- ClientModels: added `CurrentCCUCount` in `CMWorldConfig`.

## v11.1.1

### Fixed

- Fixed package version in package.json

## v11.1.0

### Added

- Platform: added signature for `LoadDefaultEvent` method in `IReflectisApplicationManager`.

## v11.0.0

### Changed

- Changed `RuntimePlatform` enum into `ESupportedPlatform` and specified Android in VR.

### Added

- New WebSocket module, with an interface system that allows to implement a custom socket communication.
- New ChatBot module, with an interface system that allows to implement communication with AI services and implement a chatbot.
- Networking: the `Networking System` is exposing the boolean property `LocalPlayerId`. This read-only property returns the player ID (i.e. Photon's actor number) of the local player. Player ID is a unique identifier for the user in the current shard context.
- ClientModels: added supported platform data to `CMEnvironment`
- Interaction: Submesh generation in explodable objects is now cached, to reduce computational load.
- Utilities: added `RenameAttribute` which can be used to rename inspector fields.
- Utilities: added awaitable `DownloadImageAsync` method in `ImageDownlaoder` utility script.

### Removed

- Avatar: removed `OnBeforeIstantiation` callback, changed `AvatarData` structure to include prefab.

## v10.1.0

### Added

- Networking: added a new bool field `isShardClosed` to ping method data (`IClientModelSystem.PingMyOnlinePresence`), online user presence classes (`CMOnlinePresence` and `OnlinePresenceDTO`)and shard data class (`CMShard`). These fields are used to implement shard closing: the flag is checked when Reflectis client has to detect if an event shard is joinable or a new one should be created and joined.
- Networking: new `Networking System` module. This will be used to expose events and behaviours related to Photon's networking in the Reflectis context.
- Networking: the `Networking System` is hosting the events `OtherPlayerJoinedShard` and `OtherPlayerLeftShard`, that can be used to detect players entering/leaving the Reflectis event where the local player is currently staying. These events have two integer parameters, respectively the User ID (from the Reflectis profile) and the actor number of the user that just entered/left the shard.
- Networking: the `Networking System` is exposing the methods `OpenCurrentShard` and `CloseCurrentShard`. These can be used to respectively open or close the shard where the local player is. A closed shard will prevent other player from joining it, like if the shard has reached max capacity. In case the shard event is set as "unlimited", player entering after the shard has been closed will just land in a new shard.
- Networking: the `Networking System` is exposing the boolean property `IsCurrentShardOpen`. This read-only property returns true if the current shard is open, false if the current shard is closed.
- Added signature for the `LoadEvent` method in `IReflectisApplicationManager` interface. This will allow accessing the implemented method via `Virtuademy.SDK.ApplicationManagement` module.

## v10.0.0

### Changed

- Interaction: changed the `hasHoveredState` variable of `GenericInteractable` from private to protected.
- VisualScripting: renamed `AwaitableEventNode` class into `AwaitableEventUnit`.

### Added

- Added new VideoChat module, providing the interfaces needed to implement video/audio RTC communications.
- Added new Diagnostics module to collect data within events and more specifically trainings inside events.
- ApplicationManagement: added new method `InitializeObject` in `IReflectisApplicationManager`. This can be called to initialize a placeholder on an object that wasn't part of the environment and was instantiated dynamically.
- ApplicationManagement: added `IsNetworkMaster` property to `IReflectisApplicationManager` interface.
- Avatars: added new method `EnableOtherAvatarsMeshes`. It can be used to hide/show other player's avatars.
- Avatars: added new public variable `OtherAvatarsConfigControllers`. It's a dictionary containing a collection of references to the `IAvatarConfigController` components on each of the non-local player's avatars present in the current event (updated in real time). The key for each entry in the dictionary is the actor number of the player controlling the related avatar.
- ClientModels: added `CanWrite` property to `CMEvent`.
- Interaction: added different types of hover events in GenericInteractable
- Utilities: added `InspectorButton` property drawer.
- Utilities: added `GuidGenerator` utility class.
- Utilities: added compression and encryption methods to `StringUtilities`.
- VisualScripting: added `AwaitableUnit` class.

### Fixed

- Avatars: fixed VR avatar's show/hide logic in `AvatarConfigControllerVR`.
- Interaction: fixed some issues regarding hover on interactable behaviours.
- TextChat: changed the encoding of chat messages from ASCII to UTF8.

## v9.1.0

### Added

- Utilities: added extension class for `BinaryWriter` and `BinaryReader` to allow reading and writing of custom types such as Vector2, Vector3, objects ...
- Utilities: added extension class for `Component` and implemented the method `GetComponentInactive` that allows to do a GetComponent on inactive objects.

### Fixed

- TextChat: changed `ChatMessage` serialization method from XML to JSON, to improve readability and performance.

## v9.0.0

### Added

- ClientModels: add method `GetEventPermissionsByTag` in `IClientModelSystem`.
- CharacterController: added `SetFirstPersonCameraMode` and `SetThirdPersonCameraMode` methods to `ICharacterControllerSystem`, they can be called to switch the character controller to first person or third person view respectively.
- Interaction: added `OnSelectedInteractableChange` event to `IGenericInteractionSystem`. This method is fired whenever the current selected generic interactable changes, and its argument is a reference to the new selected item. The event is also fired when the user clicks on an empty area, in which case the reference is null.
- TextChat: added `GetMessageLocalTime` utility method in `ChatMessage`.
- Utilities: extended features of `WaypointPositioner` to add movement lerp and more editor scripting.

### Changed

- ClientModels: refactored `CMPermission` entity.
- ClientModels: merged `IsEventPermissionGranted` and `IsWorldPermissionGranted` methods of `IClientModelSystem` into a single method `IsPermissionGranted`.
- ClientModels: changed `GetWorldPermissions` method name into `GetMyWorldPermissions`.
- ClientModels: changed `GetEventPermissions` method name into `GetAllPermissionsByTag`.
- ClientModels: `GetMyWorldPermissions` and `GetMyEventPermissions` methods of return a list of `EfacetIdentifier` instead of a list of `CMPermission`, same for properties `CurrentEventPermissions` and `WorldPermissions` which are now a list of `EfacetIdentifier`.
- ClientModels: changed `CanRead` field of CMEvent to `CanVisualize`.

### Removed

- ClientModels: removed `CMFacet` entity and `GetFacets` method from `IClientModelSystem`.
- VoiceChat: removed `MuteUser` method from `IVoiceChatSystem`.

### Fixed

- Avatars: fixed parent animator null check during rebind operation in `AvatarConfigControllerDesktop`.
- Interaction: fixed hover issue with `Manipulable`.

## v8.1.2

### Fixed

- ClientModels: fixed `CacheVariable`'s auto-refresh coroutine not being stopped properly, causing multiple coroutines to be active simoultanously.
- Interaction: fixed `EContextualMenuOption` enum values by adding `None`.

## v8.1.1

### Fixed

- VoiceChat: fixed redundant disconnection from a voice channel in the base implementation of `DisposeEngine` method of `VoiceChatSystemBase`.

## v8.1.0

### Added

- ClientModels: added `GetCachedEventShards` method to `IClientModelSystem` to retrieved cached shards data.
- VoiceChat: added `OnDisconnected` callback to `VoiceChatSystemBase`.

## v8.0.0

### Added

- ClientModels: added `InvalidateCache` method to `IClientModelSsytem`.
- ClientModels: added "System" value to `EFacetGroup` enum in `CMFacet`.

### Removed

- ClientModels: removed `PlayerImageUrl` from `CMUser`.

### Fixed

- Interaction: fixed wrong calculation of `CurrentBlockedState` of `Manipulable` which happens in `SetBlockByPermission` method.

## v7.0.0

### Added

- New BrowserCommunicationSystem module, for exchanging messages between Unity client and JavaScript container application.
- New RadialMenuUtils module, which is a menu to spawn `Manipulable` objects in VR.
- New ApplicationManagement module, with interfaces useful to manage the application flow in an agnostic way.
- Avatars: added `EnableAvatarIstance` method to `IAvatarSystem`, with base implementation in `AvatarSystem`.
- CharacterController: added `EnableCharacterGravity` method to `ICharacterControllerSystem`.
- ClientModels: added `IReflectisApplicationManager`.
- ClientModels: added `EnableCacheAutorefresh` to `IClientModelSystem` (starts refreshing all the variables that refresh automatically, like online user presence).
- ClientModels: added `CheckMyKeys` and `CheckScheduleAccessibilityForToday` methods in `IClientModelSystem`.
- ClientModels: added return value to `PingMyOnlinePresence` method in `IClientModelSystem`.
- ClientModels: added `avatarId` field to `CMOnlinePresence` class.
- Fade: added `SetTargetCamera` method to `FadeManager`.
- Transitions: added `CanvasInterpolatorTransitionProvider`, `TransformInterpolatorTransitionProvider`, `AbstractInterpolatorColorTransitionProvider` and `AbstractInterpolatorFloatTransitionProvider`.
- Utilities: added `AnimationCurveExtensions` utility class with `GetInverseCurve` and `GetAngularCoefficient` static methods.
- Utilities: added `Interpolator` wrappers to implement animations and interpolations.
- Utilities: added `RemoveSpaces` extension method in `StringExtensions`.
- Utilities: added `CacheVariable` class for caching CM entities.
- VoiceChat: added `enableMicrophoneByDefault` inspector variable in `VoiceChatSystemBase`.

### Changed

- Moved `IApplicationManager` interface from Utilities module to its own new ApplicationManagement module.
- ClientModels: refactored online users presence methods in `IClientModelSystem`.
- ClientModels: added `eventInvitationMessage` parameter to `InviteUsersToEvent` method in `IClientModelSystem`.
- ClientModels: renamed `GetMyUserPreference` method to `GetMyUserPreferences`, remove `myUserId` parameter.
- ClientModels: changed `DeleteEvent` method return type to `long`.
- ClientModels: removed return type from `UpdateUserPreference` method of `IClientModelSystem`.
- Transitions: added "Interpolator" to some transition provider class names.
- Utilities: `CheckUserInternetConnection` method of `NetworkUtilities` class now calls a method of the `IApplicationManager` interface in WebGL, allowing a custom implementation in absence of an implementation of a ping.

### Removed

- CharacterController: removed AvatarPlaceholder prefab.
- Transitions: removed dependency to DOTween, removed `CanvasTransitionProvider`, `TransformTransitionProvider`, `AbstractColorTransitionProvider` and `AbstractFloatTransitionProvider`.
- ObjectSpawner: removed dependency to CreatorKit logic from `ObjectSpawnerSystem`.
- ClientModels: removed `IsShardFull` method from `IClientModelSystem`.
- ClientModels: removed `CalculateShard` method from `IClientModelSystem`.
- ClientModels: removed `GetOnlineUsers` duplicate method from `IClientModelSystem`.

### Fixed

- Avatars: add null check in `EnableAvatarInstance` method of `AvatarSystem`.
- Transitions: fixed `GetStartTime` method in transition providers, other fixes.

## v6.1.0

### Added

- ClientModels: added optional `worldId` parameter to `PingMyOnlinePresence` method in `IClientModelSystem`.
- ClientModels: added `GetUserTags` method in `IClientModelSystem`.
- ClientModels: added `AssetControlManager` facets in `EFacetGroup`.
- Interaction: added asset managemement permission check in `Manipulable` and Contextual menu prefab.

## v6.0.0

### Changed

- ClientModels: changed `GetPlayerData` method name to `GetMyUserData` in `IClientModelSystem`.
- ClientModels: added `worldId` int parameter to `GetDefaultWorldEvent` signature.

### Added

- Add new `SceneHierarchy` module for better scene objects grouping and management.
- Avatars: added `Finish` method in `AvatarSystem` to unload the current avatar.
- Avatars: added `EnableSpeaker` value in `EFacetGroup` enum of `CMFacet`.
- Avatars: added `VoiceChatSource` in `AvatarControllerBase`.
- Avatars: added `AvatarConfigBase` class.
- Avatars: implemented height management for RPM avatars, added `CalibrateAvatar` method in `ICharacter` and `IAvatarConfigController`.
- ClientModels: added `GetWorld` method in `IClientModelSystem`.
- ClientModels: added `Description` and `ThumbnailUri` field to `CMWorld`.
- ClientModels: added `UserPrefs` to `IClientModelSystem`.
- ClientModels: added NickName property in CMUser in order to differentiate the name of a User from its DisplayName.
- ClientModels: added `GetUserCode` method in `IClientModelSystem`.
- ClientModels: added `Height` property in `CMUserPreference`.
- ClientModels: added `LocalizationName` and `LocalizationCSV` to `CMEnvironment`.
- ClientModels: Added `UserPrefs` property in `IClientModelSystem`.
- Interaction: added `OnGrabManipulableStart`, `OnGrabManipulableEnd`, `OnRayGrabManipulableStart` and `OnRayGrabManipulableEnd` events in `Manipulable`.

### Removed

- ClientModels: Remove `DefaultEvent` property from `IClientModelSystem`.

### Fixed

- Update Addressables package dependency to version 1.21.20.
- Avatars: added `EnableAvatarInstanceLabel` in `IAvatarSystem` (with implementation in `AvatarSystem`) and `EnableAvatarLabel` in `IAvatarConfigController` (with implementation in `AvatarControllerBase` and `AvatarControllerDesktop`).
- Avatars: fixed issue of avatar not being attached to character controller on setup, in the case the avatar is setupped automatically.
- ClientModels: changed some entries in `EFacetIdentifier` enum, added missing entries.
- Fade: fixed set float method in `CanvasFadeManager` which prevented the fade to be completed.
- Interaction: fixed search of disabled `InteractableRef`s in `InteractableBehaviourBase`.
- Interaction: added a minimum treshold under which a `Manipulable` can be moved in WebGL.
- Interaction: Fixed networked contextual menu option and removed hard reference to bounding box collider renamed scene hierarchy to scene objects.
- Interaction: Fixed missing reference on `Manipulable`'s object center getter.

## v5.0.0

### Changed

- Avatars: added references to network avatar prefabs.
- ClientModels: changed saved color in `CMTemplateObj` from Color to string, to allow for an empty/null color value.
- ColorPicker: renamed method used to load saved model color state, from `AssignColorToPicker` to `AssignSavedColorToPicker`.
- Core: systems initialization is now awaitable.
- Interaction: `GenericInteractable` now accepts script machines instead of scriptable actions to define custom logic.
- Interaction: `ContextualMenuManageable`'s dictionary with button ids as keys now has `UnityAction`s as values instead of `UnityEvent`s.
- ModelExploder: renamed method used to load saved model explosion state, from `AssignExplosionToModelExploder` to `AssignSavedExplosionToModelExploder`.
- Utilities: renamed `ReflectionExtensions` to `ReflectionUtilities`.

### Added

- Added new Popup module with `IPopupSystem` interface.
- ClientModels: added `Catalog` property to `CMEnvironment`.
- ClientModels: added `Email` property to `CMUser`.
- ClientModels: added `WorldId` property to `CMEvent`.
- ClientModels: added `CMCatalog` entity and `GetWorldCatalogs(int worldId)` in `IClientModelSystem`.
- ColorPicker: added optional boolean parameter `networkedContext` to `AssignColorPicker`, to allow for offline/networked color changer component setup.
- Interaction: added new visual scripting nodes - generic interactable event nodes and expose node.
- Interaction: implement logic of object destroying on `ContextualMenuManageable`.
- Interaction: added new public property `IsSubMesh` in `Manipulable`, to check if the `Manipulable` is applied to a submesh.
- Interaction: added new public property `RootManipulable` in `Manipulable`, which returns the component at the root of the interactable object, whether the `Manipulable` reference is on the root of the interactable object or on one of its submeshes.
- ModelExploder: added optional boolean parameter `networkedContext` to `AssignModelExploder`, to allow for offline/networked model exploder component setup.
- Utilities: added new `IApplicationManager` interface that defines application management methods (`QuitApplication`, `ErasePlayerSessionData`, `HideEverithing`).
- Utilities: added new `NetworkUtilities` which contains static utilitiy methods such as `CheckUserInternetConnection`.
- Utilities: added `GetFieldsRecurse`, `GetPropertiesRecurse` and `GetMethodRecurse` in `ReflectionUtilities`.

### Removed

- ClientModels: removed `CMEnvironmentNameComparer` in `CMEnvironment`.
- Interaction: removed awaitable scriptable actions.
- Interaction: removed legacy `IDesktopInteractionSystem` interface.

### Fixed

- Interaction: fixed `Manipulable`'s `BoundingBox` initialization.
- Interaction: updated `ObjectCenter` and `ObjectSize` properties of `Manipulable` to be more coherent in checking if the `Manipulable` is on a submesh or not.

## v4.0.0

### Changed

- Avatars: refactored loading logic, `AvatarLoaderBase` is the base class for the implementation of any avatar loader and `AvatarLoadersController` handles the choice of the avatar loader.
- Avatars: renamed `AvatarConfigChanged` event to `OnPlayerAvatarConfigChanged`.
- Core: moved auto-initialization of systems to root systems instead of sub-systems.
- Voice: Added Photon global channel and id to communication channels. `CommunicationChannel` constructor is changed to add the id as parameter.
- Utilities: `ContentSearch` parameter of `ApiResponse` is not generic anymore.

### Added

- New ClientModels module, providing the data model used by the business logic and an API definition for data access.
- New TextChat module.
- New Audio module, which allows to manage volume and other audio-specific features.
- New Notification module.
- New Faq module.
- New ObjectSpawner module.
- New Interaction module.
- New Platform module to know in which platform the application is running.
- New Help module useful to implement tutorials.
- New ColorPicker module with base logic for changing the color of an interactable.
- New ModelExploder module with base logic for exploding a 3D asset.
- Avatars: Implemented avatar loading logic in `AvatarLoadersController`.
- Avatars: added `IAvatarConfigController` reference associated with the avatar instance in `IAvatarSystem`.
- Avatars: added `AvatarId` and `AvatarPNG` properties in `IAvatarConfig`.
- CharacterController: `EnableCharacterMovement`, `EnableCharacterJump`, `GoToInteractState`, `GoToSetMovementState`, `EnableCameraRotation` and `EnableCameraZoom` methods in `ICharacterControllerSystem`
  interface and `CharacterControllerSystem` base class. These methods allow to toggle character controller interaction.
- CharacterController: added AvatarPlacheholder prefab: a purple full-body rigged avatar for general purpose.
- CharacterController: add finger bones, references to interactor colliders, to tag and to player height to `CharacterBase`.
- CharacterController: Added logic to adjust the position of the label above the player.
- Core: Implemented `GetSystem` overload with `ISystem` as type parameter.
- Core: `IsReady` bool property on `SM` to know if a system is ready, and add a static method `DoOnceReady` to trigger a callback once `SM` is has finished setup.
- Fade: immplemented fade with a quad.
- Fade: added `FadeToBackground` and `FadeFromBackground` in `IFadeManager`, which use a `backgroundImage` field in `CanvasFadeManager` and a `backgroundColor` field in `QuadFadeManager`.
- Fade: added fade in/out via material in `CanvasFadeManager`.
- Transitions: implemented `OnEnter` and `OnExit` transition callbacks.
- Transitions: added `TransformTransitionProvider`, `AbstractColorTransitionProvider`, and `FloatTransitionProvider`.
- Transitions: added `DoEnterExitTransitionAsync` method in abstract transition provider, which calls `DoTransitionAsync(true)` and `DoTransitionAsync(true)` in sequence.
- Transitions: added `onEnterTransitionStart` callback and `onExitTransitionFinish` on transition providers.
- Utilities: added Image downloader utility script.
- Utilities: added `ColorExtensions` extension class with `IsVerySimilarToWhite`, `NameToColor` and `HexToColor` methods.
- Utilities: added `DateTimeExtensions` extension class with `ParseStringToDate` method.
- Utilities: added `StringExtensions` extension class with `CopyToClipboard`, `RemoveSpecialCharacters` and `ConcatenateStrings` method.
- Utilities: added `TextAssetExtensions` extension class with `GetTextClassCompleteName` method.
- Utilities: added `GetTextType` and `GetTypeFromString` methods.
- Utilities: added `RecompileProjectEditorWindow` and `RestartProjectEditorWindow`.
- Utilities: added `ResourceLockHandler<T>` utility script.
- Utilities: added `DictionaryExtensions` extension class with `ToDictionary` method which converts an object to a dictionary and `CustomToString` method for debugging purposes.
- Utilities: added `DrawIf` and `OnChangedCall` and `HelpBox` property drawers.
- Utilities: added `IntExtensions` extensions class with `RoundUpDivision` method.
- Utilities: added `totalCount` optional parameter in `ApiResponseArray` constructor.
- Utilities: added `AudioMixerExtension` extension class with `GetLinearFloat` and `DecibelToLinear` methods.
- VoiceChat: added `MuteLocalUser(bool muteAudio)` overload and `GetVoiceDetection()` method in `VoiceChatSystemBase`.

### Deprecated

- Legacy Interaction module (working with old Toast System)

### Removed

- Synchronous scriptable actions working with legacy Interaction module

### Fixed

- Added missing dependencies in package.json
- Updated assembly files names
- Avatars: added avatar existence checks on `AvatarConfigControllerDesktop` to prevent null reference exceptions.
- Avatars: improved avatar activation/deactivation.
- Avatars: add check on avatar bounds for a better display of the label.
- Avatars: added app execution check in `EnableAvatarMeshes` method of `AvatarControllerDesktop`
- Avatars: stored reference of current config in `AvatarConfigControllerBase`
- Avatars: fixed avatar animation issues
- Avatars: fixed layer mask issue
- CharacterController: improved character interaction activation/deactivation.
- Fade: added `CanvasScaler` on fade canvas
- Fade: fixed scale issue of the `CanvasFadeManagerDesktop`
- Transitions: `CanvasTransitionProvider` now kills the active transition before executing the new one
- Transitions: improve `AnimationTransitionProvider` behaviour and added null checks in all transition providers
- Utilities: fixed private reference to `TextMeshPro` item in `TMProUGUIHyperlinks`.
- Utilities: replaced `Parse` occurrences in `ApiResponse` with a more safe `TryParse` method
- Utilities: changed `WaitForSeconds` instruction to `WaitForSecondsRealtime` in `MonoBehaviourExtensions`, due to WebGL issues

## v3.0.0

### Changed

- Massive refactor SPACS -> Reflectis
- Utilities: merged Extensions module into Utilities one
- Fade: renamed "FadeSystem" occurrences (asmdef, namespaces, etc.) in "Fade"
- VoiceChat: renamed "Communication" occurrences (asmdef, namespaces, etc.) in "VoiceChat"
- CharacterController: change methods signatures in `ICharacterControllerSystem` API for better clarity
- Avatar: change methods signatures in `IAvatarSystem` API for better clarity

### Added

- Core module (moved from SPACS.Core)
- New Interaction module, with interactable objects base logic and scriptable actions from Core module, and with new `IDesktopInteractionSystem` interface
- New Object Spawner system
- Avatars: Spawn method in AvatarSystem's API
- Avatars: onBeforeAction and onAfterAction callbacks on avatar customization change
- Transitions: flag for performing reverse transitions in `AbstractTransitionsProvider`
- Transitions: `AnimatorTransitionProvider` now has a configurable string parameter
- Communication system: system base class
- Documentation
- Fade System with canvas

### Fixed

- Fade: condition that checks when all scenes have been loaded/unloaded

---

## v2.0.0

### Changed

- Migrate to Reflectis system framework

### Added

- Fade system (with URP volume fade sample)
- Scene loader system
- Character controller system
- Avatar system
- Communication system
- Extensions module
- Utilities module
- Transitions module

---

## v1.0.0

Initial release
