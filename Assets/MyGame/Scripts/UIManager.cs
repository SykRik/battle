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

    #endregion

    #region ===== Methods =====

    protected override void Init()
    {
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

    #endregion
}