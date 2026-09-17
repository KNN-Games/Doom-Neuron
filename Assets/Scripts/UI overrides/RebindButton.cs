using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RebindButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private GameObject resetButton;
    private UnityAction resetAction;
    public void ResetSetting()
    {
        resetAction?.Invoke();
        OptionsMenu.Instance.UpdateKeyRebindButtons(); // Update UI
        button.Select();
    }
    public void UpdateButton(string text, bool valueNotDefault)
    {
        keyText.text = text;
        resetButton.SetActive(valueNotDefault);
    }
    public void ChangeOnClickAction(UnityAction call, UnityAction resetCall)
    {
        resetAction = resetCall;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(call);
    }
}
