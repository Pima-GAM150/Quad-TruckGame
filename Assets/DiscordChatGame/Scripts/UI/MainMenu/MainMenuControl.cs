#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using UnityEngine;
using UnityEngine.UI;
using DSharpPlus;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class MainMenuControl : SerializedMonoBehaviour
{
    public UISwitchGroup Switch;

    [Required]
    public TMP_InputField TokenField;

    [Required]
    public TMP_InputField ServerField;

    [Required]
    public Button TokenButton;

    public void StartButtonClicked()
    {
        //TODO: Move to another script
        SceneManager.LoadScene("TestRoom");
    }

    public async void Start()
    {
        await GetConfig();
        OnTokenChanged();
    }

    public void OnTokenChanged()
    {
        var canSubmit = TokenField.text.Length > 0 && ServerField.text.Length > 0;
        TokenButton.interactable = canSubmit;
    }

    public void OnTokenScreenCommit()
    {
        TokenButton.interactable = false;

        TokenField.interactable = false;

        var ctx = DiscordChatActor.Instance;
        Debug.Log($"{Log.Timestamp()} Sending Token {TokenField.text} to DiscordLauncher");
        ctx.CreateClient(TokenField.text);
        ctx.Client.Ready += Client_Ready;
        ctx.FailedLogin += Client_FailedLogin;
        ctx.Run(TokenField.text);
    }

    private async Task GetConfig()
    {
        var discordConfig = await ConfigManager.Instance.GetConfig<DiscordConfig>();
        TokenField.text = discordConfig.Token;
        ServerField.text = discordConfig.ServerName;
    }

    private void Client_FailedLogin(object sender, Exception ex)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(() => Client_FailedLogin(sender, ex));
            return;
        }
        PushNotification.Instance.Push(ex.Message, "Failed");
        TokenField.interactable = true;
        TokenButton.interactable = true;
    }

    private async Task Client_Ready(ReadyEventArgs e)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(async () => await Client_Ready(e));
            return;
        }
        var config = await ConfigManager.Instance.GetConfig<DiscordConfig>();
        config.Token = TokenField.text;
        config.ServerName = ServerField.text;
        config.Save();

        Switch.SwitchTo("MainMenu");
    }
}