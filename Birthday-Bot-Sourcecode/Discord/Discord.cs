using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Discord.Webhook;
using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Xml;

namespace Discord
{
    public class Discord
    {
        public static Task Main(string token, ulong[] masters) => new Discord().MainAsync(token, masters);

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private DiscordSocketClient _client = new DiscordSocketClient();

        private ulong[] _Masters = { };

        private async Task MainAsync(string token, ulong[] masters)
        {
            Console.WriteLine("Initialising");

            _Masters = masters;

            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

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
        }


        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            EmbedBuilder emb = new EmbedBuilder();
            Embed[] embeds = new Embed[1];
            if (_Masters.Contains(command.User.Id))
            {
                Console.WriteLine("DC Master has wridden.");
            }
            switch (command.CommandName)
            {
                case "infome":
                    emb.WithAuthor(command.User.Username, command.User.GetAvatarUrl());
                    emb.WithDescription(command.User.Mention);
                    emb.WithTitle("Userinfo");
                    EmbedFieldBuilder field = new EmbedFieldBuilder();
                    field.WithName("ID");
                    field.WithValue(command.User.Id);
                    emb.WithFields(field);
                    embeds[0] = emb.Build();
                    await command.RespondAsync("", embeds);
                    break;
                default:
                    await command.RespondAsync($"You executed {command.Data.Name}");
                    break;
            }
        }
    }
}
