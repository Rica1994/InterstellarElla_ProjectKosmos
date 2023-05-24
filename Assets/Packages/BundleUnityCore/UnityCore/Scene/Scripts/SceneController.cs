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

                Debug.Log("1) Scene can be loaded ");

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

                Debug.Log("1) Scene can be loaded ");

                m_SceneIsLoading = true;
                m_TargetScene = TargetSceneAfterLoading;
                m_SceneLoadDelegate = sceneLoadDelegate;
                m_LoadingPage = loadingPage;

                StartCoroutine(LoadScene());
            }

            public void LoadIntermissionLoading(SceneType scene, SceneLoadDelegate sceneLoadDelegate = null, bool reload = false,
                PageType loadingPage = PageType.None, float timeDelayToStartFirstLoad = 1)
            {
                StartCoroutine(LoadLoadingIntoTarget(timeDelayToStartFirstLoad, scene, null, false, PageType.Loading));
            }
            public void LoadLoadingscene()
            {
                //Load(SceneType.S_Loading, null, false, PageType.Loading);

                // below logic works just fine 
                //m_TargetScene = SceneType.S_Loading;
                m_TargetScene = SceneType.S_Level_2_Build;
                string targetSceneName = SceneTypeToString(m_TargetScene);
                SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
            }



            #endregion


            #region Private Functions

            private IEnumerator LoadScene()
            {        
                if (m_LoadingPage != PageType.None)
                {
                    Debug.Log("2) Starting coroutine fade to white");

                    _pageController.TurnPageOn(m_LoadingPage);

                    yield return new WaitUntil(() => _pageController.PageIsOn(m_LoadingPage));

                    Debug.Log("3) finished fade to white");
                }

                Debug.Log(" 4) LOADING AFTER FADE");
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
            private async void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
            {
                Debug.Log("A) OnSceneLoaded happens");
                if (m_TargetScene == SceneType.None)
                {
                    return;
                }

                Debug.Log("B) OnSceneLoaded continues");
                SceneType sceneType = StringToSceneType(scene.name);
                if (m_TargetScene != sceneType)
                {
                    return;
                }

                Debug.Log("C) OnSceneLoaded continues more");
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

                Debug.Log("D) OnSceneLoaded continues FURTHER");

                // remove audio tracks where source is null
                _audioController.VerifyAudioTracks();

                // remove pages that are null
                _pageController.VerifyPages();

                if (m_LoadingPage != PageType.None)
                {
                    Debug.Log("E) turning off page loading");

                    //await Task.Delay(1000); THIS does not work in WebGL :'^{

                    Debug.Log("EE) waited for delay");
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

                Debug.Log("F) resetting sceneLoading bool");
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
                switch (scene)
                {
                    case SceneType.S_MainMenu: return "S_MainMenu";
                    case SceneType.S_Loading: return "S_Loading";
                    case SceneType.S_Level_1_Work: return "S_Level_1_Work";
                    case SceneType.S_Level_1_Build: return "S_Level_1_Build";
                    case SceneType.S_Level_2_Work: return "S_Level_2_Work";
                    case SceneType.S_Level_2_Build: return "S_Level_2_Build";
                    case SceneType.S_Level_3_Work: return "S_Level_3_Work";
                    case SceneType.S_Level_3_Build: return "S_Level_3_Build";
                    case SceneType.S_Level_4_Work: return "S_Level_4_Work";
                    case SceneType.S_Level_4_Build: return "S_Level_4_Build";
                    case SceneType.S_Level_5_Work: return "S_Level_5_Work";
                    case SceneType.S_Level_5_Build: return "S_Level_5_Build";
                    default:
                        Debug.Log("Scene [" + scene + "] does not contain a string for a valid scene. ");
                        return string.Empty;
                }
            }
            private SceneType StringToSceneType(string scene)
            {
                switch (scene)
                {
                    case "S_MainMenu": return SceneType.S_MainMenu;
                    case "S_Loading": return SceneType.S_Loading;
                    case "S_Level_1_Work": return SceneType.S_Level_1_Work;
                    case "S_Level_1_Build": return SceneType.S_Level_1_Build;
                    case "S_Level_2_Work": return SceneType.S_Level_2_Work;
                    case "S_Level_2_Build": return SceneType.S_Level_2_Build;
                    case "S_Level_3_Work": return SceneType.S_Level_3_Work;
                    case "S_Level_3_Build": return SceneType.S_Level_3_Build;
                    case "S_Level_4_Work": return SceneType.S_Level_4_Work;
                    case "S_Level_4_Build": return SceneType.S_Level_4_Build;
                    case "S_Level_5_Work": return SceneType.S_Level_5_Work;
                    case "S_Level_5_Build": return SceneType.S_Level_5_Build;

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


            #endregion
        }
    }
}