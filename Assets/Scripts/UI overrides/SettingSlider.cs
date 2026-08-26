using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SettingSlider : MonoBehaviour
{
    public Slider slider; // Used by OptionsMenu.cs
    [SerializeField] private string settingName;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject resetButton;
    public void ResetSetting()
    {
        OptionsMenu.Instance.ResetSetting(settingName);
    }
    public void UpdateSlider(float valueFloat, string valueString, bool valueNotDefault)
    {
        slider.SetValueWithoutNotify(valueFloat);
        text.text = valueString;
        resetButton.SetActive(valueNotDefault);
    }
}