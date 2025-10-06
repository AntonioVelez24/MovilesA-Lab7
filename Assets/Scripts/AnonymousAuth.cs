using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
public class AnonymousAuth : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);
        SetupEvents();

        await SignAnonymouslyAsync();
    }
    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player ID " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Access Token " + AuthenticationService.Instance.AccessToken);
        };
        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
        };
        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
        };
        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session has expired");
        };
    }
    private async Task SignAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch(AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch(RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}