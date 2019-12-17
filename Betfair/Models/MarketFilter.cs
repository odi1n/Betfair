using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Betfair.Models
{
    public class MarketFilter
    {
        /// <summary>
        /// Ограничьте рынки любым текстом, связанным с рынком, таким как Имя, Событие, Конкуренция и т. Д. Вы можете включать символ подстановки (*), если он не является первым символом.
        /// </summary>
        [JsonProperty(PropertyName = "textQuery")]
        public string TextQuery { get; set; }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        [JsonProperty(PropertyName = "exchangeIds")]
        public ISet<string> ExchangeIds { get; set; }

        /// <summary>
        /// Ограничить рынки по типу события, связанного с рынком. (т. е. футбол, хоккей и т. д.)
        /// </summary>
        [JsonProperty(PropertyName = "eventTypeIds")]
        public ISet<string> EventTypeIds { get; set; }

        /// <summary>
        /// Ограничить рынки с помощью идентификатора события, связанного с рынком.
        /// </summary>
        [JsonProperty(PropertyName = "eventIds")]
        public ISet<string> EventIds { get; set; }

        /// <summary>
        /// Ограничьте рынки соревнованиями, связанными с рынком.
        /// </summary>
        [JsonProperty(PropertyName = "competitionIds")]
        public ISet<string> CompetitionIds { get; set; }

        /// <summary>
        /// Ограничьте рынки идентификатором рынка, связанным с рынком.
        /// </summary>
        [JsonProperty(PropertyName = "marketIds")]
        public ISet<string> MarketIds { get; set; }

        /// <summary>
        /// Ограничьте рынки местом, связанным с рынком. В настоящее время только рынки скачек имеют места.
        /// </summary>
        [JsonProperty(PropertyName = "venues")]
        public ISet<string> Venues { get; set; }

        /// <summary>
        /// Ограничьте только рынки BSP, если True или не BSP, если False. Если не указано, то возвращает рынки BSP и не BSP
        /// </summary>
        [JsonProperty(PropertyName = "bspOnly")]
        public bool? BspOnly { get; set; }

        /// <summary>
        /// Ограничьте рынки, которые перейдут в игру, если True, или не превратятся в игру, если false. Если не указано, возвращает оба.
        /// </summary>
        [JsonProperty(PropertyName = "turnInPlayEnabled")]
        public bool? TurnInPlayEnabled { get; set; }

        /// <summary>
        /// Ограничить рынки, которые в данный момент находятся в игре, если True, или не в данный момент в игре, если false. Если не указано, возвращает оба.
        /// </summary>
        [JsonProperty(PropertyName = "inPlayOnly")]
        public bool? InPlayOnly { get; set; }

        /// <summary>
        /// Ограничить рынки, которые соответствуют типу ставок на рынке (например, коэффициенты, одиночные игры с азиатским гандикапом, удвоение или линия азиатского гандикапа)
        /// </summary>
        [JsonProperty(PropertyName = "marketBettingTypes")]
        public ISet<MarketBettingType> MarketBettingTypes { get; set; }

        /// <summary>
        /// Ограничить рынки, которые находятся в указанной стране или странах
        /// </summary>
        [JsonProperty(PropertyName = "marketCountries")]
        public ISet<string> MarketCountries { get; set; }

        /// <summary>
        /// Ограничить рынки, которые соответствуют типу рынка (например, MATCH_ODDS, HALF_TIME_SCORE). Вы должны использовать это вместо того, чтобы полагаться на название рынка, так как коды типов рынка одинаковы во всех локалях.
        /// </summary>
        [JsonProperty(PropertyName = "marketTypeCodes")]
        public ISet<string> MarketTypeCodes { get; set; }

        /// <summary>
        /// Ограничить рынки с временем начала рынка до или после указанной даты
        /// </summary>
        [JsonProperty(PropertyName = "marketStartTime")]
        public TimeRange MarketStartTime { get; set; }

        /// <summary>
        /// Ограничить рынки, на которых у меня есть один или несколько заказов в этом статусе.
        /// </summary>
        [JsonProperty(PropertyName = "withOrders")]
        public ISet<OrderStatus> WithOrders { get; set; }

        /// <summary>
        /// Ограничение по типу расы (например, барьер, плоскость, бампер, подвеска, погоня)
        /// </summary>
        [JsonProperty(PropertyName = "raceTypes")]
        public ISet<string> RaceTypes { get; set; }

    }
}
