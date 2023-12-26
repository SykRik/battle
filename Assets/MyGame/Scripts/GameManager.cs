using AxieMixer.Unity;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    #region ===== Fields =====

    private HeroData    dataAttacker = null;
    private HeroData    dataDefender = null;

    #endregion

    #region ===== Properties =====

    public HeroData DataAttacker => dataAttacker;
    public HeroData DataDefender => dataDefender;

    #endregion

    #region ===== Singletons =====

    UIManager   UIManager   => UIManager.Instance;
    MapManager  MapManager  => MapManager.Instance;
    AxieManager AxieManager => AxieManager.Instance;

    #endregion

    #region ===== Methods =====

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        dataAttacker = new HeroData
        {
            AxieID  = "4191804",
            Genes   = "0x20000000000001000281a000810c0000000100102080830a000100141061020a000100101880c4060001000c0880c50c000100101820c4060001001008804402",
            HP      = 16
        };
        dataDefender = new HeroData
        {
            AxieID  = "2724598",
            Genes   = "0x18000000000001000240a050c210000000010008286045040001000c08814302000100042021430c0001000c2861430a0001000c3061830c0001000c08404302",
            HP      = 32
        };

        Mixer.Init();
        UIManager.Init();
        MapManager.Init();
        AxieManager.Init();
    }

    public void Reset()
    {
        UIManager.Reset();
        MapManager.Reset();
        AxieManager.Reset();
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    #endregion
}
