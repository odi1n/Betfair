using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Betfair.Models;
using System.Web.Services.Protocols;
using System.Net;
using System.IO;
using Betfair.Json;
using Betfair.Action;
namespace Betfair
{
    public class JsonRpcClient : HttpWebClientProtocol, IClient
    {
        public NameValueCollection CustomHeaders { get; set; }
        public string EndPoint { get; set; }
        private static readonly IDictionary<string, Type> operationReturnTypeMap = new Dictionary<string, Type>();

        private static readonly string LIST_EVENT_TYPES_METHOD = "SportsAPING/v1.0/listEventTypes";
        private static readonly string LIST_COMPETITIONS = "SportsAPING/v1.0/listCompetitions";
        private static readonly string LIST_TIME_RANGES = "SportsAPING/v1.0/listTimeRanges";
        private static readonly string LIST_EVENTS = "SportsAPING/v1.0/listEvents";
        private static readonly string LIST_MARKET_TYPES_METHOD = "SportsAPING/v1.0/listMarketTypes";
        private static readonly string LIST_COUNTRIES = "SportsAPING/v1.0/listCountries";
        private static readonly string LIST_VENUES = "SportsAPING/v1.0/listVenues";
        private static readonly string LIST_MARKET_CATALOGUE_METHOD = "SportsAPING/v1.0/listMarketCatalogue";
        private static readonly string LIST_MARKET_BOOK_METHOD = "SportsAPING/v1.0/listMarketBook";
        private static readonly string LIST_RUNNER_BOOK = "SportsAPING/v1.0/listRunnerBook";
        private static readonly string LIST_MARKET_PROFIT_AND_LOST_METHOD = "SportsAPING/v1.0/listMarketProfitAndLoss";
        private static readonly string LIST_CURRENT_ORDERS_METHOD = "SportsAPING/v1.0/listCurrentOrders";
        private static readonly string LIST_CLEARED_ORDERS_METHOD = "SportsAPING/v1.0/listClearedOrders";
        private static readonly string PLACE_ORDERS_METHOD = "SportsAPING/v1.0/placeOrders";
        private static readonly string CANCEL_ORDERS_METHOD = "SportsAPING/v1.0/cancelOrders";
        private static readonly string REPLACE_ORDERS_METHOD = "SportsAPING/v1.0/replaceOrders";
        private static readonly string UPDATE_ORDERS_METHOD = "SportsAPING/v1.0/updateOrders";

        private static readonly string GET_ACCOUNT_FUNDS_METHOD = "AccountAPING/v1.0/getAccountFunds";
        private static readonly string GET_ACCOUNT_DETAILS = "AccountAPING/v1.0/getAccountDetails";
        private static readonly string GET_DEVELORER_APPKEYS = "AccountAPING/v1.0/getDeveloperAppKeys";
        private static readonly string CREATE_DEVELORER_APPKEYS = "AccountAPING/v1.0/createDeveloperAppKeys";
        private static readonly string GET_ACCOUNT_FUNDS = "AccountAPING/v1.0/getAccountFunds";
       
        #region ������ ��� ���������
        /// <summary>
        /// ����� ��� �������� ���������
        /// </summary>
        /// <param name="uri">������</param>
        /// <returns></returns>
        protected WebRequest CreateWebRequest(Uri uri)
        {
            WebRequest request = WebRequest.Create(new Uri(EndPoint));
            request.Method = "POST";
            request.ContentType = "application/json-rpc";
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");
            request.Headers.Add(CustomHeaders);
            ServicePointManager.Expect100Continue = false;
            return request;
        }

        public T Invoke<T>(string method, IDictionary<string, object> args = null)
        {
            if ( method == null )
                throw new ArgumentNullException("method");
            if ( method.Length == 0 )
                throw new ArgumentException(null, "method");

            var request = CreateWebRequest(new Uri(EndPoint));

            using ( Stream stream = request.GetRequestStream() )
            using ( StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) )
            {
                var call = new JsonRequest { Method = method, Id = 1, Params = args };
                JsonConvert.Export(call, writer);
            }
            Console.WriteLine("\nCalling: " + method + " With args: " + JsonConvert.Serialize<IDictionary<string, object>>(args));

            using ( WebResponse response = GetWebResponse(request) )
            using ( Stream stream = response.GetResponseStream() )
            using ( StreamReader reader = new StreamReader(stream, Encoding.UTF8) )
            {
                var jsonResponse = JsonConvert.Import<T>(reader);
                // Console.WriteLine("\nGot Response: " + JsonConvert.Serialize<JsonResponse<T>>(jsonResponse));
                if ( jsonResponse.HasError )
                {
                    throw ReconstituteException(jsonResponse.Error);
                }
                else
                {
                    return jsonResponse.Result;
                }
            }
        }

        private System.Exception ReconstituteException(Betfair.Models.Exception ex)
        {
            var data = ex.Data;

            // API-NG exception -- it must have "data" element to tell us which exception
            var exceptionName = data.Property("exceptionname").Value.ToString();
            var exceptionData = data.Property(exceptionName).Value.ToString();

            return JsonConvert.Deserialize<APINGException>(exceptionData);
        }
        #endregion

        //������ ��� ������ � ��������

        public JsonRpcClient() { }

        public JsonRpcClient(TypeApi typeApi, string sessionToken)
        {
            CustomHeaders = new NameValueCollection();
            if ( sessionToken != null ) CustomHeaders[Request.SESSION_TOKEN_HEADER] = sessionToken;

            switch ( typeApi )
            {
                case TypeApi.Account: EndPoint = "https://api.betfair.com/exchange/account" + "/json-rpc/v1/"; break;
                case TypeApi.Betting: EndPoint = "https://api.betfair.com/exchange/betting" + "/json-rpc/v1/"; break;
            }
        }

        /// <summary>
        /// �������� �������� � ������� JsonRpcClient
        /// </summary>
        /// <param name="typeApi">������ ��� ����� ������ ������������ Account - ��� ������ � ��������� � ��</param>
        /// <param name="appKey">���� ������������</param>
        /// <param name="sessionToken">������ ����</param>
        public JsonRpcClient(TypeApi typeApi, string appKey, string sessionToken)
        {
            CustomHeaders = new NameValueCollection();
            if ( appKey != null ) CustomHeaders[Request.APPKEY_HEADER] = appKey;

            if ( sessionToken != null ) CustomHeaders[Request.SESSION_TOKEN_HEADER] = sessionToken;

            switch ( typeApi )
            {
                case TypeApi.Account: EndPoint = "https://api.betfair.com/exchange/account" + "/json-rpc/v1/"; break;
                case TypeApi.Betting: EndPoint = "https://api.betfair.com/exchange/betting" + "/json-rpc/v1/"; break;
            }
        }

        /// <summary>
        /// ���������� ������ ����� ������� (��������, �����), ��������� � �������, ���������� � MarketFilter.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="locale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<EventTypeResult> listEventTypes(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<EventTypeResult>>(LIST_EVENT_TYPES_METHOD, args);
        }

        /// <summary>
        /// ���������� ������ ������������ (��������, ��������� ���� 2013), ��������� � �������, ���������� � MarketFilter. � ��������� ����� ������ ���������� ����� ����� ��������� �����������.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="locale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<CompetitionResult> listCompetitions(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<CompetitionResult>>(LIST_COMPETITIONS, args);
        }

        /// <summary>
        /// ���������� ������ ��������� ���������� � ������� �����������, ��������� � ������� (�� ���� � 15:00 �� 16:00, � 14 �� 15 �������), ��������� � �������, ���������� � MarketFilter.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="locale">����������� �������� �������, ������� ������������� ������, ��������� �������� �����.</param>
        /// <returns></returns>
        public IList<TimeRangeResult> listTimeRanges(MarketFilter marketFilter, TimeGranularity granularity)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.GRANULARITY] = granularity;
            return Invoke<List<TimeRangeResult>>(LIST_TIME_RANGES, args);
        }

        /// <summary>
        /// ���������� ������ ������� (�. �. ������� ������ ��������), ��������� � �������, ���������� � MarketFilter.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="locale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<EventResult> listEvents(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<EventResult>>(LIST_EVENTS, args);
        }

        /// <summary>
        /// ���������� ������ ����� ������ (��������, MATCH_ODDS, NEXT_GOAL), ��������� � �������, ���������� � MarketFilter. ���� ����� ������ ���������, ���������� �� ������.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="stringLocale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<MarketTypeResult> listMarketTypes(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<MarketTypeResult>>(LIST_MARKET_TYPES_METHOD, args);
        }

        /// <summary>
        /// ���������� ������ �����, ��������� � �������, ���������� � MarketFilter.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="stringLocale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<CountryCodeResult> listCountries(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<CountryCodeResult>>(LIST_COUNTRIES, args);
        }

        /// <summary>
        /// ���������� ������ �������� (�.�. Cheltenham, Ascot), ��������� � �������, ���������� � MarketFilter. � ��������� ����� ������ ����� ���������� ������ ������� � ������ ����������.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="stringLocale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<VenueResult> listVenues(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<VenueResult>>(LIST_VENUES, args);
        }

        /// <summary>
        /// ���������� ������ ���������� �� �������������� ( ACTIVE / SUSPENDED ) ������, ������� �� ���������� (��� ���������� ����� �����). �� ����������� listMarketCatalogue ��� ��������� �������� �����, �������� ������� � ������ ���������� � ������.  ������ �������� �������� ������ ����������� � ��������, ������������ � listMarketCatalogue.
        /// </summary>
        /// <param name="marketFilter">������ ��� ������ �������� ������. ��� �����, ������� ������������� ��������� � �������, �������.</param>
        /// <param name="marketProjections">��� � ����� ������, ������������ � �����.</param>
        /// <param name="marketSort">������� �����������. �� ��������� ����� RANK, ���� �� �������. RANK - ��� ����������� ���������, ������� ������������ ����� �������� �� �������� ��������� � ����� ��������� �������. </param>
        /// <param name="maxResult">����������� �� ����� ���������� ������������ ����������� ������ ���� ������ 0 � ������ ��� ����� 1000</param>
        /// <param name="locale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<MarketCatalogue> listMarketCatalogue(MarketFilter marketFilter, ISet<MarketProjection> marketProjections = null, MarketSort? marketSort = null, string maxResult = "1", string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.MARKET_PROJECTION] = marketProjections;
            args[Parametrs.SORT] = marketSort;
            args[Parametrs.MAX_RESULTS] = maxResult;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<MarketCatalogue>>(LIST_MARKET_CATALOGUE_METHOD, args);
        }

        /// <summary>
        /// ���������� ������ ������������ ������ � ������. ������������ ������ �������� ����, ��������� �����, ��������� ������, ����� ������ � ��������� ����� �������, ������� �� ���������� �� �����.
        /// ����������, �������� ��������: ��������� ������� ������ ���� ������� ��� �������� � �������� ������. ������, ������� �������� ��� ��������, ��� � �������� �����, ������ ������ �� �������� ����� .
        /// </summary>
        /// <param name="marketIds">���� ��� ��������� ��������������� �����. ���������� ������������ ������ ������� �� ���������� ������, ������� �� ������������ ����� ������� �������.</param>
        /// <param name="priceProjection">������� ������� ������, ������� �� ������ �������� � ������.</param>
        /// <param name="orderProjection">������, ������� �� ������ �������� � ������.</param>
        /// <param name="matchProjection">���� �� ������������ ������, ����������� ������������� ������.</param>
        /// <param name="currencyCode">����������� ��� ������ Betfair. ���� �� ������, ������������ ��� ������ �� ���������.</param>
        /// <param name="locale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public IList<MarketBook> listMarketBook(IList<string> marketIds, PriceProjection priceProjection, OrderProjection? orderProjection = null, MatchProjection? matchProjection = null, string currencyCode = null, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.MARKET_IDS] = marketIds;
            args[Parametrs.PRICE_PROJECTION] = priceProjection;
            args[Parametrs.ORDER_PROJECTION] = orderProjection;
            args[Parametrs.MATCH_PROJECTION] = matchProjection;
            args[Parametrs.LOCALE] = locale;
            args[Parametrs.CURRENCY_CODE] = currencyCode;
            return Invoke<List<MarketBook>>(LIST_MARKET_BOOK_METHOD, args);
        }

        /// <summary>
        /// ���������� ������ ������������ ������ � ����� � ��������� ������ . ������������ ������ �������� ����, ��������� �����, ��������� ������, ����� ������ � ��������� ����� �������, ������� �� ���������� �� �����.
        /// </summary>
        /// <param name="marketId">���������� ������������� ��� ����� ..</param>
        /// <param name="selectionId">���������� ������������� ��� ������ �� �����.</param>
        /// <param name="handicap">������� ������� ������, ������� �� ������ �������� � ������</param>
        /// <param name="priceProjection">������� ������� ������, ������� �� ������ �������� � ������.</param>
        /// <param name="orderProjection">������, ������� �� ������ �������� � ������.</param>
        /// <param name="matchProjection">���� �� ������������ ������, ����������� ������������� ������.</param>
        /// <param name="includeOverallPosition">���� �� ������������ ������, ���������� ���������� ��� ������� ������. �� ��������� true, ���� �� �������.</param>
        /// <param name="partitionMatchedByStrategyRef">���� �� ������������ ������, ���������� �������� ������ �� ��������� ��� ������� ������. �� ��������� false, ���� �� �������.</param>
        /// <param name="customerStrategyRefs">	���� �� ������������ ������, ���������� �������������� ��������, ���������������� ����� �� ���������� ������ ������������ �������� ���������. </param>
        /// <param name="currencyCode">����������� ��� ������ Betfair. ���� �� ������, ������������ ��� ������ �� ���������.</param>
        /// <param name="locale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <param name="matchedSince">���� �� ������������ ������, ���������� �������������� ��������, � ������� ���� �� ���� �������� ����������� �  ��������� ����(��� ����������� ��������� ������ ������ ����� ����������, ���� ���� ��������� ���� ������������ �� ��������� ����). </param>
        /// <returns></returns>
        public IList<MarketBook> listRunnerBook(string marketId, long selectionId, double? handicap = null, PriceProjection priceProjection = null,OrderProjection? orderProjection = null, MatchProjection? matchProjection = null, bool includeOverallPosition = false,bool partitionMatchedByStrategyRef = false, List<string> customerStrategyRefs = null, string currencyCode = null, string locale = null,DateTime? matchedSince = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.MARKET_ID] = marketId;
            args[Parametrs.SELECTION_ID] = selectionId;
            args[Parametrs.HANDICAP] = handicap;
            args[Parametrs.PRICE_PROJECTION] = priceProjection;
            args[Parametrs.ORDER_PROJECTION] = orderProjection;
            args[Parametrs.MARKET_PROJECTION] = matchProjection;
            args[Parametrs.INCLUDE_OVERALL_POSITION] = includeOverallPosition;
            args[Parametrs.PARTITION_MATCHED_BY_STRATEGY_REF] = partitionMatchedByStrategyRef;
            args[Parametrs.CUSTOMER_STRATEGY_REFS] = customerStrategyRefs;
            args[Parametrs.CURRENCY_CODE] = currencyCode;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<MarketBook>>(LIST_RUNNER_BOOK, args);
        }

        /// <summary>
        /// �������� ������� � ������ ��� ������� ������ �������� ������. �������� �������������� � �������������� �������������� ������ � ����������� ������������ ������. ����������� ������ ����� � �������������� ( MarketBettingType = ODDS), ����� ������ ����� ������������.
        /// </summary>
        /// <param name="marketIds">������ ������ ��� ������� �������� � �������</param>
        /// <param name="includeSettledBets">����������� �������� ��������� ������ (������ �������� ��������� �����). �� ��������� false, ���� �� �������.</param>
        /// <param name="includeBspBets">����������� �������� ������ BSP. �� ��������� false, ���� �� �������.</param>
        /// <param name="netOfCommission">����������� �������� �������� � ������� �� ������� ������� ������������ ������ ������������� ��� ����� �����, ������� ����� ����������� ������. �� ��������� false, ���� �� �������.</param>
        /// <returns></returns>
        public IList<MarketProfitAndLoss> listMarketProfitAndLoss(IList<string> marketIds, bool includeSettledBets = false, bool includeBspBets = false, bool netOfCommission = false)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.MARKET_IDS] = marketIds;
            args[Parametrs.INCLUDE_SETTLED_BETS] = includeSettledBets;
            args[Parametrs.INCLUDE_BSP_BETS] = includeBspBets;
            args[Parametrs.NET_OF_COMMISSION] = netOfCommission;
            return Invoke<List<MarketProfitAndLoss>>(LIST_MARKET_PROFIT_AND_LOST_METHOD, args);
        }

        /// <summary>
        /// ���������� ������ ����� ������� �������. ��� ������� �� ������ ������������� � ������������� ���� ������� ������, ��������� ��������� ���������, ��� ��������� �� ������ �� ���������� �� ����� ���������� ��� ���� ������� ������ �������� �� 1000 ������, ����������� BY_BET � ������������� EARLIEST_TO_LATEST. 
        /// </summary>
        /// <param name="betIds">����������� ������������ ���������� ���������� ���������������� ������. ����������� �������� 250 ������ ��� ���������� 250 ������ � marketId.</param>
        /// <param name="marketIds">����������� ������������ ���������� ���������� ���������������� �����. ����������� �������� 250 ��������������� marketId ��� ���������� 250 ��������������� marketId � betId .</param>
        /// <param name="orderProjection">����������� ������������ ���������� ��������� �������� ������.</param>
        /// <param name="placedDateRange">������������� ������������ ���������� ������ � / ��� ��������� �����, ��� ���� �������������� ������������ �������, � ������� ����, ������������ ��� ����������, ����� �������� �� ���� ����������, �������������, ������������� ��� ��������� � ����������� �� orderBy. ��� ���� �������� ����������, �� ����, ���� ����� ��� �������� ������ � ��� ���� (� ��������� �� ������������), �� �� ����� ������� � ����������. ���� �������� from �����, ��� to, ���������� �� ����� ����������.</param>
        /// <param name="orderBy">����������, ��� ����� ����������� ����������. ���� �������� �� ����������, �� ��������� ������������ �������� BY_BET. ����� ��������� ��� ������, ��� ��� ����� ���������� ������ ������ � �������������� ��������� � ����, � ������� ��� ����������� (�. �. BY_VOID_TIME ���������� ������ �������������� ������, BY_SETTLED_TIME (����������� � �������� ������������ ������) ���������� ������ ���������� ������, � BY_MATCH_TIME ���������� ������ ������ � ������������ ��������. ���� (��������������, �����������, ������������� ������)).</param>
        /// <param name="sortDir">���������� �����������, � ������� ����� ������������� ����������. ���� �������� �� ����������, �� ��������� ������������ �������� EARLIEST_TO_LATEST.</param>
        /// <param name="fromRecord">���������� ������ ������, ������� ����� ����������. ������ ���������� � �������� �������, � �� � �������.</param>
        /// <param name="recordCount">����������, ������� ������� ����� ���������� �� ������� ������� 'fromRecord'. �������� ��������, ��� ���������� ����������� ������� �������� � 1000. ������� �������� ���������, ��� �� ������, ����� ��� ������ (������� � �� 'fromRecord') �� �������.</param>
        /// <returns></returns>
        public CurrentOrderSummaryReport listCurrentOrders(ISet<String> betIds, ISet<String> marketIds, OrderProjection? orderProjection = null, TimeRange placedDateRange = null, OrderBy? orderBy = null, SortDir? sortDir = null, int? fromRecord = null, int? recordCount = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.BET_IDS] = betIds;
            args[Parametrs.MARKET_IDS] = marketIds;
            args[Parametrs.ORDER_PROJECTION] = orderProjection;
            args[Parametrs.PLACED_DATE_RANGE] = placedDateRange;
            args[Parametrs.ORDER_BY] = orderBy;
            args[Parametrs.SORT_DIR] = sortDir;
            args[Parametrs.FROM_RECORD] = fromRecord;
            args[Parametrs.RECORD_COUNT] = recordCount;

            return Invoke<CurrentOrderSummaryReport>(LIST_CURRENT_ORDERS_METHOD, args);
        }

        /// <summary>
        /// ���������� ������ ����������� ������ � ����������� �� ������� ������, ������������� �� ������������� ����. ����� �������� ����� 1000 �������, ��� ���������� ������������ ��������� fromRecord � recordCount. �� ��������� ������ ���������� ��� ��������� ������ �� ��������� 90 ���� 
        /// </summary>
        /// <param name="betStatus">������������ ���������� ��������� ��������.</param>
        /// <param name="eventTypeIds">������������� ������������ ���������� ���������� ���������������� ����� �������.</param>
        /// <param name="eventIds">������������� ������������ ���������� ���������� ���������������� �������.</param>
        /// <param name="marketIds">����������� ������������ ���������� ���������� ���������������� �����.</param>
        /// <param name="runnerIds">����������� ������������ ���������� ���������� ��������.</param>
        /// <param name="betIds">����������� ������������ ���������� ���������� ���������������� ������.</param>
        /// <param name="side">����������� ������������ ���������� ��������� ��������.</param>
        /// <param name="settledDateRange">������������� ������������ ���������� ���� �� / �� ��������� ������������� ����. ��� ���� �������� ����������, �. �. ���� ����� ��� ������ ������ � ��� ���� (� ��������� �� ������������), �� �� ����� ������� � ����������. ���� �������� from �����, ��� to, ���������� �� ����� ����������. �������� ��������: ��� ������������ ��� ������������� groupBy MARKET</param>
        /// <param name="groupBy">��� ������������ �����, ���� �� ������� ����, ������������ ����� ������ �������, �� ���� ������ �� ������. ��� ��������� ������ � SETTLED BetStatus.</param>
        /// <param name="includeItemDescription">���� true, �� ������ ItemDescription ���������� � �����.</param>
        /// <param name="locale">����, ������������ ��� itemDescription. ���� �� ������, ������������ ������� ������ ������� �� ���������.</param>
        /// <param name="fromRecord">���������� ������ ������, ������� ����� ����������. ������ ���������� � �������� �������.</param>
        /// <param name="recordCount">����������, ������� ������� ����� ���������� �� ������� ������� fromRecord. �������� ��������, ��� ���������� ����������� ������� �������� � 1000. ������� �������� ���������, ��� �� ������, ����� ��� ������ (������� � �� 'fromRecord') �� �������.</param>
        /// <returns></returns>
        public ClearedOrderSummaryReport listClearedOrders(BetStatus betStatus, ISet<string> eventTypeIds = null, ISet<string> eventIds = null, ISet<string> marketIds = null, ISet<RunnerId> runnerIds = null, ISet<string> betIds = null, Side? side = null, TimeRange settledDateRange = null, GroupBy? groupBy = null, bool? includeItemDescription = null, String locale = null, int? fromRecord = null, int? recordCount = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.BET_STATUS] = betStatus;
            args[Parametrs.EVENT_TYPE_IDS] = eventTypeIds;
            args[Parametrs.EVENT_IDS] = eventIds;
            args[Parametrs.MARKET_IDS] = marketIds;
            args[Parametrs.RUNNER_IDS] = runnerIds;
            args[Parametrs.BET_IDS] = betIds;
            args[Parametrs.SIDE] = side;
            args[Parametrs.SETTLED_DATE_RANGE] = settledDateRange;
            args[Parametrs.GROUP_BY] = groupBy;
            args[Parametrs.INCLUDE_ITEM_DESCRIPTION] = includeItemDescription;
            args[Parametrs.LOCALE] = locale;
            args[Parametrs.FROM_RECORD] = fromRecord;
            args[Parametrs.RECORD_COUNT] = recordCount;

            return Invoke<ClearedOrderSummaryReport>(LIST_CLEARED_ORDERS_METHOD, args);
        }

        /// <summary>
        /// ��������� ����� ������ �� �����. �������� ��������, ��� �������������� ������� ����������� ������� ������ ����������� � �������, ����������� �� ����������� �����.
        /// </summary>
        /// <param name="marketId">����� �������������� ��� ������</param>
        /// <param name="customerRef">�������������� ��������, ����������� ������� ���������� ���������� ������ (�� 32 ��������), ������� ������������ ��� ���������� ��������� ��������� �������������. </param>
        /// <param name="placeInstructions">���������� ���� ����������. ������������ ���������� ���������� ��� ������� ������� ���������� 200 ��� ���������� ����� � 50 ��� ����������� �����.</param>
        /// <param name="locale">����, ������������ ��� ������. ���� �� ������, ������������ �������� �� ���������.</param>
        /// <returns></returns>
        public PlaceExecutionReport placeOrders(string marketId, string customerRef, IList<PlaceInstruction> placeInstructions, string locale = null)
        {
            var args = new Dictionary<string, object>();

            args[Parametrs.MARKET_ID] = marketId;
            args[Parametrs.INSTRUCTIONS] = placeInstructions;
            args[Parametrs.CUSTOMER_REFERENCE] = customerRef;
            args[Parametrs.LOCALE] = locale;

            return Invoke<PlaceExecutionReport>(PLACE_ORDERS_METHOD, args);
        }

        /// <summary>
        /// �������� ��� ������ ��� �������� ��� ������ �� ����� ��� ��������� ��� �������� �������� ������������ ������ �� �����. ������ ������������ ������ ����� ���� �������� ��� �������� �������� ����� ����������
        /// </summary>
        /// <param name="marketId">���� marketId � betId �� �������, ��� ������ ����������. �������� ��������: ������������� ������� �� ������ ���� ������ ����� ��������� �� ��� ���, ���� �� ����� �������� �������������� ������ �� ������ ���� ������.</param>
        /// <param name="instructions">��� ���������� ������ ���� �� ����� �����. ���� �� �������������, ��� ��������������� ������ �� ����� (���� ������� ������������� �����) ��������� ����������. ������ ������ ���������� ��� ������� ���������� 60</param>
        /// <param name="customerRef">�������������� ��������, ����������� ������� ���������� ���������� ������ (�� 32 ��������), ������� ������������ ��� ���������� ��������� ��������� �������������.</param>
        /// <returns></returns>
        public CancelExecutionReport cancelOrders(string marketId, IList<CancelInstruction> instructions, string customerRef)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.MARKET_ID] = marketId;
            args[Parametrs.INSTRUCTIONS] = instructions;
            args[Parametrs.CUSTOMER_REFERENCE] = customerRef;

            return Invoke<CancelExecutionReport>(CANCEL_ORDERS_METHOD, args);
        }

        /// <summary>
        /// ��� �������� ��������� �������� �������� �������, �� ������� ������� �������� ������. ������ ��������� �������, ����� ����� ������ ���������. ����� ������ ����� ��������� �������� � ��� ������, ��� ��� ��� ����� ��������� ��� �� ���� �� ����� ��������. � ������ ���� ����� ������ �� ����� ���� ���������, ������ �� ����� ��������.
        /// </summary>
        /// <param name="marketId">����� �������������� ��� ������</param>
        /// <param name="instructions">����� ���������� �� ������. ����� ���������� �� ������ ��� ������� ������� ���������� 60.</param>
        /// <param name="customerRef">�������������� ��������, ����������� ������� ���������� ���������� ������ (�� 32 ��������), ������� ������������ ��� ���������� ��������� ��������� �������������.</param>
        /// <returns></returns>
        public ReplaceExecutionReport replaceOrders(String marketId, IList<ReplaceInstruction> instructions, String customerRef)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.MARKET_ID] = marketId;
            args[Parametrs.INSTRUCTIONS] = instructions;
            args[Parametrs.CUSTOMER_REFERENCE] = customerRef;

            return Invoke<ReplaceExecutionReport>(REPLACE_ORDERS_METHOD, args);
        }

        /// <summary>
        /// �������� ����, �� �������� ����������
        /// </summary>
        /// <param name="marketId">����� �������������� ��� ������</param>
        /// <param name="instructions">���������� ���������� �� ����������. ����� ���������� �� ���������� ��� ������� ������� ���������� 60</param>
        /// <param name="customerRef">�������������� ��������, ����������� ������� ���������� ���������� ������ (�� 32 ��������), ������� ������������ ��� ���������� ��������� ��������� �������������.</param>
        /// <returns></returns>
        public UpdateExecutionReport updateOrders(String marketId, IList<UpdateInstruction> instructions, String customerRef)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.MARKET_ID] = marketId;
            args[Parametrs.INSTRUCTIONS] = instructions;
            args[Parametrs.CUSTOMER_REFERENCE] = customerRef;

            return Invoke<UpdateExecutionReport>(UPDATE_ORDERS_METHOD, args);
        }

        /// <summary>
        /// ���������� ���������� � ����� ������, ���������� � ��������.
        /// </summary>
        /// <param name="wallet">�������� �������� ��� ��������. ���������� ������� ������������ �� ���������</param>
        /// <returns></returns>
        public AccountFundsResponse getAccountFunds(Wallet? wallet= null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.WALLET] = wallet;
            return Invoke<AccountFundsResponse>(GET_ACCOUNT_FUNDS_METHOD, args);
        }

        /// <summary>
        /// ���������� ������, ����������� � ����� ������� ������, ������� ���� ������ ��������������� � ������ ����� Betfair.
        /// </summary>
        /// <returns></returns>
        public AccountDetailsResponse getAccountDetails()
        {
            var args = new Dictionary<string, object>();
            return Invoke<AccountDetailsResponse>(GET_ACCOUNT_DETAILS, args);
        }

        /// <summary>
        /// �������� ����� ����������
        /// </summary>
        /// <returns></returns>
        public IList< DeveloperApp> getDeveloperAppKeys()
        {
            var args = new Dictionary<string, object>();
            return Invoke<IList<DeveloperApp>>(GET_DEVELORER_APPKEYS, args);
        }

        /// <summary>
        /// ������� ���� ����������
        /// </summary>
        /// <param name="appName">������������ ��� ��� ����������</param>
        /// <returns></returns>
        public DeveloperApp createDeveloperAppKeys(string appName)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.APP_NAME] = appName;
            return Invoke<DeveloperApp>(CREATE_DEVELORER_APPKEYS, args);
        }
    }
}
