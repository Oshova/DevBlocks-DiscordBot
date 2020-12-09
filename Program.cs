using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace discord
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private DiscordSocketClient _client;

        // You will need to create a config.json file that complies to the Config class
        // The only required paramters are the token and the categoryId
        // Without these the bot won't be able to connect, and/or won't know where to add voice channels
        public static Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

	        // _client.MessageReceived += MessageReceived;
            _client.UserVoiceStateUpdated += UserVoiceStateUpdated;


            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
        }

        private async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState stateAfter, SocketVoiceState stateBefore)
        {
            List<SocketVoiceState> states = new List<SocketVoiceState>();
            if (stateAfter.VoiceChannel != null)
            {
                states.Add(stateAfter);
            }
            if (stateBefore.VoiceChannel != null)
            {
                states.Add(stateBefore);
            }
            if (states.Count == 2 && states.Any(x => x.VoiceChannel.Users.Count == 2))
            {

            } 
            else if (states.Count == 2 && (states[0].VoiceChannel.Users.Count == 2 || states[1].VoiceChannel.Users.Count == 2))
            {

            } else {
                foreach (var state in states)
                {
                    Console.WriteLine();
                    Console.WriteLine(user.Username + " - " + DateTime.Now);
                    Console.WriteLine("Channel: " + state.VoiceChannel.Name);
                    Console.WriteLine("Users: " + state.VoiceChannel.Users.Count);
                    int channelCount = state.VoiceChannel.Guild.VoiceChannels.Count;
                    if(state.VoiceChannel.Users.Count == 0)
                    {
                        if(channelCount > config.KeepCount)
                        {
                            if (!config.KeepIds.Any(x => x == state.VoiceChannel.Id))
                            {
                                await state.VoiceChannel.DeleteAsync();
                                Console.WriteLine(state.VoiceChannel.Name + " deleted");
                            }
                        }
                    } else if (state.VoiceChannel.Users.Count == 1 && !config.KeepIds.Any(x => x == state.VoiceChannel.Id)) {
                        var existingChannels = state.VoiceChannel.Guild.VoiceChannels.Where(x => x.Users.Count == 0);
                        Console.WriteLine(existingChannels.Count() + " channels existing");
                        if (existingChannels.Count() < (config.KeepCount + 1))
                        {
                            int channelNumber = channelCount - 1;
                            string channelName = "";
                            // var channelNames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("channelNames.json"));
                            
                            int i = 0;
                            while (channelName == "")
                            {
                                if (channelCount >= config.ChannelNames.Count + 2)
                                {
                                    channelName = "Room " + (channelCount - 1);
                                }
                                Random random = new Random();
                                int number = random.Next(0, config.ChannelNames.Count);
                                string name = config.ChannelNames[number];
                                if (!state.VoiceChannel.Guild.VoiceChannels.Any(x => x.Name == name))
                                {
                                    channelName = name;
                                }
                                i++;
                            }
                            if (channelName == "")
                            {
                                channelName = "Room " + (channelCount - 1);
                            }
                            var test = await state.VoiceChannel.Guild.CreateVoiceChannelAsync(channelName, x =>
                            {
                                x.CategoryId = config.CategoryId;
                                x.Position = 1;
                            }, null);
                            Console.WriteLine(channelName + " created");
                        } else {
                            Console.WriteLine("Enough channels already");
                        }
                    }
                }
            }
        }
    }
}
