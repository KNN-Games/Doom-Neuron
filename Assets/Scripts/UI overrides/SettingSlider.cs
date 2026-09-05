using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used on the Setting Slider prefab to simplify updating its values
/// </summary>
[RequireComponent(typeof(Slider))]
public class SettingSlider : MonoBehaviour
{
    [SerializeField] private string settingName;
    [SerializeField] private Slider slider;
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