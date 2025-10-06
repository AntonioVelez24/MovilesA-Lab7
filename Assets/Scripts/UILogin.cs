using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button loginButton;

    [SerializeField] private UnityPlayerAuth unityPlayerAuth;

    private void OnEnable()
    {
        loginButton?.onClick.AddListener(LoginButton);
        unityPlayerAuth.OnSignedIn += UnityPlayerOnSignedIn;
    }

    private void UnityPlayerOnSignedIn(PlayerInfo playerInfo, string PlayerName)
    {
        SceneManager.LoadScene("Menu");
    }

    private async void LoginButton()
    {
        await unityPlayerAuth.InitSignIn();
    }

    private void OnDisable()
    {
        loginButton?.onClick.RemoveListener(LoginButton);
        unityPlayerAuth.OnSignedIn -= UnityPlayerOnSignedIn;
    }
}