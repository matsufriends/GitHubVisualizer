using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class GitHubInputHelperMono : MonoBehaviour
{
    [SerializeField] private TMP_InputField _ownerName;
    [SerializeField] private TMP_Dropdown _dropDown;
    [SerializeField] private TMP_InputField _clientId;
    [SerializeField] private TMP_InputField _clientSecret;
    [SerializeField] private Button _loadButton;

    private void Awake()
    {
        _ownerName.text = PlayerPrefs.GetString(nameof(_ownerName), "");
        var optionCount = PlayerPrefs.GetInt("OptionCount", 0);
        _dropDown.ClearOptions();
        for (var i = 0; i < optionCount; i++)
        {
            var optionData = new TMP_Dropdown.OptionData(PlayerPrefs.GetString($"Option{i}", "None"));
            _dropDown.options.Add(optionData);
        }

        _dropDown.value = PlayerPrefs.GetInt(nameof(_dropDown), -1);
        _clientId.text = PlayerPrefs.GetString(nameof(_clientId), "");
        _clientSecret.text = PlayerPrefs.GetString(nameof(_clientSecret), "");
        _ownerName.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetString(nameof(_ownerName), x);
            PlayerPrefs.Save();
            _dropDown.ClearOptions();
            _dropDown.value = -1;
        });
        _dropDown.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetInt(nameof(_dropDown), x);
            var options = _dropDown.options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                PlayerPrefs.SetString($"Option{i}", option.text);
            }

            PlayerPrefs.SetInt("OptionCount", options.Count);
            PlayerPrefs.Save();
        });
        _clientId.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetString(nameof(_clientId), x);
            PlayerPrefs.Save();
        });
        _clientSecret.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetString(nameof(_clientSecret), x);
            PlayerPrefs.Save();
        });
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (_ownerName.isFocused)
            {
                _clientId.ActivateInputField();
            }
            else if (_clientId.isFocused)
            {
                _clientSecret.ActivateInputField();
            }
            else if (_clientSecret.isFocused)
            {
                _ownerName.ActivateInputField();
            }
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            _loadButton.onClick.Invoke();
        }
    }
}
