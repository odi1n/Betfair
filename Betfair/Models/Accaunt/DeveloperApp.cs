using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Betfair.Models
{
    public class DeveloperApp
    {
        [JsonProperty(PropertyName = "appName")]
        public string AppName { get; set; }

        [JsonProperty(PropertyName = "appId")]
        public string AppId { get; set; }

        [JsonProperty(PropertyName = "appVersions")]
        public List<DeveloperAppVersion> AppVersions { get; set; }
    }
}
