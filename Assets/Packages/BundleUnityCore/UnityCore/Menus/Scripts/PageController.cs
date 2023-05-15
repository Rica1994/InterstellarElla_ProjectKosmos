using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityCore
{
    namespace Menus
    {
        public class PageController : Service
        {
            public PageType EntryPage;

            public List<Page> PagesScene;

            private Hashtable m_Pages;

            public IEnumerator ButtonRunner = null;

            #region Unity Functions

            protected override void Awake()
            {
                base.Awake();
                m_Pages = new Hashtable();
                RegisterAllPages();
                TurnAllPagesOffExcept(EntryPage);
            }

            #endregion


            #region Public Functions

            public void TurnPageOn(PageType typeToTurnOn)
            {
                if (typeToTurnOn == PageType.None) return;
                if (PageExists(typeToTurnOn) == false)
                {
                    Debug.Log("You're trying to turn a page on [" + typeToTurnOn + "] that has not been registered");
                    return;
                }

                Page page = GetPage(typeToTurnOn);
                page.gameObject.SetActive(true);
                page.Animate(true);
            }

            public void TurnPageOff(PageType typeToTurnOff, PageType typeToTurnOn = PageType.None,
                bool waitForExit = false)
            {
                //if (typeToTurnOff == PageType.None) return;
                if (PageExists(typeToTurnOff) == false)
                {
                    Debug.Log("You're trying to turn a page off [" + typeToTurnOff + "] that has not been registered");
                    return;
                }

                Page offPage = GetPage(typeToTurnOff);
                if (offPage.gameObject.activeSelf == true)
                {
                    offPage.Animate(false);
                }

                if (typeToTurnOn != PageType.None)
                {
                    Page onPage = GetPage(typeToTurnOn);
                    if (waitForExit == true)
                    {
                        StopCoroutine(WaitForPageExit(onPage, offPage));
                        StartCoroutine(WaitForPageExit(onPage, offPage));
                    }
                    else
                    {
                        TurnPageOn(onPage.Type);
                    }
                }
            }

            public bool PageIsOn(PageType pageType)
            {
                if (PageExists(pageType) == false)
                {
                    //Debug.Log("You are trying to detect if a page is on [" + pageType + "], but it has not been registered");
                    return false;
                }

                var page = GetPage(pageType);
                return page.isOn /*|| page.gameObject.activeSelf*/;
            }

            // custom methods below
            public void TurnAllPagesOffExcept(PageType turnOn)
            {
                for (int i = 0; i < PagesScene.Count; i++)
                {
                    var page = PagesScene[i];
                    if (PageIsOn(page.Type) == true || GetPage(page.Type).gameObject.activeSelf)
                    {
                        TurnPageOff(PagesScene[i].Type);
                    }
                }

                TurnPageOn(turnOn);
            }

            public IEnumerator TurnPageOffDelay(PageType typeToTurnOff, PageType typeToTurnOn = PageType.None,
                bool waitForExit = false, float delayTime = 0.25f)
            {
                yield return new WaitForSeconds(delayTime);

                if (typeToTurnOff == PageType.None)
                {
                    Debug.Log("You're trying to turn Nothing off");
                }

                if (PageExists(typeToTurnOff) == false)
                {
                    Debug.Log("You're trying to turn a page off [" + typeToTurnOff + "] that has not been registered");
                }

                Page offPage = GetPage(typeToTurnOff);
                if (offPage.gameObject.activeSelf == true)
                {
                    offPage.Animate(false);
                }

                if (typeToTurnOn != PageType.None)
                {
                    Page onPage = GetPage(typeToTurnOn);
                    if (waitForExit == true)
                    {
                        StopCoroutine(WaitForPageExit(onPage, offPage));
                        StartCoroutine(WaitForPageExit(onPage, offPage));
                    }
                    else
                    {
                        TurnPageOn(onPage.Type);
                    }
                }
            }

            public void VerifyPages()
            {
                for (int i = 0; i < PagesScene.Count; i++)
                {
                    //Debug.Log("Checking page index -> " + i);
                    Page pageToCheck = PagesScene[i];
                    RemoveNullPage(pageToCheck);
                }
            }

            public void RegisterOutsiderPage(Page pageToRegister)
            {
                if (PageExists(pageToRegister.Type) == false)
                {
                    RegisterPage(pageToRegister);
                }       
            }

            #endregion


            #region Private Functions

            private IEnumerator WaitForPageExit(Page on, Page off)
            {
                while (off.TargetState != PageState.None)
                {
                    yield return null;
                }

                TurnPageOn(on.Type);
            }

            private void RegisterAllPages()
            {
                for (int i = 0; i < PagesScene.Count; i++)
                {
                    RegisterPage(PagesScene[i]);
                }
            }

            private void RegisterPage(Page page)
            {
                if (PageExists(page.Type))
                {
                    Debug.Log("You are trying to register a page [" + page.Type +
                              "] that has already been registered : " + page.gameObject.name);
                    return;
                }

                m_Pages.Add(page.Type, page);
                PagesScene.Add(page);

            }

            private Page GetPage(PageType type)
            {
                if (PageExists(type) == false)
                {
                    Debug.Log("You are trying to get a page [" + type + "] that has not been registered");
                    return null;
                }

                return (Page)m_Pages[type];
            }

            private bool PageExists(PageType type)
            {
                return m_Pages.ContainsKey(type);
            }


            private void RemoveNullPage(Page pageToCheck)
            {
                if (pageToCheck == null)
                {
                    //Debug.Log("Removing page -> " + pageToCheck);

                    PagesScene.Remove(pageToCheck);
                    m_Pages.Remove(pageToCheck.Type);
                }
            }

            #endregion
        }
    }
}