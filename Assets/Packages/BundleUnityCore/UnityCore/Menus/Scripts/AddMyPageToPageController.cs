using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityCore
{
    namespace Menus
    {
        public class AddMyPageToPageController : MonoBehaviour
        {
            [SerializeField]
            private Page _myPage;

            PageController _pageController;

            private void Start()
            {
                _pageController = ServiceLocator.Instance.GetService<PageController>();

                if (_myPage != null)
                {
                    _pageController.RegisterOutsiderPage(_myPage);
                }
                else
                {
                    Debug.LogWarning("You forgot to assign my page on object -> " + this.gameObject);
                }

                this.gameObject.SetActive(false);
            }
        }
    } 
}
