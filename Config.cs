using System.Collections.Generic;

namespace discord
{
    public class Config
    {
        public string Token { get; set; }
        public ulong CategoryId { get; set; }
        public List<ulong> KeepIds { get; set; }
    }
}