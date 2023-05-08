

using UnityEngine;


namespace UnityCore
{
    namespace Menus
    {
        public class TestMenu : MonoBehaviour
        {
            public PageController PageControllerScript;


            private void Update()
            {
                if (Input.GetKeyUp(KeyCode.F))
                {
                    PageControllerScript.TurnPageOn(PageType.Loading);
                }

                if (Input.GetKeyUp(KeyCode.G))
                {
                    PageControllerScript.TurnPageOff(PageType.Loading);
                }

                if (Input.GetKeyUp(KeyCode.H))
                {
                    PageControllerScript.TurnPageOff(PageType.Loading, PageType.Pause);
                }

                if (Input.GetKeyUp(KeyCode.J))
                {
                    PageControllerScript.TurnPageOff(PageType.Loading, PageType.Pause, true);
                }
            }
        }
    }
}


