
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityCore.Audio;
using UnityCore.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityCore
{
    namespace Scene
    {
        public class SceneController : Service
        {
            public delegate void SceneLoadDelegate(SceneType scene);

            private SceneType m_TargetScene;
            private PageType m_LoadingPage;
            private SceneLoadDelegate m_SceneLoadDelegate;
            private bool m_SceneIsLoading;

            private AudioController _audioController;
            private PageController _pageController;

            public SceneType TargetSceneAfterLoading;

            private string CurrentSceneName
            {
                get { return SceneManager.GetActiveScene().name; }
            }

            #region Unity Functions


            protected override void OnEnable()
            {
                base.OnEnable();
                Configure();
            }
            protected virtual void Start()
            {
                _audioController = ServiceLocator.Instance.GetService<AudioController>();
                _pageController = ServiceLocator.Instance.GetService<PageController>();
            }
            protected override void OnDisable()
            {
                base.OnDisable();
                Dispose();
            }

            #endregion


            #region Public Functions

            public void Load(SceneType sceneType)
            {
                LoadIntermissionLoading(sceneType, false, null, false, PageType.None, 1);
            }

            public void Load(SceneType scene, SceneLoadDelegate sceneLoadDelegate = null, bool reload = false,
                PageType loadingPage = PageType.None)
            {
                if (loadingPage != PageType.None && _pageController == null)
                {
                    return;
                }

                if (SceneCanBeLoaded(scene, reload) == false)
                {
                    return;
                }

                m_SceneIsLoading = true;
                m_TargetScene = scene;
                m_SceneLoadDelegate = sceneLoadDelegate;
                m_LoadingPage = loadingPage;

                StartCoroutine(LoadScene());
            }
            public void LoadTargetAfterLoadingScene(SceneLoadDelegate sceneLoadDelegate = null, bool reload = false,
                PageType loadingPage = PageType.None)
            {
                if (loadingPage != PageType.None && _pageController == null)
                {
                    return;
                }

                if (SceneCanBeLoaded(TargetSceneAfterLoading, reload) == false)
                {
                    return;
                }

                m_SceneIsLoading = true;
                m_TargetScene = TargetSceneAfterLoading;
                m_SceneLoadDelegate = sceneLoadDelegate;
                m_LoadingPage = loadingPage;

                StartCoroutine(LoadScene());
            }

            public void LoadIntermissionLoading(SceneType scene, bool isSameBuild = false, SceneLoadDelegate sceneLoadDelegate = null, bool reload = false,
                PageType loadingPage = PageType.None, float timeDelayToStartFirstLoad = 1)
            {
                // we still need to add a delay on the webgl loading (so we still have our fancy main menu effect)

                // in the second option, set a bool on the levelManager "IsSameBuildNextScene"..
                // => if true, do the usual loading, else the webgl loading
                if (isSameBuild == true)
                {
                    StartCoroutine(LoadLoadingIntoTarget(timeDelayToStartFirstLoad, scene, null, false, PageType.Loading));
                    return;
                }
#if UNITY_EDITOR
                StartCoroutine(LoadLoadingIntoTarget(timeDelayToStartFirstLoad, scene, null, false, PageType.Loading));
#elif UNITY_WEBGL && !UNITY_EDITOR
                string currentSceneName = SceneManager.GetActiveScene().name;
                string relativePath = GetRelativePath(currentSceneName, scene);

                string data = $"{UnityEngine.Networking.UnityWebRequest.EscapeURL(GameManager.Data.ToString())}";

                Application.ExternalEval($"window.location.href = '{relativePath + "/index.html" + "?data=" + data}';");
#endif
            }


            #endregion

            private IEnumerator WebGLLoadingDelay(SceneType scene, SceneLoadDelegate sceneLoadDelegate, bool reload,
    PageType loadingPage, float timeDelay)
            {
                // Pause for the specified timeDelay
                yield return new WaitForSeconds(timeDelay);

                
            }

            #region Private Functions

            private IEnumerator LoadScene()
            {
                if (m_LoadingPage != PageType.None)
                {
                    _pageController.TurnPageOn(m_LoadingPage);

                    yield return new WaitUntil(() => _pageController.PageIsOn(m_LoadingPage));
                }

                string targetSceneName = SceneTypeToString(m_TargetScene);
                SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
            }
            private IEnumerator LoadLoadingIntoTarget(float timeDelay, SceneType sceneToLoad,
                                                      SceneLoadDelegate sceneLoadDelegate = null, bool reload = false,
                                                      PageType loadingPage = PageType.None)
            {
                yield return new WaitForSeconds(timeDelay);

                Load(SceneType.S_Loading, null, false, PageType.Loading);

                yield return new WaitForSeconds(3);

                Load(sceneToLoad, null, false, PageType.Loading);
            }
            private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
            {
                if (m_TargetScene == SceneType.None)
                {
                    return;
                }

                SceneType sceneType = StringToSceneType(scene.name);
                if (m_TargetScene != sceneType)
                {
                    return;
                }

                if (m_SceneLoadDelegate != null)
                {
                    try
                    {
                        m_SceneLoadDelegate(sceneType);
                    }
                    catch (System.Exception)
                    {
                        Debug.Log("Unable to respond with sceneLoadDelegate after scene [" + sceneType + "] loaded.");
                    }
                }

                // remove audio tracks where source is null
                _audioController.VerifyAudioTracks();

                // remove pages that are null
                _pageController.VerifyPages();

                if (m_LoadingPage != PageType.None)
                {
                    //await Task.Delay(1000); THIS does not work in WebGL :'^{

                    _pageController.TurnPageOff(m_LoadingPage);
                }

                // showcase the UI_persistent(pause_button) when needed
                if (m_TargetScene == SceneType.S_MainMenu || m_TargetScene == SceneType.S_Loading)
                {
                    _pageController.ShowPauseButton(false);
                    ServiceLocator.Instance.GetService<GameManager>().SetPlayerController(null);
                }
                else
                {
                    _pageController.ShowPauseButton(true);

                    var playerController = FindObjectOfType<PlayerController>();
                    ServiceLocator.Instance.GetService<GameManager>().SetPlayerController(playerController);
                }

                Debug.Log("turned off sceneLoading bool");

                m_SceneIsLoading = false;
            }
            private bool SceneCanBeLoaded(SceneType scene, bool reload)
            {
                string targetSceneName = SceneTypeToString(scene);

                if (CurrentSceneName == targetSceneName && reload == false)
                {
                    Debug.Log("You are tring to load a scene [" + scene + "] which is already active.");
                    return false;
                }
                else if (targetSceneName == string.Empty)
                {
                    Debug.Log("The scene you are trying to load [" + scene + "] is not valid.");
                    return false;
                }
                else if (m_SceneIsLoading == true)
                {
                    Debug.Log("Unable to load scene [" + scene + "]. Another scene [" + m_TargetScene +
                              "] is already loading.");
                    return false;
                }

                return true;
            }


            // add extra scene names made for the game to this list of strings ?? 
            private string SceneTypeToString(SceneType scene)
            {
                return scene.ToString();
            }
            private SceneType StringToSceneType(string scene)
            {
                switch (scene)
                {
                    case "S_GameStartUpScene": return SceneType.S_GameStartUpScene;

                    case "S_MainMenu": return SceneType.S_MainMenu;
                    case "S_Loading": return SceneType.S_Loading;

                    case "S_Level_1_Intro": return SceneType.S_Level_1_Intro;
                    case "S_Level_1_0_Work": return SceneType.S_Level_1_0_Work;
                    case "S_Level_1_1_Work": return SceneType.S_Level_1_1_Work;
                    case "S_Level_1_Outro": return SceneType.S_Level_1_Outro;

                    case "S_Level_2_Intro": return SceneType.S_Level_2_Intro;
                    case "S_Level_2_3_Work": return SceneType.S_Level_2_3_Work;
                    case "S_Level_2_4_Work": return SceneType.S_Level_2_4_Work;
                    case "S_Level_2_Outro": return SceneType.S_Level_2_Outro;

                    case "S_Level_3_Intro": return SceneType.S_Level_3_Intro;
                    case "S_Level_3_0_Work": return SceneType.S_Level_3_0_Work;
                    case "S_Level_3_1_Work": return SceneType.S_Level_3_1_Work;
                    case "S_Level_3_2_Work": return SceneType.S_Level_3_2_Work;
                    case "S_Level_3_3_Work": return SceneType.S_Level_3_3_Work;
                    case "S_Level_3_4_Work": return SceneType.S_Level_3_4_Work;
                    case "S_Level_3_5_Work": return SceneType.S_Level_3_5_Work;
                    case "S_Level_3_Outro": return SceneType.S_Level_3_Outro;

                    case "S_Level_4_Intro": return SceneType.S_Level_4_Intro;
                    case "S_Level_4_0_Work": return SceneType.S_Level_4_0_Work;
                    case "S_Level_4_1_Work": return SceneType.S_Level_4_1_Work;
                    case "S_Level_4_2_Work": return SceneType.S_Level_4_2_Work;
                    case "S_Level_4_3_Work": return SceneType.S_Level_4_3_Work;
                    case "S_Level_4_4_Work": return SceneType.S_Level_4_4_Work;
                    case "S_Level_4_5_Work": return SceneType.S_Level_4_5_Work;
                    case "S_Level_4_Outro": return SceneType.S_Level_4_Outro;

                    case "S_Level_5_Intro": return SceneType.S_Level_5_Intro;
                    case "S_Level_5_0_Work": return SceneType.S_Level_5_0_Work;
                    case "S_Level_5_1_Work": return SceneType.S_Level_5_1_Work;
                    case "S_Level_5_2_Work": return SceneType.S_Level_5_2_Work;
                    case "S_Level_5_3_Work": return SceneType.S_Level_5_3_Work;
                    case "S_Level_5_Outro": return SceneType.S_Level_5_Outro;

                    case "S_Quiz": return SceneType.S_Quiz;

                    case "S_Level_1_0": return SceneType.S_Level_1_0;
                    case "S_Level_1_1": return SceneType.S_Level_1_1;
                    case "S_Level_2_0": return SceneType.S_Level_2_0;
                    case "S_Level_2_1": return SceneType.S_Level_2_1;
                    case "S_Level_2_2": return SceneType.S_Level_2_2;
                    case "S_Mars_1_99_OutroCutscene": return SceneType.S_Mars_1_99_OutroCutscene;

                    default:
                        Debug.Log("Scene [" + scene + "] does not contain a type for a valid scene. ");
                        return SceneType.None;
                }
            }


            private void Configure()
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            private void Dispose()
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }


            private int GetLevelsToGoUp(string sceneName)
            {
                if (sceneName.Contains("Level"))
                    return 2; // For scenes like S_Level_1_0_Introscene, go two levels up
                else if (sceneName.Contains("Quiz"))
                    return 1;
                else
                    return 0; // Default case, stay in the current directory
            }

            public string GetPlanetNameFromEnum(SceneType sceneType)
            {
                var sceneTypeString = sceneType.ToString();
                return GetPlanetNameFromString(sceneTypeString);
            }

            public string GetPlanetNameFromString(string sceneName)
            {
                if (sceneName.Contains("Level_1") || sceneName.Contains("Mars"))
                    return "Mars";
                else if (sceneName.Contains("Level_2"))
                    return "Pluto";
                else if (sceneName.Contains("Level_3"))
                    return "Venus";
                else if (sceneName.Contains("Level_4"))
                    return "Saturn";
                else if (sceneName.Contains("Level_5"))
                    return "Mercury";
                else return "";
            }

            private string GetLevelIndexFromTargetScene(string targetScene)
            {
                if (targetScene.Contains("Level_1"))
                    return "1";
                else if (targetScene.Contains("Level_2"))
                    return "2";
                else if (targetScene.Contains("Level_3"))
                    return "3";
                else if (targetScene.Contains("Level_4"))
                    return "4";
                else if (targetScene.Contains("Level_5"))
                    return "5";
                else return "0";
            }

            private string GetRelativePath(string currentSceneName, SceneType targetScene)
            {
                string targetPlanetName = GetPlanetNameFromEnum(targetScene);
                string currentPlanetName = GetPlanetNameFromString(currentSceneName);

                if (currentSceneName.Contains("MainMenu"))
                {
                    return $"./Levels/{targetPlanetName}/{targetScene.ToString()}";
                }

                if (currentSceneName.Contains("Quiz"))
                {
                    // From quiz we always go to Main Menu
                    return $"../../";
                }


                // If we are leaving from a planet scene

                // If our target scene is a planet
                if (targetPlanetName.Length != 0)
                {
                    // if our target planet is not the same planet as the current planet
                    if (targetPlanetName != currentPlanetName)
                    {
                        return $"../../{targetPlanetName}/{targetScene.ToString()}";
                    }
                    else return $"../{targetScene.ToString()}";
                }

                switch (targetScene)
                {
                    case SceneType.S_MainMenu:
                        return "../../../";
                    case SceneType.S_Quiz:
                        return "../../Quiz";
                }

                Debug.LogError($"{currentSceneName} to load {targetScene.ToString()} does not work.");
                return "";
            }

            #endregion
        }
    }
}