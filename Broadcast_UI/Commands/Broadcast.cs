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
    [Command("bc")]
    [CommandDescription("Announce a message")]
    [CommandSyntax("<type> <message>")]
    public class Broadcast : Command
    {
        private readonly UnturnedUserDirectory _UnturnedUserDirectory;
        public Broadcast(IServiceProvider serviceProvider, UnturnedUserDirectory unturnedUserDirectory) : base(serviceProvider)
        {
            _UnturnedUserDirectory = unturnedUserDirectory;
        }

        protected override async Task OnExecuteAsync()
        {
            var actor = Context.Actor as UnturnedUser;

            if (actor == null) 
            { 
                throw new UserFriendlyException("An unknown error occurred.");
            }

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

            switch (type)
            {
                case "general":
                    announce = "Announcement";
                    id = 19366;
                    key = 366;
                    break;
                case "staff" or "s":
                    announce = "Staff Announcement";
                    id = 19369;
                    key = 369;
                    break;
                case "event" or "e":
                    announce = "Event Announcement";
                    id = 19367;
                    key = 367;
                    break;
                case "police" or "pd":
                    announce = "PD Announcement";
                    id = 19368;
                    key = 368;
                    break;
            }

            Parallel.ForEach(_UnturnedUserDirectory.GetOnlineUsers(), async x =>
            {
                await UniTask.SwitchToMainThread();

                EffectManager.sendUIEffect(id, key, x.SteamId, true);
                EffectManager.sendUIEffectText(key, actor.SteamId, false, "Variable {0}", announce);
                EffectManager.sendUIEffectText(key, actor.SteamId, false, "Variable {1}", message);

                await UniTask.SwitchToThreadPool();
                await UniTask.Delay(10000);
                await UniTask.SwitchToMainThread();

                EffectManager.askEffectClearByID(id, x.SteamId);
            });
        }
    }
}
