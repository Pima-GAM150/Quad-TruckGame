#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void

using System.Collections;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using UnityEngine;
using System.Threading.Tasks;
using System.Text;

public class DiscordCommands : MonoBehaviour
{
    [Command("spawn")]
    [Description("Spawns a GameObject with name specified")]
    [Aliases("sp")]
    public async Task Spawn(CommandContext ctx, [Description("The object ot spawn")] string name)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(async () => await Spawn(ctx, name));
            return;
        }

        await ctx.TriggerTypingAsync();
        var cc = CargoController.Instance;
        if (CargoController.Instance.Cargo.ContainsKey(name))
        {
            Instantiate(cc.Cargo[name], cc.SpawnLocation);
            await ctx.RespondAsync($"Spawned a {name}");
        }
        else
            await ctx.RespondAsync($"Could not find an object with name: {name}");
    }

    [Command("list")]
    [Description("Lists available spawns")]
    [Aliases("list-spawns")]
    public async Task List(CommandContext ctx)
    {
        if (!MainThreadQueue.Instance.IsMain())
        {
            MainThreadQueue.Instance.Queue(async () => await List(ctx));
            return;
        }

        await ctx.TriggerTypingAsync();

        StringBuilder str = new StringBuilder();
        str.AppendLine("Available Spawns");
        str.AppendLine("----------------");
        var cc = CargoController.Instance;

        foreach (var cargo in cc.Cargo)
            str.AppendLine(cargo.Key);

        await ctx.RespondAsync(str.ToString());
    }
}