using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.WpfApp
{
    class FilesInfo
    {
        [JsonProperty("FileName")]
        public string FileName { get; set; }
        [JsonProperty("Url")]
        public string Url { get; set; }
        [JsonProperty("PublicKey")]
        public string PublicKey { get; set; }
        [JsonProperty("Version")]
        public string Version { get; set; }
    }
}
