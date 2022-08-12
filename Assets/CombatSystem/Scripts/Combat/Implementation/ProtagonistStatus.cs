using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProtagonistStatus : MonoBehaviour
{
    [SerializeField] TMP_Text _nameLabel;
    [SerializeField] TMP_Text _maxHPLabel;
    [SerializeField] TMP_Text _currentHPLabel;
    [SerializeField] Image[] _backgrounds = new Image[2];
    [SerializeField] Image _statusHPSlider;
    [SerializeField] Image _statusIcon;

    [SerializeField] OpenTabTween ActionBar;

    public ProtagonistData currentCharacter;

    public void SetupStatus(ProtagonistData charData)
    {

        currentCharacter = charData;

        _nameLabel.text = currentCharacter.CharacterName;
        _maxHPLabel.text = currentCharacter.MaxHP.ToString();
        _currentHPLabel.text = currentCharacter.GetCurrentHP(0).ToString();

        _statusIcon.sprite = currentCharacter.CharacterIcon;
        _statusIcon.color = currentCharacter.CharacterColor;
        _statusHPSlider.color = currentCharacter.CharacterColor;

        foreach (Image bg in _backgrounds)
            bg.color = currentCharacter.CharacterColor;
    }

    public void ToggleActionBar()
    {
        ActionBar.ToggleTab();
    }

    public void UpdateStatus()
    {
        if (currentCharacter == null)
            return;

        _currentHPLabel.text = currentCharacter.GetCurrentHPClamped(0).ToString();
        _statusHPSlider.fillAmount = currentCharacter.FillHPRange(0);
    }

}
