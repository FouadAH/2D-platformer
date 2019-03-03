using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_HUD : MonoBehaviour
{
    public Slider healthSlider;
    public Slider healthSliderGhost;
    public TMPro.TMP_Text currencyText;
    public static UI_HUD instance;
    private float previousHealthPercent;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        previousHealthPercent = 1;
    }

    void Update()
    {
        currencyText.SetText(GameSceneManager.instance.currency.ToString());
        healthSlider.value = CalculateHealthPercent();
        healthSliderGhost.value = Mathf.MoveTowards(previousHealthPercent, CalculateHealthPercent(), (1f / 5f) * Time.deltaTime);
        previousHealthPercent = healthSliderGhost.value;
    }

    private float CalculateHealthPercent()
    {
        return GameSceneManager.instance.health / GameSceneManager.instance.maxHealth;
    }
}
