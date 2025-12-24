using Database;
using Database.Con;
using Common;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Webhook;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Cfg;
//using System.Xml;

namespace Discord
{
    public class Discord
    {
        public static Task Main(config cfg) => new Discord().MainAsync(cfg);

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private static DiscordSocketClient _client = null;
        private config config = null;

        Thread postLoop = null;

        public static DiscordSocketClient instanz
        {
            get
            {
                if (_client == null)
                {
                    throw new NullReferenceException();
                }
                return _client;
            }
        }

        private ulong[] _Masters = { };

        private async Task MainAsync(config cfg)
        {
            config = cfg;
            DiscordSocketConfig socketCfg = new DiscordSocketConfig
            {
                WebSocketProvider = DefaultWebSocketProvider.Create(WebRequest.GetSystemWebProxy()),
                UdpSocketProvider = DefaultUdpSocketProvider.Instance
            };

            _client = new DiscordSocketClient(socketCfg);
            Console.WriteLine("Initialising");

            _Masters = cfg.MasterDiscord;
            
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, cfg.DiscordToken);
            await _client.StartAsync();

            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
            _client.JoinedGuild += TryAddGuildCommands;


            postLoop = new Thread(PostLoop.Instance.postLoop);
            
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task Client_Ready()
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            //var guild = _client.GetGuild(guildId);

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            //var guildCommand = new SlashCommandBuilder();

            // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            //guildCommand.WithName("infome");

            // Descriptions can have a max length of 100.
            //guildCommand.WithDescription("");
            try
            {
                List<ApplicationCommandProperties> applicationCommandProperties = new();

                var globalCommand_infome = new SlashCommandBuilder();
                globalCommand_infome.WithName("infome");
                globalCommand_infome.WithDescription("Answers with user information");
                applicationCommandProperties.Add(globalCommand_infome.Build());

                // Let's do our global command

                foreach (SocketGuild guild in _client.Guilds)
                {
                    await addGuildCommands(guild);
                }

                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                //await guild.CreateApplicationCommandAsync(guildCommand.Build());

                // With global commands we don't need the guild.
                await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
                //await _client.CreateGlobalApplicationCommandAsync(globalCommand_infome.Build());

                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
            if (postLoop.ThreadState != ThreadState.Running)
            {
                postLoop.Start(config);
            }
        }

        private async Task TryAddGuildCommands(SocketGuild guild)
        {
            try
            {
                addGuildCommands(guild);
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        private async Task addGuildCommands(SocketGuild guild)
        {
            List<ApplicationCommandProperties> applicationCommandPropertiesGuild = new();
            ulong _guildId = guild.Id;
            var monthOption = new SlashCommandOptionBuilder()
                .WithName("monat")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithMaxValue(12)
                .WithMinValue(1)
                .WithDescription("Dein Geburtsmonat als Zahl zwischen 1 und 12")
                .WithRequired(true);
            var dayOption = new SlashCommandOptionBuilder()
                .WithName("tag")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithMaxValue(31)
                .WithMinValue(1)
                .WithDescription("Dein Geburtstag als Zahl zwischen 1 und 31")
                .WithRequired(true);
            var yearOption = new SlashCommandOptionBuilder()
                .WithName("jahr")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithDescription("Dein Geburtsjahr (optional)")
                .WithRequired(false);
            var globalCommand_setBirthday = new SlashCommandBuilder();
            globalCommand_setBirthday.WithName("geburtstag");
            globalCommand_setBirthday.WithDescription("Hier kannst du deinen Geburtstag hinzufügen");
            globalCommand_setBirthday.AddOption(dayOption);
            globalCommand_setBirthday.AddOption(monthOption);
            globalCommand_setBirthday.AddOption(yearOption);
            applicationCommandPropertiesGuild.Add(globalCommand_setBirthday.Build());

            var globalCommand_deleteBirthday = new SlashCommandBuilder();
            globalCommand_deleteBirthday.WithName("vergissmich");
            globalCommand_deleteBirthday.WithDescription("Hiermit kannst du deinen Geburtstag");
            applicationCommandPropertiesGuild.Add(globalCommand_deleteBirthday.Build());

            var hourOption = new SlashCommandOptionBuilder()
                .WithName("stunde")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithMaxValue(24)
                .WithMinValue(0)
                .WithDescription("Stunde zu der Geposted werden soll.")
                .WithRequired(true);
            var minuteOption = new SlashCommandOptionBuilder()
                .WithName("minute")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithMaxValue(59)
                .WithMinValue(0)
                .WithDescription("Minute zu der Geposted werden soll.")
                .WithRequired(true);
            var globalCommand_setTime = new SlashCommandBuilder();
            globalCommand_setTime.WithName("set_time");
            globalCommand_setTime.WithDescription("Hier kann der Moderator oder Server-Owner die Post-Zeit einstellen.");
            globalCommand_setTime.AddOption(hourOption);
            globalCommand_setTime.AddOption(minuteOption);
            applicationCommandPropertiesGuild.Add(globalCommand_setTime.Build());

            var modRoleOption = new SlashCommandOptionBuilder()
                .WithName("modrole")
                .WithType(ApplicationCommandOptionType.Mentionable)
                .WithDescription("Rolle des Mods")
                .WithRequired(true);

            var globalCommand_setModRole = new SlashCommandBuilder();
            globalCommand_setModRole.WithName("enmod");
            globalCommand_setModRole.WithDescription("Hier kann die Rolle der Mods gesetzt werden.");
            globalCommand_setModRole.AddOption(modRoleOption);
            applicationCommandPropertiesGuild.Add(globalCommand_setModRole.Build());

            var globalCommand_setChannel = new SlashCommandBuilder();
            globalCommand_setChannel.WithName("set_channel");
            globalCommand_setChannel.WithDescription("Der Channel, in welchem der Command Ausgeführt wird, bekommt Benachrichtigungen des Bots.");
            applicationCommandPropertiesGuild.Add(globalCommand_setChannel.Build());

            SocketGuild guildSrc = _client.GetGuild(_guildId);
            await guildSrc.BulkOverwriteApplicationCommandAsync(applicationCommandPropertiesGuild.ToArray());
        }


        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            EmbedBuilder emb = new EmbedBuilder();
            Embed[] embeds = new Embed[1];
            bool mod = false;
            bool master = false;
            bool right_channel = false;
            ulong right_channelId = 0;
            if (command.GuildId is not null)
            {
                ulong modRoleId = DatabaseConnector.instanze.getMod(command.GuildId.Value);
                SocketRole[] usrRoles = ((SocketGuildUser)command.User).Roles.ToArray();
                if (modRoleId != 0)
                {
                    foreach (SocketRole role in usrRoles)
                    {
                        if (modRoleId == role.Id)
                        {
                            mod = true;
                        }
                    }
                }
                if(!mod)
                {
                    SocketUser owner = _client.GetGuild(command.GuildId.Value).Owner;
                    if (owner.Id == command.User.Id)
                    {
                        mod = true;
                        Console.WriteLine("Owner of Discord.");
                    }
                }
                SocketTextChannel cmd_channel = (SocketTextChannel)command.Channel;
                right_channelId = DatabaseConnector.instanze.getChannel(command.GuildId.Value);
                if (right_channelId != 0)
                {
                    if (cmd_channel.Id == right_channelId)
                    {
                        right_channel = true;
                    }
                }
                else
                {
                    right_channel = true;
                }
            }
            if (_Masters.Contains(command.User.Id))
            {
                Console.WriteLine("DC Master has wridden.");
                master = true;
                mod = true;
            }
            ulong ServerId = 0;
            ulong MemberId = 0;
            switch (command.CommandName)
            {
                case "infome":
                    if (command.GuildId is null)
                    {
                        await command.RespondAsync($"Der Command kann nur auf einem Server ausgeführt werden.");
                        break;
                    }
                    if (!right_channel)
                    {
                        await command.RespondAsync($"Der Command kann nur im Channel <#{right_channelId}> ausgeführt werden.", null, false, true);
                        break;
                    }
                    emb.WithAuthor(command.User.Username, command.User.GetAvatarUrl());
                    emb.WithDescription(command.User.Mention);
                    emb.WithTitle("Userinfo");
                    EmbedFieldBuilder field = new EmbedFieldBuilder();
                    field.WithName("ID");
                    field.WithValue(command.User.Id);
                    EmbedFieldBuilder field_birthday = new EmbedFieldBuilder();
                    emb.WithFields(field);
                    int[] birthday_array = DatabaseConnector.instanze.getBirthday(command.User.Id);
                    field_birthday.WithName("Geburtstag");
                    if (birthday_array !=  null)
                    {
                        string birthday = helper.intArrayToBorthdayString(birthday_array);
                        field_birthday.WithValue(birthday);
                    } else
                    {
                        field_birthday.WithValue("nicht angegeben");
                    }
                    emb.WithFields(field_birthday);
                    embeds[0] = emb.Build();
                    await command.RespondAsync("", embeds);
                    break;
                case "geburtstag":
                    if (command.GuildId is null)
                    {
                        await command.RespondAsync($"Der Command kann nur auf einem Server ausgeführt werden.");
                        break;
                    }
                    if (!right_channel)
                    {
                        await command.RespondAsync($"Der Command kann nur im Channel <#{right_channelId}> ausgeführt werden.", null, false, true);
                        break;
                    }
                    ServerId = command.GuildId.Value;
                    MemberId = command.User.Id;
                    long day = -1;
                    long month = -1;
                    long year = -1;
                    foreach (SocketSlashCommandDataOption option in command.Data.Options)
                    {
                        switch (option.Name)
                        {
                            case "tag":
                                day = (long)option.Value;
                                break;
                            case "monat":
                                month = (long)option.Value;
                                break;
                            case "jahr":
                                year = (long)option.Value;
                                break;
                            default:
                                break;
                        }
                    }
                    if (year == -1)
                    {
                        DatabaseConnector.instanze.setBirthday(ServerId, MemberId, day, month);
                    }
                    else
                    {
                        DatabaseConnector.instanze.setBirthday(ServerId, MemberId, day, month, year);
                    }
                    emb.WithDescription($"{command.User.Mention} dein Geburtstag wurde für diesen Server hinzugefügt.");
                    embeds[0] = emb.Build();
                    await command.RespondAsync("", embeds);
                    break;
                case "vergissmich":
                    if (command.GuildId is null)
                    {
                        await command.RespondAsync($"Der Command kann nur auf einem Server ausgeführt werden.");
                        break;
                    }
                    if (!right_channel)
                    {
                        await command.RespondAsync($"Der Command kann nur im Channel <#{right_channelId}> ausgeführt werden.", null, false, true);
                        break;
                    }
                    ServerId = command.GuildId.Value;
                    MemberId = command.User.Id;
                    DatabaseConnector.instanze.deleteBirthday(ServerId, MemberId);
                    emb.WithDescription($"{command.User.Mention} dein Geburtstag wurde für diesen Server entfernt.");
                    embeds[0] = emb.Build();
                    await command.RespondAsync("", embeds);
                    break;
                case "enmod":
                    if (command.GuildId is null)
                    {
                        await command.RespondAsync($"Der Command kann nur auf einem Server ausgeführt werden.");
                        break;
                    }
                    if (!mod)
                    {
                        await command.RespondAsync($"Der Command muss durch einen Mod ausgeführt werden.", null, false, true);
                        break;
                    }
                    SocketSlashCommandDataOption modrole = command.Data.Options.First();
                    if (modrole.Value.GetType() != typeof(SocketRole))
                    {
                        await command.RespondAsync($"Es muss eine Rolle angegeben werden.", null, false, true);
                        break;
                    }
                    SocketRole socketRole = (SocketRole)modrole.Value;
                    if (socketRole.IsManaged)
                    {
                        await command.RespondAsync($"Die Rolle muss Serverspezifisch sein.", null, false, true);
                        break;
                    }
                    DatabaseConnector.instanze.setMod(command.GuildId.Value, socketRole.Id);
                    emb.WithAuthor(command.User.Username, command.User.GetAvatarUrl());
                    emb.WithDescription($"Die Modrolle des Bots gesetzt.");
                    emb.WithTitle("Enmod");
                    EmbedFieldBuilder field_modrole = new EmbedFieldBuilder();
                    field_modrole.WithName("Rolle");
                    field_modrole.WithValue($"{socketRole.Mention}");
                    emb.WithFields(field_modrole);
                    embeds[0] = emb.Build();
                    await command.RespondAsync("", embeds);
                    break;
                case "set_channel":
                    if (command.GuildId is null)
                    {
                        await command.RespondAsync($"Der Command kann nur auf einem Server ausgeführt werden.");
                        break;
                    }
                    if (!mod)
                    {
                        await command.RespondAsync($"Der Command muss durch einen Mod ausgeführt werden.", null, false, true);
                        break;
                    }
                    ServerId = command.GuildId.Value;
                    ulong channelid = command.Channel.Id;
                    DatabaseConnector.instanze.setChannel(ServerId, channelid);
                    emb.WithAuthor(command.User.Username, command.User.GetAvatarUrl());
                    emb.WithDescription($"Der Channel des Bots gesetzt.");
                    emb.WithTitle("set_channel");
                    EmbedFieldBuilder field_channel = new EmbedFieldBuilder();
                    field_channel.WithName("Channel");
                    SocketTextChannel channel = (SocketTextChannel) command.Channel;
                    field_channel.WithValue($"{channel.Mention}");
                    emb.WithFields(field_channel);
                    embeds[0] = emb.Build();
                    await command.RespondAsync("", embeds);
                    PostLoop.Instance.reloadMap();
                    break;
                case "set_time":
                    if (command.GuildId is null)
                    {
                        await command.RespondAsync($"Der Command kann nur auf einem Server ausgeführt werden.");
                        break;
                    }
                    if (!mod)
                    {
                        await command.RespondAsync($"Der Command muss durch einen Mod ausgeführt werden.", null, false, true);
                        break;
                    }
                    ServerId = command.GuildId.Value;
                    int hour = 0;
                    int minute = 0;
                    foreach (SocketSlashCommandDataOption option in command.Data.Options)
                    {
                        switch (option.Name)
                        {
                            case "stunde":
                                hour = (int)option.Value;
                                break;
                            case "minute":
                                minute = (int)option.Value;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    await command.RespondAsync($"You executed {command.Data.Name}");
                    break;
            }
        }
    }
}
