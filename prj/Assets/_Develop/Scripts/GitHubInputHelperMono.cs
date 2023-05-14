using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class GitHubInputHelperMono : MonoBehaviour
{
    [SerializeField] private TMP_InputField _ownerName;
    [SerializeField] private TMP_InputField _repoName;
    [SerializeField] private TMP_InputField _clientId;
    [SerializeField] private TMP_InputField _clientSecret;
    [SerializeField] private Toggle _useOAuth;
    [SerializeField] private Button _loadButton;

    private void Awake()
    {
        _ownerName.text = PlayerPrefs.GetString(nameof(_ownerName), "");
        _repoName.text = PlayerPrefs.GetString(nameof(_repoName), "");
        _clientId.text = PlayerPrefs.GetString(nameof(_clientId), "");
        _clientSecret.text = PlayerPrefs.GetString(nameof(_clientSecret), "");
        _useOAuth.isOn = PlayerPrefs.GetInt(nameof(_useOAuth), 0) == 1;
        _ownerName.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetString(nameof(_ownerName), x);
            PlayerPrefs.Save();
        });
        _repoName.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetString(nameof(_repoName), x);
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
        _useOAuth.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetInt(nameof(_useOAuth), x ? 1 : 0);
            PlayerPrefs.Save();
        });
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (_ownerName.isFocused)
            {
                _repoName.ActivateInputField();
            }
            else if (_repoName.isFocused)
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
