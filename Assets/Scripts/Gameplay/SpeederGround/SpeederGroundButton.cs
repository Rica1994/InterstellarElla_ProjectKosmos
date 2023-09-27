using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeederGroundButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void OnPointerDownHandler();
    public event OnPointerDownHandler Pressed;

     public void OnPressedDown()
    {
        Pressed?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    //    Debug.Log("OnPointerDown called");
    //
    //    _currentFingerId = eventData.pointerId;
    //
    //    _pressingButton = true;
    //    _holdingIndication.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
     //   Debug.Log("OnPointerUp called");
     //
     //   if (_currentFingerId == eventData.pointerId)
     //   {
     //       _pressingButton = false;
     //       _holdingIndication.gameObject.SetActive(false);
     //   }
    }
}
