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

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);
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
            // SocketVoiceState state = stateAfter;
            // if(state.VoiceChannel == null){
            //     state = stateBefore;
            // }
            List<SocketVoiceState> states = new List<SocketVoiceState>();
            if (stateAfter.VoiceChannel != null)
            {
                states.Add(stateAfter);
            }
            if (stateBefore.VoiceChannel != null)
            {
                states.Add(stateBefore);
            }
            foreach (var state in states)
            {
                int channelCount = state.VoiceChannel.Guild.VoiceChannels.Count;
                // var generalChannel = state.VoiceChannel.Guild.TextChannels.Where(x => x.Id == 563397204396736515).First();
                if(state.VoiceChannel.Users.Count == 0)
                {
                    if(channelCount > 3)
                    {
                        if (state.VoiceChannel.Name != "AFK" && state.VoiceChannel.Name != "Lobby")
                        {
                            // await generalChannel.SendMessageAsync(state.VoiceChannel.Name + " deleted");
                            await state.VoiceChannel.DeleteAsync();
                        }
                    }
                } else if (state.VoiceChannel.Users.Count == 1 && state.VoiceChannel.Id != 778589019733884948 && state.VoiceChannel.Id != 554602615204216833) {
                    VoiceChannelProperties properties = new VoiceChannelProperties();
                    properties.UserLimit = 2;
                    properties.CategoryId = 554605557823307811;
                    // Action<VoiceChannelProperties> propertiesAction = new Action<VoiceChannelProperties>();
                    // string channelName = ChannelName.GetChannelName(channelCount);
                    int channelNumber = channelCount - 1;
                    string channelName = "";
                    var channelNames = JsonConvert.DeserializeObject<List<string>>(
                        "[\"Having Kids\",\"Running with Scissors\",\"Crossing the Streams\",\"Getting Married\",\"Fighting Chuck Norris\",\"Knife at a Gun Fight\"]"
                    );
                    
                    int i = 0;
                    while (channelName == "")
                    {
                        if (channelCount >= channelNames.Count + 2)
                        {
                            channelName = "Room " + (channelCount - 1);
                        }
                        Random random = new Random();
                        int number = random.Next(0, channelNames.Count);
                        string name = channelNames[number];
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
                        x.CategoryId = 554605557823307811;
                        x.Position = 1;
                    }, null);
                    // await generalChannel.SendMessageAsync(channelName + " added");
                }
            }
        }
    }
}
