using AxieMixer.Unity;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    #region ===== Fields =====

    private HeroData    dataAttacker = null;
    private HeroData    dataDefender = null;
    private float       timeScale = 1f;
    private float       intervalStartTime = 3f;
    private float       remainStartTime = 0f;
    private float       intervalTurnTime = 1f;
    private float       remainTurnTime = 0f;

    #endregion

    #region ===== Properties =====

    #endregion

    #region ===== Methods =====

    protected override void Init()
    {
        dataAttacker = new HeroData
        {
            AxieID = "4191804",
            Genes = "0x20000000000001000281a000810c0000000100102080830a000100141061020a000100101880c4060001000c0880c50c000100101820c4060001001008804402",
            HP = 16
        };
        dataDefender = new HeroData
        {
            AxieID = "2724598",
            Genes = "0x18000000000001000240a050c210000000010008286045040001000c08814302000100042021430c0001000c2861430a0001000c3061830c0001000c08404302",
            HP = 16
        };

        Mixer.Init();
    }

    public void SetTimeScale(float timeScale)
    { 
        this.timeScale = timeScale;
    }

    public void ResetTimeScale()
    {
        this.timeScale = 1f;
    }

    private void Update()
    {
        if (remainStartTime > 0)
        {
            remainStartTime -= Time.deltaTime * timeScale;
        }
        else if (remainTurnTime > 0)
        {
            remainTurnTime -= Time.deltaTime * timeScale;
        }
        else
        { }
    }

    #endregion
}
