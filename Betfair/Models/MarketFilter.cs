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
        /// ���������� ����� ����� �������, ��������� � ������, ����� ��� ���, �������, ����������� � �. �. �� ������ �������� ������ ����������� (*), ���� �� �� �������� ������ ��������.
        /// </summary>
        [JsonProperty(PropertyName = "textQuery")]
        public string TextQuery { get; set; }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        [JsonProperty(PropertyName = "exchangeIds")]
        public ISet<string> ExchangeIds { get; set; }

        /// <summary>
        /// ���������� ����� �� ���� �������, ���������� � ������. (�. �. ������, ������ � �. �.)
        /// </summary>
        [JsonProperty(PropertyName = "eventTypeIds")]
        public ISet<string> EventTypeIds { get; set; }

        /// <summary>
        /// ���������� ����� � ������� �������������� �������, ���������� � ������.
        /// </summary>
        [JsonProperty(PropertyName = "eventIds")]
        public ISet<string> EventIds { get; set; }

        /// <summary>
        /// ���������� ����� ��������������, ���������� � ������.
        /// </summary>
        [JsonProperty(PropertyName = "competitionIds")]
        public ISet<string> CompetitionIds { get; set; }

        /// <summary>
        /// ���������� ����� ��������������� �����, ��������� � ������.
        /// </summary>
        [JsonProperty(PropertyName = "marketIds")]
        public ISet<string> MarketIds { get; set; }

        /// <summary>
        /// ���������� ����� ������, ��������� � ������. � ��������� ����� ������ ����� ������ ����� �����.
        /// </summary>
        [JsonProperty(PropertyName = "venues")]
        public ISet<string> Venues { get; set; }

        /// <summary>
        /// ���������� ������ ����� BSP, ���� True ��� �� BSP, ���� False. ���� �� �������, �� ���������� ����� BSP � �� BSP
        /// </summary>
        [JsonProperty(PropertyName = "bspOnly")]
        public bool? BspOnly { get; set; }

        /// <summary>
        /// ���������� �����, ������� �������� � ����, ���� True, ��� �� ����������� � ����, ���� false. ���� �� �������, ���������� ���.
        /// </summary>
        [JsonProperty(PropertyName = "turnInPlayEnabled")]
        public bool? TurnInPlayEnabled { get; set; }

        /// <summary>
        /// ���������� �����, ������� � ������ ������ ��������� � ����, ���� True, ��� �� � ������ ������ � ����, ���� false. ���� �� �������, ���������� ���.
        /// </summary>
        [JsonProperty(PropertyName = "inPlayOnly")]
        public bool? InPlayOnly { get; set; }

        /// <summary>
        /// ���������� �����, ������� ������������� ���� ������ �� ����� (��������, ������������, ��������� ���� � ��������� ����������, �������� ��� ����� ���������� ���������)
        /// </summary>
        [JsonProperty(PropertyName = "marketBettingTypes")]
        public ISet<MarketBettingType> MarketBettingTypes { get; set; }

        /// <summary>
        /// ���������� �����, ������� ��������� � ��������� ������ ��� �������
        /// </summary>
        [JsonProperty(PropertyName = "marketCountries")]
        public ISet<string> MarketCountries { get; set; }

        /// <summary>
        /// ���������� �����, ������� ������������� ���� ����� (��������, MATCH_ODDS, HALF_TIME_SCORE). �� ������ ������������ ��� ������ ����, ����� ���������� �� �������� �����, ��� ��� ���� ����� ����� ��������� �� ���� �������.
        /// </summary>
        [JsonProperty(PropertyName = "marketTypeCodes")]
        public ISet<string> MarketTypeCodes { get; set; }

        /// <summary>
        /// ���������� ����� � �������� ������ ����� �� ��� ����� ��������� ����
        /// </summary>
        [JsonProperty(PropertyName = "marketStartTime")]
        public TimeRange MarketStartTime { get; set; }

        /// <summary>
        /// ���������� �����, �� ������� � ���� ���� ���� ��� ��������� ������� � ���� �������.
        /// </summary>
        [JsonProperty(PropertyName = "withOrders")]
        public ISet<OrderStatus> WithOrders { get; set; }

        /// <summary>
        /// ����������� �� ���� ���� (��������, ������, ���������, ������, ��������, ������)
        /// </summary>
        [JsonProperty(PropertyName = "raceTypes")]
        public ISet<string> RaceTypes { get; set; }

    }
}
