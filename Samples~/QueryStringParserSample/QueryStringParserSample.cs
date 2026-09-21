using Virtuademy.SDK.Core.Authentication;
using Virtuademy.SDK.Core.SystemFramework;
using Virtuademy.SDK.Http;
using Virtuademy.SDK.RealtimeApi;
using Virtuademy.SDK.ApiData;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;


using SPACS.Utilities;

namespace Virtuademy.SDK.Core.ApplicationManagement.Samples
{
    /// <summary>
    /// Parameters arriving in the query string: 
    /// - sessionHash: authentication session hash. Used to authenticate and receive API tokens.
    /// - worldId: used to track experience analytics.
    /// - experienceId: used to create a session and bind analytics to that session.
    /// </summary>
    public class QueryStringParserSample : MonoBehaviour
    {
        [SerializeField] private UrlParametersParserBase urlParametersParser;

        public UnityEvent<WorldDTO> OnWorldRetrieved;
        public UnityEvent<ExperienceDTO> OnExperienceRetrieved;
        public UnityEvent<SessionDTO> OnSessionCreated;
        public UnityEvent<string> OnConnectionCreated;

        private const string WORLD_ID = "worldId";
        private const string SESSION_HASH = "authSessionHash";
        private const string EXPERIENCE_ID = "experienceId";


        private void OnEnable()
        {
            OnSessionCreated.AddListener(CreateWebSocketConnection);
        }

        private void OnDisable()
        {
            OnSessionCreated.RemoveListener(CreateWebSocketConnection);
        }

        public async void ParseQuerystringParameters(Dictionary<string, string> querystring)
        {
            // Parameters validation
            if (!querystring.ContainsKey(WORLD_ID))
            {
                throw new ArgumentNullException($"{nameof(QueryStringParserSample)}: {WORLD_ID} not found in deep link parameters.");
            }

            if (!querystring.ContainsKey(SESSION_HASH))
            {
                throw new ArgumentNullException($"{nameof(QueryStringParserSample)}: {SESSION_HASH} not found in deep link parameters.");
            }

            if (!querystring.ContainsKey(EXPERIENCE_ID))
            {
                throw new ArgumentNullException($"{nameof(QueryStringParserSample)}: {EXPERIENCE_ID} not found in deep link parameters.");
            }

            // Reload a user session with the provided session hash
            string sessionHash = querystring[SESSION_HASH];
            Debug.Log($"{nameof(QueryStringParserSample)}: reloading session with hash {sessionHash}");
            await SM.GetSystem<IAuthenticationSystem>().ReloadSession(sessionHash);


            // Retrieve user data
            UserDTO user = await RetrieveUserData();

            // Retrieve world data
            int worldId = int.Parse(querystring[WORLD_ID]);
            await RetrieveWorldData(worldId);

            // Retrieve experience data
            int experienceId = int.Parse(querystring[EXPERIENCE_ID]);
            await CreateSessionFromExperience(worldId, experienceId, user.Id);
        }

        public async Task<UserDTO> RetrieveUserData()
        {
            ApiResponse<UserDTO> userReq = await PlatformClient.Current.GetMyUserData();
            if (userReq.IsSuccess)
            {
                Debug.Log($"{nameof(QueryStringParserSample)}: Successfully retrieved user data - {userReq.Content.Id}");
                return userReq.Content;
            }
            else
            {
                throw new Exception($"{nameof(QueryStringParserSample)}: Unable to retrieve user data - {userReq.ReasonPhrase}");
            }
        }

        public async Task<WorldDTO> RetrieveWorldData(int worldId)
        {
            ApiResponse<WorldDTO> worldReq = await PlatformClient.Current.GetWorld(worldId);
            if (worldReq.IsSuccess)
            {
                WorldDTO world = worldReq.Content;
                OnWorldRetrieved?.Invoke(world);
                Debug.Log($"{nameof(QueryStringParserSample)}: Successfully retrieved world {worldId} - {world.Label}");
                return world;
            }
            else
            {
                throw new Exception($"{nameof(QueryStringParserSample)}: Unable to retrieve world {worldId} - {worldReq.ReasonPhrase}");
            }
        }

        public async Task<SessionDTO> CreateSessionFromExperience(int worldId, int experienceId, int userId)
        {
            ApiResponse<ExperienceDTO> experienceReq = await PlatformClient.Current.GetExperience(worldId, experienceId);
            if (experienceReq.IsSuccess)
            {
                ExperienceDTO experience = experienceReq.Content;
                OnExperienceRetrieved?.Invoke(experience);
                Debug.Log($"{nameof(QueryStringParserSample)}: Successfully retrieved session data: {experienceId} - {experience.Label}");

                // Create a new single player session for the experience
                NewSessionDTO newSession = new()
                {
                    Label = $"{experience.Label} - {userId} - External experience",
                    StartDate = null,
                    EndDate = null,
                    Multiplayer = false,
                    TagIds = Array.Empty<int>(),
                    Accessibility = ESessionAccessibility.Closed,
                    UserIds = Array.Empty<int>(),
                };

                ApiResponse<SessionDTO> newSessionReq = await PlatformClient.Current.CreateSession(worldId, experienceId, newSession);
                if (newSessionReq.IsSuccess)
                {
                    SessionDTO createdSession = newSessionReq.Content;
                    Debug.Log($"{nameof(QueryStringParserSample)}: Successfully created session: {createdSession.Id} - {createdSession.Label}");
                    OnSessionCreated.Invoke(createdSession);
                    return createdSession;
                }
                else
                {
                    throw new Exception($"DeepLinkParserSample: Unable to create session: {experienceId} - {newSessionReq.ReasonPhrase}");
                }
            }
            else
            {
                throw new Exception($"DeepLinkParserSample: Unable to retrieve session data: {experienceId} - {experienceReq.ReasonPhrase}");
            }
        }

        private void CreateWebSocketConnection(SessionDTO session)
        {
            // Connect to the realtime api to ping user presence in the created session
            RealtimeApiClient realtimeApiSystem = RealtimeApiClient.Current;
            realtimeApiSystem.ConnectToRealtime(
                (handshake) =>
                {
                    Debug.Log($"{nameof(QueryStringParserSample)}: successfully created websocket connection, client id: {handshake.ConnectionId}");
                    realtimeApiSystem.JoinWorld(session.WorldId, session.Id, (value) =>
                    {
                        Debug.Log($"{nameof(QueryStringParserSample)}: successfully joined world {session.WorldId} with session {session.Id}");
                        OnConnectionCreated?.Invoke(handshake.ConnectionId);
                    });
                },
                (reason) =>
                {
                    Debug.LogError($"{nameof(QueryStringParserSample)}: disconnected from websocket, reason : {reason}");
                },
                (kick) =>
                {
                    Debug.Log($"{nameof(QueryStringParserSample)}: kicked from websocket, {kick}");
                },
                () =>
                {
                    Debug.Log($"{nameof(QueryStringParserSample)}: Double session login");
                });
        }
    }
}

