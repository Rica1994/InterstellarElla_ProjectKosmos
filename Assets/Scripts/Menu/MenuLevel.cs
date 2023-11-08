using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevel : MonoBehaviour
{
    [Header("My Planet is")]
    [SerializeField]
    private GameManager.Planet _myPlanetType;
    public GameManager.Planet MyPlanetType => _myPlanetType;


    [Header("Animation components children")]
    public Animation AnimationScaler;
    public Animation AnimationRotater;

    [Header("Animation components UI")]
    [SerializeField] private Animation _animationPlanet;

    [Header("Text components score")]
    [SerializeField]
    private Text _textPlanet;

    [SerializeField]
    private FactSheet.FactSheetData _factSheetData;
    public FactSheet.FactSheetData FactSheetData => _factSheetData;

    private const string _animationPlanetPop = "A_MenuLevel_Text_Planet_Popup";
    private const string _animationPlanetPoof = "A_MenuLevel_Text_Planet_Poof";

    private const string _animationScorePop = "A_MenuLevel_Text_Score_Popup";
    private const string _animationScorePoof = "A_MenuLevel_Text_Score_Poof";

    private const string _animationValuePop = "A_MenuLevel_Text_Value_Popup";
    private const string _animationValuePoof = "A_MenuLevel_Text_Value_Poof";

    private void Start()
    {
        _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.MercuryCompletion;
        switch (_myPlanetType)
        {
            case GameManager.Planet.Venus:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.VenusCompletion;
                break;
            case GameManager.Planet.Mars:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.MarsCompletion;
                break;
            case GameManager.Planet.Saturn:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.SaturnCompletion;
                break;
            case GameManager.Planet.Pluto:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.PlutoCompletion;
                break;
            case GameManager.Planet.Mercury:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.MercuryCompletion;
                break;
        }
    }

    public void PopupAnimationsText()
    {
        ServiceLocator.Instance.GetService<MainMenuManager>().StartCoroutine(PopupAnimationRoutine());     
    }
    public void PoofAnimationsText()
    {
        ServiceLocator.Instance.GetService<MainMenuManager>().StartCoroutine(PoofAnimationRoutine());
    }

    public void HideAllText()
    {
        _textPlanet.gameObject.SetActive(false);
    }


    private IEnumerator PopupAnimationRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        _animationPlanet.Play(_animationPlanetPop);
    }
    private IEnumerator PoofAnimationRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        _animationPlanet.Play(_animationPlanetPoof);
    }

}


