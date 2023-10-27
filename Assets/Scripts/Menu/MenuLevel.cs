using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LevelType
{
    Venus = 0,
    Mars = 1,
    Saturn = 2,
    Pluto = 3,
    Mercury = 4
}

public class MenuLevel : MonoBehaviour
{
    [Header("My Planet is")]
    [SerializeField]
    private LevelType _myLevelType;
    public LevelType MyLevelType => _myLevelType;

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
        switch (_myLevelType)
        {
            case LevelType.Venus:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.VenusCompletion;
                break;
            case LevelType.Mars:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.MarsCompletion;
                break;
            case LevelType.Saturn:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.SaturnCompletion;
                break;
            case LevelType.Pluto:
                _factSheetData.CompletionPercentage = GameManager.Data.PlanetCompletionValues.PlutoCompletion;
                break;
            case LevelType.Mercury:
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


