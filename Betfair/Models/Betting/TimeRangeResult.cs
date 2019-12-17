using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Betfair.Models
{
    public class TimeRangeResult
    {
        [JsonProperty(PropertyName = "timeRange")]
        public TimeRange TimeRange { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }

        public override string ToString()
        {
            return new StringBuilder().AppendFormat("{0}", "TimeRangeResult")
                        .AppendFormat(" : TimeRange={0}", TimeRange)
                        .AppendFormat(" : MarketCount={0}", MarketCount)
                        .ToString();
        }
    }
}
