﻿using System.Collections;
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
            protected virtual void Start()
            {
                _audioController = ServiceLocator.Instance.GetService<AudioController>();
                _pageController = ServiceLocator.Instance.GetService<PageController>();
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

            public void LoadIntermissionLoading(SceneType scene, SceneLoadDelegate sceneLoadDelegate = null, bool reload = false,
                PageType loadingPage = PageType.None, float timeDelayToStartFirstLoad = 1)
            {
                StartCoroutine(LoadLoadingIntoTarget(timeDelayToStartFirstLoad, scene, null, false, PageType.Loading));
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
                    case SceneType.S_MainMenu: return "S_MainMenu";
                    case SceneType.S_Loading: return "S_Loading";
                    case SceneType.S_Level_1_Blockout: return "S_Level_1_Blockout";
                    case SceneType.S_Level_1_Art: return "S_Level_1_Art";
                    case SceneType.S_Level_1_Final: return "S_Level_1_Final";
                    case SceneType.S_Level_2_Blockout: return "S_Level_2_Blockout";
                    case SceneType.S_Level_2_Art: return "S_Level_2_Art";
                    case SceneType.S_Level_2_Final: return "S_Level_2_Final";
                    case SceneType.S_Level_3_Blockout: return "S_Level_3_Blockout";
                    case SceneType.S_Level_3_Art: return "S_Level_3_Art";
                    case SceneType.S_Level_3_Final: return "S_Level_3_Final";
                    case SceneType.S_Level_4_Blockout: return "S_Level_4_Blockout";
                    case SceneType.S_Level_4_Art: return "S_Level_4_Art";
                    case SceneType.S_Level_4_Final: return "S_Level_4_Final";
                    case SceneType.S_Level_5_Blockout: return "S_Level_5_Blockout";
                    case SceneType.S_Level_5_Art: return "S_Level_5_Art";
                    case SceneType.S_Level_5_Final: return "S_Level_5_Final";
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
                    case "S_Level_1_Blockout": return SceneType.S_Level_1_Blockout;
                    case "S_Level_1_Art": return SceneType.S_Level_1_Art;
                    case "S_Level_1_Final": return SceneType.S_Level_1_Final;
                    case "S_Level_2_Blockout": return SceneType.S_Level_2_Blockout;
                    case "S_Level_2_Art": return SceneType.S_Level_2_Art;
                    case "S_Level_2_Final": return SceneType.S_Level_2_Final;
                    case "S_Level_3_Blockout": return SceneType.S_Level_3_Blockout;
                    case "S_Level_3_Art": return SceneType.S_Level_3_Art;
                    case "S_Level_3_Final": return SceneType.S_Level_3_Final;
                    case "S_Level_4_Blockout": return SceneType.S_Level_4_Blockout;
                    case "S_Level_4_Art": return SceneType.S_Level_4_Art;
                    case "S_Level_4_Final": return SceneType.S_Level_4_Final;
                    case "S_Level_5_Blockout": return SceneType.S_Level_5_Blockout;
                    case "S_Level_5_Art": return SceneType.S_Level_5_Art;
                    case "S_Level_5_Final": return SceneType.S_Level_5_Final;

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