using SPACS.Utilities;

using Newtonsoft.Json;

using Virtuademy.SDK.Authentication;
using Virtuademy.SDK.Authentication.Samples;
using Virtuademy.SDK.Core.ApplicationManagement.Samples;
using Virtuademy.SDK.Core.SystemFramework;
using Virtuademy.SDK.Http;
using Virtuademy.SDK.ApiData;

using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;


public class QueryStringSceneManager : MonoBehaviour
{
    [SerializeField] private int worldId;
    [SerializeField] private int experienceId;

    [SerializeField] private QueryStringParserSample queryStringParserSample;
    [SerializeField] private UrlParametersParserBase urlParametersParser;
    [SerializeField] private LoginPanelBinding loginPanelBinding;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI profile;
    [SerializeField] private TextMeshProUGUI worldData;
    [SerializeField] private TextMeshProUGUI experienceData;
    [SerializeField] private TextMeshProUGUI sessionData;
    [SerializeField] private TextMeshProUGUI websocket;

    [Header("Sample analytics")]
    [SerializeField] private string encryptionPassword = "SamplePassword";
    [SerializeField] private string experienceKey = "sample";

    [SerializeField] private ExperienceJoinDTO sampleExperienceJoin;
    [SerializeField] private ExperienceStartDTO sampleExperienceStart;
    [SerializeField] private ExperienceCompleteDTO sampleExperienceComplete;
    [SerializeField] private ExperienceStepStartDTO sampleExperienceStepStart;
    [SerializeField] private ExperienceStepCompleteDTO sampleExperienceStepComplete;
    [SerializeField] private ExperienceTranscriptDTO sampleExperienceTranscript;


    private int sessionId = -1;
    private string experienceUniqueId;

    public int VerbIntValue { get; set; }
    private EAnalyticVerb Verb => (EAnalyticVerb)(VerbIntValue + 1);

    private void Awake()
    {
        JsonConverters.SetJsonConvertDefaultSettings();

        SM.OnAllSystemsSetupsDone.AddListener(OnAllSystemsSetupsDone);
    }

    private async void OnAllSystemsSetupsDone()
    {
        Debug.Log("All systems setups done. App is ready.");

        loginPanelBinding.Setup();

        // register to events fired by the QueryStringParserSample
        queryStringParserSample.OnWorldRetrieved.AddListener((world) =>
        {
            worldData.text += JsonConvert.SerializeObject(world);
        });

        queryStringParserSample.OnExperienceRetrieved.AddListener((experience) =>
        {
            experienceData.text += JsonConvert.SerializeObject(experience);
        });

        queryStringParserSample.OnSessionCreated.AddListener((session) =>
        {
            sessionData.text += JsonConvert.SerializeObject(session);
            sessionId = session.Id;
            experienceUniqueId = GenerateUniqueKey(experienceKey);
        });

        queryStringParserSample.OnConnectionCreated.AddListener((handshake) =>
        {
            websocket.text += JsonConvert.SerializeObject(handshake);
        });

        AuthenticationSystem authSystem = SM.GetSystem<AuthenticationSystem>();
        authSystem.OnAuthenticated.AddListener(async () =>
        {
            ApiResponse<JObject> prefs = await SM.GetSystem<AuthenticationSystem>().GetMyPreferences(false);
            profile.text += JsonConvert.SerializeObject(prefs.Content);
            loginPanelBinding.gameObject.SetActive(false);
        });

        // Parse the URL parameters and start the flow
        Dictionary<string, string> parameters = urlParametersParser.ParseUrlParameters();
        if (parameters.Count > 0)
        {
            queryStringParserSample.ParseQuerystringParameters(parameters);
        }
        else
        {
            authSystem.OnAuthenticated.AddListener(async () =>
            {
                UserDTO user = await queryStringParserSample.RetrieveUserData();
                await queryStringParserSample.RetrieveWorldData(worldId);
                await queryStringParserSample.CreateSessionFromExperience(worldId, experienceId, user.Id);
            });
            await authSystem.LoadSession();
        }
    }

    /// <summary>
    /// Test method to send an analytic to Virtuademy API.
    /// </summary>
    public async void SendAnalytic()
    {
        ExperienceAnalyticDTO experienceAnalyticDTO = null;

        switch (Verb)
        {
            case EAnalyticVerb.ExpJoin:
                experienceAnalyticDTO = sampleExperienceJoin;
                break;

            case
                EAnalyticVerb.ExpStart:
                experienceAnalyticDTO = sampleExperienceStart;
                break;

            case
                EAnalyticVerb.ExpComplete:
                experienceAnalyticDTO = sampleExperienceComplete;
                break;

            case
                EAnalyticVerb.StepStart:
                experienceAnalyticDTO = sampleExperienceStepStart;
                break;

            case
                EAnalyticVerb.StepComplete:
                experienceAnalyticDTO = sampleExperienceStepComplete;
                break;

            case
                EAnalyticVerb.ExpTranscript:
                experienceAnalyticDTO = sampleExperienceTranscript;
                break;

            default:
                Debug.LogError($"Unsupported verb type: {Verb}");
                break;
        }

        experienceAnalyticDTO.SessionId = sessionId;
        experienceAnalyticDTO.uniqueId = experienceUniqueId;
        experienceAnalyticDTO.Statement = null;

        Debug.Log($"Sending analytic: {JsonConvert.SerializeObject(experienceAnalyticDTO)}");

        ApiResponse experienceAnalyticReq = await PlatformClient.Current.CreateExperienceAnalytic(experienceAnalyticDTO);
        if (experienceAnalyticReq.IsSuccess)
        {
            Debug.Log("Experience analytic sent successfully.");
        }
        else
        {
            Debug.LogError($"Error sending experience analytic: {experienceAnalyticReq.StatusCode} {experienceAnalyticReq.ReasonPhrase}");
        }
    }


    private string GenerateUniqueKey(string key)
    {
        return $"{DateTime.UtcNow:yyyyMMddHHmmssfff}{sessionId:00000}{StringExtensions.GenerateRandomAlphanumericString(8)}{key}".EncryptDecriptXOR(encryptionPassword);
    }
}
