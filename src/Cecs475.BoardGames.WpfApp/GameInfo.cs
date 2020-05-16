using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.WpfApp
{
    class GameInfo
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Files")]
        public List<FilesInfo> Files { get; set; }

    }
}
