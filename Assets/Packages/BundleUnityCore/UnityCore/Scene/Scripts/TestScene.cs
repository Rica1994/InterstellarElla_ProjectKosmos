
using UnityCore.Menus;
using UnityEngine;


namespace UnityCore
{
    namespace Scene
    {
        public class TestScene : MonoBehaviour
        {
            public SceneController SceneControllerScript;


            private void Awake()
            {
                if (SceneControllerScript == null)
                {
                    SceneControllerScript = FindObjectOfType<SceneController>();
                }
            }


            private void Update()
            {
                if (Input.GetKeyUp(KeyCode.M))
                {
                    //SceneControllerScript.Load(SceneType.Level_0, (_scene) =>
                    //{
                    //   Debug.Log("Scene [" + _scene + "] loaded from test script");
                    //}, 
                    //false, 
                    //PageType.Loading);
                    SceneControllerScript.Load(SceneType.Level_01, null, false, PageType.Loading);
                }

                if (Input.GetKeyUp(KeyCode.G))
                {
                    SceneControllerScript.Load(SceneType.Level_02, null, false, PageType.Loading);
                }
            }
        }
    }
}



