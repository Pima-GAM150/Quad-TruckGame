#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void

using DSharpPlus;
using DSharpPlus.Net.WebSocket;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using DSharpPlus.EventArgs;
using System.Net;
using DSharpPlus.Exceptions;
using DSharpPlus.Entities;
using System.Linq;

public class DiscordChatActor : MonoBehaviour
{
    public delegate void ExceptionEventHandler(object sender, Exception ex);

    public delegate void ClientEvent(DiscordClient client);

    public delegate void NotificationEvent(string message);

    public event ExceptionEventHandler FailedLogin;

    public event ClientEvent OnBeforeConnect;

    public event NotificationEvent OnInviteCreated;

    public DiscordGuild Guild;

    private string _guildName;
    public static DiscordChatActor Instance { get; private set; }
    public DiscordClient Client { get; private set; }

    public async Task Run(string token, string name)
    {
        if (Client == null)
            CreateClient(token, name);
        try
        {
            OnBeforeConnect?.Invoke(Client);
            await Client.ConnectAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"{Log.Timestamp()} Discord-Login: {e.Message} {e.StackTrace}");
            FailedLogin?.Invoke(Client, e);
            Client = null;
        }
    }

    private string _name;

    public async Task Stop()
    {
        if (Client != null)
        {
            await Client.DisconnectAsync();
            Client = null;
        }
    }

    public void CreateClient(string token, string guildName)
    {
        // Disable SSL Certificate Validation
        // see: https://dsharpplus.emzi0767.com/articles/hosting_rpi.html#method-4-run-your-bot-using-mono
        ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;

        // Setup Client only if this is the first time.
        if (Client == null)
        {
            _guildName = guildName;
            Client = new DiscordClient(GenerateConfig(token));
            Client.SetWebSocketClient<WebSocketSharpClient>();

            Client.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
            Client.Ready += Client_Ready;
            Client.ClientErrored += Client_ClientErrored;
            Client.GuildAvailable += Client_GuildAvailable;
        }
    }

    private async Task Client_GuildAvailable(GuildCreateEventArgs e)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(async () => await Client_GuildAvailable(e));
            return;
        }
        Debug.Log($"Guild Available {e.Guild.Name}");
        Guild = e.Guild;

        if (e.Guild.Name != _guildName)
        {
            await Guild.ModifyAsync(_guildName);
            Debug.Log($"Renamed Guild to {Guild.Name}");
        }

        var invites = await Guild.GetInvitesAsync();
        DiscordInvite invite;
        if (invites.Any())
        {
            invite = invites.First();
        }
        else
        {
            invite = await Guild.GetDefaultChannel().CreateInviteAsync();
        }
        PushNotification.Instance.Push($"Invite: {invite}", "Success");
        OnInviteCreated?.Invoke(invite.ToString());
    }

    private async Task Client_Ready(ReadyEventArgs e)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(async () => await Client_Ready(e));
            return;
        }
        Debug.Log($"{Log.Timestamp()} Discord-ClientReady: Client is connected.");
        PushNotification.Instance.Push($"Connected as: {e.Client.CurrentUser.Username}", "Success");

        if (Client.Guilds.Count > 0)
        {
            Debug.Log($"Guild already exists. {Client.Guilds.First()}");
            return;
        }

        Guild = await Client.CreateGuildAsync(_guildName);
        var invite = await Guild.Channels.First().CreateInviteAsync();
        PushNotification.Instance.Push($"Invite: {invite}", "Success");
        OnInviteCreated?.Invoke(invite.ToString());
    }

    private Task Client_ClientErrored(ClientErrorEventArgs e)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(() => Client_ClientErrored(e));
            return Task.CompletedTask;
        }

        Debug.LogError($"{Log.Timestamp()} Discord-ClientErrored: {e.Exception}");
        PushNotification.Instance.Push("Failed to connect to discord. Is your key valid?", "Failed");
        return Task.CompletedTask;
    }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private async void OnDestroy()

    {
        if (Instance = this)
            Instance = null;

        await Stop();
    }

    private void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(() => DebugLogger_LogMessageReceived(sender, e));
            return;
        }

        var msg = $"{Log.Timestamp()} Discord-{e.Level}: {e.Message}";
        var push = PushNotification.Instance;
        push.Push(e.Message, e.Level.ToString());
    }

    private DiscordConfiguration GenerateConfig(string token)
    {
        return new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,

            AutoReconnect = true,
            LogLevel = LogLevel.Debug,
            UseInternalLogHandler = false
        };
    }
}