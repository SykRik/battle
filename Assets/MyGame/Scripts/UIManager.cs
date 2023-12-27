using UnityEngine;
using UnityEngine.UI;

public enum ResultType
{
    WinAttacker,
    WinDefender,
    Draw
}

public class UIManager : SingletonMono<UIManager>
{
    #region ===== Fields =====

    [SerializeField]
    private GameObject result = null;
    [SerializeField]
    private GameObject draw = null;
    [SerializeField]
    private GameObject winA = null;
    [SerializeField]
    private GameObject winD = null;

    [SerializeField]
    private HealthBar healthBarAttacker = null;
    [SerializeField]
    private Text textAttacker = null;

    [SerializeField]
    private HealthBar healthBarDefender = null;
    [SerializeField]
    private Text textDefender = null;
    [SerializeField]
    private Slider sliderZoom = null;

    #endregion

    #region ===== Singletons =====

    GameManager GameManager => GameManager.Instance;

    #endregion

    #region ===== Methods =====

    public override void Init()
    {
        sliderZoom.onValueChanged.AddListener(OnValueChangeZoom);
        Reset();
    }

    public void ShowResult(ResultType resultType)
    {
        result.SetActive(true);
        draw.SetActive(resultType == ResultType.Draw);
        winA.SetActive(resultType == ResultType.WinAttacker);
        winD.SetActive(resultType == ResultType.WinDefender);
    }

    public void HideResult()
    {
        result.SetActive(false);
    }

    public void UpdateHPBar(Team team, float value, string text)
    {
        switch (team)
        {
            case Team.Attacker:
                textAttacker.text = text;
                healthBarAttacker.SetValue(value);
                break;
            case Team.Defender:
                textDefender.text = text;
                healthBarDefender.SetValue(value);
                break;
        }
    }

    public void Reset()
    {
        result.SetActive(false);
        draw.SetActive(false);
        winA.SetActive(false);
        winD.SetActive(false);

        textAttacker.text = string.Empty;
        healthBarAttacker.SetValue(1f);
        textDefender.text = string.Empty;
        healthBarDefender.SetValue(1f);
    }

    public void OnClickSpeed1()
    {
        GameManager.SetTimeScale(1f);
    }

    public void OnClickSpeed2()
    {
        GameManager.SetTimeScale(2f);
    }

    public void OnClickSpeed3()
    {
        GameManager.SetTimeScale(3f);
    }

    private void OnValueChangeZoom(float value)
    {
        Camera.main.orthographicSize = 7f - (4f * value);
    }

    #endregion
}