using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace discord
{
    public class ChannelName
    {
        public static string GetChannelName(int count)
        {
            var channelNames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("C:\\Users\\TimBrownCCSITSolutio\\Documents\\gaming\\discord\\channelNames.json"));
            return channelNames[count - 1];
        }
    }
}