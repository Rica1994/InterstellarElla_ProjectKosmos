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

            private void Start()
            {
                if (_myPage != null)
                {
                    PageController.Instance.RegisterOutsiderPage(_myPage);
                }
                else
                {
                    Debug.LogWarning("You forgot to assign my page on object -> " + this.gameObject);
                }            
            }
        }
    } 
}
