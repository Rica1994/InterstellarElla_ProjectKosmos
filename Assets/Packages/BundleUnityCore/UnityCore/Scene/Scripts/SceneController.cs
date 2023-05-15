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
        public class SceneController : MonoBehaviourSingleton<SceneController>
        {
            public delegate void SceneLoadDelegate(SceneType scene);

            private PageController _pageController
            {
                get
                {
                    var instance = PageController.Instance;
                    if (instance == null)
                        Debug.Log("You are trying to access the Pagecontroller, but no instance was found.");
                    return instance;
                }
            }

            private SceneType m_TargetScene;
            private PageType m_LoadingPage;
            private SceneLoadDelegate m_SceneLoadDelegate;
            private bool m_SceneIsLoading;

            private string CurrentSceneName
            {
                get { return SceneManager.GetActiveScene().name; }
            }

            #region Unity Functions

            protected override void Awake()
            {
                base.Awake();
                Configure();
            }
            private void OnDisable()
            {
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

                m_SceneIsLoading = true;
                m_TargetScene = scene;
                m_SceneLoadDelegate = sceneLoadDelegate;
                m_LoadingPage = loadingPage;

                StartCoroutine(LoadScene());
            }

            #endregion


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
            private async void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
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
                AudioController.Instance.VerifyAudioTracks();

                // remove pages that are null
                PageController.Instance.VerifyPages();

                if (m_LoadingPage != PageType.None)
                {
                    await Task.Delay(1000);
                    _pageController.TurnPageOff(m_LoadingPage);
                }

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
                    case SceneType.MainMenu: return "MainMenu";
                    case SceneType.Level_01: return "Level_01";
                    case SceneType.Level_02: return "Level_02";
                    default:
                        Debug.Log("Scene [" + scene + "] does not contain a string for a valid scene. ");
                        return string.Empty;
                }
            }
            private SceneType StringToSceneType(string scene)
            {
                switch (scene)
                {
                    case "MainMenu": return SceneType.MainMenu;
                    case "Level_01": return SceneType.Level_01;
                    case "Level_02": return SceneType.Level_02;
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