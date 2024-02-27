using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Command = OpenMod.Core.Commands.Command;

namespace BroadcastUI.Commands
{
    [Command("announce")]
    [CommandAlias("ann")]
    [CommandDescription("Announce a message")]
    [CommandSyntax("<type> <message>")]
    public class Announce : Command
    {
        public Announce(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async Task OnExecuteAsync()
        {
            Announce commandDuty = this;
            UnturnedUser actor = (UnturnedUser)((CommandBase)commandDuty).Context.Actor;

            string type = "general";
            string message;
            string announce = "Announcement";
            ushort id = 19366;
            short key = 366;

            if (Context.Parameters.Length == 1)
            {
                message = await Context.Parameters.GetAsync<string>(0);
            }
            else
            {
                type = await Context.Parameters.GetAsync<string>(0);
                message = await Context.Parameters.GetAsync<string>(1);
            }
            await UniTask.SwitchToMainThread();

            switch (type)
            {
                case "general":
                    announce = "Announcement";
                    id = 19366;
                    key = 366;
                    EffectManager.sendUIEffect(19366, 366, actor.SteamId, true);
                    break;
                case "staff" or "s":
                    announce = "Staff Announcement";
                    id = 19369;
                    key = 369;
                    EffectManager.sendUIEffect(19369, 369, actor.SteamId, true);
                    break;
                case "event" or "e":
                    announce = "Event Announcement";
                    id = 19367;
                    key = 367;
                    EffectManager.sendUIEffect(19367, 367, actor.SteamId, true);
                    break;
                case "police" or "pd":
                    announce = "PD Announcement";
                    id = 19368;
                    key = 368;
                    EffectManager.sendUIEffect(19368, 368, actor.SteamId, true);
                    break;
            }

            EffectManager.sendUIEffectText(key, actor.SteamId, false, "Variable {0}", announce);
            EffectManager.sendUIEffectText(key, actor.SteamId, false, "Variable {1}", message);
            await UniTask.SwitchToThreadPool();

            await UniTask.Delay(10000);

            await UniTask.SwitchToMainThread();
            EffectManager.askEffectClearByID(id, actor.SteamId);
        }
    }
}
