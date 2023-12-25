using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region ===== Fields =====

    private LTDescr tween = null;
    [SerializeField]
    private Image bar = null;
    [SerializeField]
    private float duration = 0.5f;

    #endregion

    #region ===== Methods =====

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        if (tween != null)
        {
            LeanTween.cancel(tween.uniqueId);
        }
        if (bar != null)
        {
            bar.fillAmount = 1f;
        }
    }

    public void SetValue(float value)
    {
        if (tween != null)
        {
            LeanTween.cancel(tween.uniqueId);
        }
        if (bar != null)
        {
            tween = LeanTween.value(gameObject, bar.fillAmount, value, duration)
                             .setEaseOutCubic()
                             .setOnUpdate(value => bar.fillAmount = value)
                             .setOnComplete(() => tween = null);
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        var value   = bar.fillAmount > 0 
    //                    ? Mathf.Clamp01(bar.fillAmount - 0.1f) 
    //                    : 1f;
    //        SetValue(value);
    //    }
    //}

    #endregion
}