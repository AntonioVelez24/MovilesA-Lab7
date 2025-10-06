using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;

public class AnonymousAuth : MonoBehaviour
{
    private bool isSignedIn = false;

    private void Start()
    {
        InitializeUnityServices();
    }

    private async void InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);
        SetupEvents();
    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player ID " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Access Token " + AuthenticationService.Instance.AccessToken);
            isSignedIn = true;
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
            isSignedIn = false; 
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
            isSignedIn = false;
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session has expired");
            isSignedIn = false; 
        };
    }

    public async Task SignInAnonymously()
    {
        if (!isSignedIn) 
        {
            try
            {
                Debug.Log("Attempting to sign in anonymously...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }
        else
        {
            Debug.Log("Already signed in.");
        }
    }
}
