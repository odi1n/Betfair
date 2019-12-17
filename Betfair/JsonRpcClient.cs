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
       
        #region Методы для запроссов
        /// <summary>
        /// Метод для отправки запроссов
        /// </summary>
        /// <param name="uri">Ссылка</param>
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

        //Методы для работы с сервисом

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
        /// Отправка запросов с помощью JsonRpcClient
        /// </summary>
        /// <param name="typeApi">Ссылку под какие данные использовать Account - под работу с аккаунтом и тд</param>
        /// <param name="appKey">Ключ пользователя</param>
        /// <param name="sessionToken">Сессия ключ</param>
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
        /// Возвращает список типов событий (например, спорт), связанных с рынками, выбранными в MarketFilter.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="locale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
        /// <returns></returns>
        public IList<EventTypeResult> listEventTypes(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<EventTypeResult>>(LIST_EVENT_TYPES_METHOD, args);
        }

        /// <summary>
        /// Возвращает список соревнований (например, чемпионат мира 2013), связанных с рынками, выбранными в MarketFilter. В настоящее время только футбольные рынки имеют связанную конкуренцию.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="locale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
        /// <returns></returns>
        public IList<CompetitionResult> listCompetitions(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<CompetitionResult>>(LIST_COMPETITIONS, args);
        }

        /// <summary>
        /// Возвращает список временных диапазонов в степени детализации, указанной в запросе (то есть с 15:00 до 16:00, с 14 по 15 августа), связанной с рынками, выбранными в MarketFilter.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="locale">Детальность периодов времени, которые соответствуют рынкам, выбранным фильтром рынка.</param>
        /// <returns></returns>
        public IList<TimeRangeResult> listTimeRanges(MarketFilter marketFilter, TimeGranularity granularity)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.GRANULARITY] = granularity;
            return Invoke<List<TimeRangeResult>>(LIST_TIME_RANGES, args);
        }

        /// <summary>
        /// Возвращает список событий (т. Е. «Чтение против человека»), связанных с рынками, выбранными в MarketFilter.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="locale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
        /// <returns></returns>
        public IList<EventResult> listEvents(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<EventResult>>(LIST_EVENTS, args);
        }

        /// <summary>
        /// Возвращает список типов рынков (например, MATCH_ODDS, NEXT_GOAL), связанных с рынками, выбранными в MarketFilter. Типы рынка всегда одинаковы, независимо от локали.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="stringLocale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
        /// <returns></returns>
        public IList<MarketTypeResult> listMarketTypes(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<MarketTypeResult>>(LIST_MARKET_TYPES_METHOD, args);
        }

        /// <summary>
        /// Возвращает список стран, связанных с рынками, выбранными в MarketFilter.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="stringLocale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
        /// <returns></returns>
        public IList<CountryCodeResult> listCountries(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<CountryCodeResult>>(LIST_COUNTRIES, args);
        }

        /// <summary>
        /// Возвращает список объектов (т.е. Cheltenham, Ascot), связанных с рынками, выбранными в MarketFilter. В настоящее время только место проведения скачек связано с местом проведения.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="stringLocale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
        /// <returns></returns>
        public IList<VenueResult> listVenues(MarketFilter marketFilter, string locale = null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.FILTER] = marketFilter;
            args[Parametrs.LOCALE] = locale;
            return Invoke<List<VenueResult>>(LIST_VENUES, args);
        }

        /// <summary>
        /// Возвращает список информации об опубликованных ( ACTIVE / SUSPENDED ) рынках, который не изменяется (или изменяется очень редко). Вы используете listMarketCatalogue для получения названия рынка, названий выборов и другой информации о рынках.  Лимиты запросов рыночных данных применяются к запросам, направленным в listMarketCatalogue.
        /// </summary>
        /// <param name="marketFilter">Фильтр для выбора желаемых рынков. Все рынки, которые соответствуют критериям в фильтре, выбраны.</param>
        /// <param name="marketProjections">Тип и объем данных, возвращаемых о рынке.</param>
        /// <param name="marketSort">Порядок результатов. По умолчанию будет RANK, если не пройден. RANK - это назначенный приоритет, который определяется нашей командой по рыночным операциям в нашей серверной системе. </param>
        /// <param name="maxResult">Ограничение на общее количество возвращаемых результатов должно быть больше 0 и меньше или равно 1000</param>
        /// <param name="locale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
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
        /// Возвращает список динамических данных о рынках. Динамические данные включают цены, состояние рынка, состояние выбора, объем торгов и состояние любых ордеров, которые вы разместили на рынке.
        /// Пожалуйста, обратите внимание: отдельные запросы должны быть сделаны для открытых и закрытых рынков. Запрос, который включает как открытые, так и закрытые рынки, вернет только те открытые рынки .
        /// </summary>
        /// <param name="marketIds">Один или несколько идентификаторов рынка. Количество возвращаемых рынков зависит от количества данных, которые вы запрашиваете через ценовой прогноз.</param>
        /// <param name="priceProjection">Прогноз ценовых данных, которые вы хотите получить в ответе.</param>
        /// <param name="orderProjection">Заказы, которые вы хотите получить в ответе.</param>
        /// <param name="matchProjection">Если вы запрашиваете заказы, указывается представление матчей.</param>
        /// <param name="currencyCode">Стандартный код валюты Betfair. Если не указан, используется код валюты по умолчанию.</param>
        /// <param name="locale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
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
        /// Возвращает список динамических данных о рынке и указанном бегуне . Динамические данные включают цены, состояние рынка, состояние выбора, объем торгов и состояние любых ордеров, которые вы разместили на рынке.
        /// </summary>
        /// <param name="marketId">Уникальный идентификатор для рынка ..</param>
        /// <param name="selectionId">Уникальный идентификатор для выбора на рынке.</param>
        /// <param name="handicap">Прогноз ценовых данных, которые вы хотите получить в ответе</param>
        /// <param name="priceProjection">Прогноз ценовых данных, которые вы хотите получить в ответе.</param>
        /// <param name="orderProjection">Заказы, которые вы хотите получить в ответе.</param>
        /// <param name="matchProjection">Если вы запрашиваете заказы, указывается представление матчей.</param>
        /// <param name="includeOverallPosition">Если вы запрашиваете заказы, возвращает совпадения для каждого выбора. По умолчанию true, если не указано.</param>
        /// <param name="partitionMatchedByStrategyRef">Если вы запрашиваете заказы, возвращает разбивку матчей по стратегии для каждого выбора. По умолчанию false, если не указано.</param>
        /// <param name="customerStrategyRefs">	Если вы запрашиваете заказы, результаты ограничиваются заказами, соответствующими любой из указанного набора определенных клиентом стратегий. </param>
        /// <param name="currencyCode">Стандартный код валюты Betfair. Если не указан, используется код валюты по умолчанию.</param>
        /// <param name="locale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
        /// <param name="matchedSince">Если вы запрашиваете заказы, результаты ограничиваются заказами, в которых хотя бы один фрагмент сопоставлен с  указанной даты(все совпадающие фрагменты такого заказа будут возвращены, даже если некоторые были сопоставлены до указанной даты). </param>
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
        /// Получить прибыль и убыток для данного списка открытых рынков. Значения рассчитываются с использованием сопоставленных ставок и опционально рассчитанных ставок. Реализуются только рынки с коэффициентами ( MarketBettingType = ODDS), рынки других типов игнорируются.
        /// </summary>
        /// <param name="marketIds">Список рынков для расчета прибылей и убытков</param>
        /// <param name="includeSettledBets">Возможность включить расчетные ставки (только частично расчетные рынки). По умолчанию false, если не указано.</param>
        /// <param name="includeBspBets">Возможность включить ставки BSP. По умолчанию false, если не указано.</param>
        /// <param name="netOfCommission">Возможность возврата прибылей и убытков за вычетом текущих комиссионных ставок пользователей для этого рынка, включая любые специальные тарифы. По умолчанию false, если не указано.</param>
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
        /// Возвращает список ваших текущих заказов. При желании вы можете отфильтровать и отсортировать ваши текущие ордера, используя различные параметры, при установке ни одного из параметров не будут возвращены все ваши текущие ордера максимум до 1000 ставок, упорядочены BY_BET и отсортированы EARLIEST_TO_LATEST. 
        /// </summary>
        /// <param name="betIds">Опционально ограничивает результаты указанными идентификаторами ставок. Допускается максимум 250 ставок или комбинация 250 ставок и marketId.</param>
        /// <param name="marketIds">Опционально ограничивает результаты указанными идентификаторами рынка. Допускается максимум 250 идентификаторов marketId или комбинация 250 идентификаторов marketId и betId .</param>
        /// <param name="orderProjection">Опционально ограничивает результаты указанным статусом заказа.</param>
        /// <param name="placedDateRange">Необязательно ограничивает результаты датами и / или указанной датой, эти даты контекстуальны возвращаемым заказам, и поэтому даты, используемые для фильтрации, будут меняться на даты размещения, сопоставления, аннулирования или погашения в зависимости от orderBy. Эта дата является включающей, то есть, если заказ был размещен именно в эту дату (с точностью до миллисекунды), то он будет включен в результаты. Если значение from позже, чем to, результаты не будут возвращены.</param>
        /// <param name="orderBy">Определяет, как будут упорядочены результаты. Если значение не передается, по умолчанию используется значение BY_BET. Также действует как фильтр, так что будут возвращены только ордера с действительным значением в поле, в котором они упорядочены (т. Е. BY_VOID_TIME возвращает только аннулированные ордера, BY_SETTLED_TIME (применяется к частично рассчитанным рынкам) возвращает только оплаченные ордера, а BY_MATCH_TIME возвращает только ордера с совпадающими ордерами. дата (аннулированные, выполненные, согласованные заказы)).</param>
        /// <param name="sortDir">Определяет направление, в котором будут отсортированы результаты. Если значение не передается, по умолчанию используется значение EARLIEST_TO_LATEST.</param>
        /// <param name="fromRecord">Определяет первую запись, которая будет возвращена. Записи начинаются с нулевого индекса, а не с первого.</param>
        /// <param name="recordCount">Определяет, сколько записей будет возвращено из позиции индекса 'fromRecord'. Обратите внимание, что существует ограничение размера страницы в 1000. Нулевое значение указывает, что вы хотите, чтобы все записи (включая и из 'fromRecord') до предела.</param>
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
        /// Возвращает список разрешенных ставок в зависимости от статуса ставки, упорядоченный по установленной дате. Чтобы получить более 1000 записей, вам необходимо использовать параметры fromRecord и recordCount. По умолчанию служба возвращает все доступные данные за последние 90 дней 
        /// </summary>
        /// <param name="betStatus">Ограничивает результаты указанным статусом.</param>
        /// <param name="eventTypeIds">Необязательно ограничивает результаты указанными идентификаторами типов событий.</param>
        /// <param name="eventIds">Необязательно ограничивает результаты указанными идентификаторами событий.</param>
        /// <param name="marketIds">Опционально ограничивает результаты указанными идентификаторами рынка.</param>
        /// <param name="runnerIds">Опционально ограничивает результаты указанными бегунами.</param>
        /// <param name="betIds">Опционально ограничивает результаты указанными идентификаторами ставок.</param>
        /// <param name="side">Опционально ограничивает результаты указанной стороной.</param>
        /// <param name="settledDateRange">Необязательно ограничивает результаты быть от / до указанной установленной даты. Эта дата является включающей, т. Е. Если заказ был очищен именно в эту дату (с точностью до миллисекунды), то он будет включен в результаты. Если значение from позже, чем to, результаты не будут возвращены. Обратите внимание: это игнорируется при использовании groupBy MARKET</param>
        /// <param name="groupBy">Как агрегировать линии, если не указано иное, возвращается самый низкий уровень, то есть ставка на ставку. Это применимо только к SETTLED BetStatus.</param>
        /// <param name="includeItemDescription">Если true, то объект ItemDescription включается в ответ.</param>
        /// <param name="locale">Язык, используемый для itemDescription. Если не указан, возвращается учетная запись клиента по умолчанию.</param>
        /// <param name="fromRecord">Определяет первую запись, которая будет возвращена. Записи начинаются с нулевого индекса.</param>
        /// <param name="recordCount">Определяет, сколько записей будет возвращено из позиции индекса fromRecord. Обратите внимание, что существует ограничение размера страницы в 1000. Нулевое значение указывает, что вы хотите, чтобы все записи (включая и из 'fromRecord') до предела.</param>
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
        /// Размещать новые заказы на рынке. Обратите внимание, что дополнительные правила определения размера ставок применяются к ставкам, размещенным на Итальянской бирже.
        /// </summary>
        /// <param name="marketId">Рынок идентифицирует эти ордера</param>
        /// <param name="customerRef">Необязательный параметр, позволяющий клиенту передавать уникальную строку (до 32 символов), которая используется для устранения ошибочных повторных представлений. </param>
        /// <param name="placeInstructions">Количество мест инструкции. Максимальное количество инструкций для каждого запроса составляет 200 для Глобальной биржи и 50 для Итальянской биржи.</param>
        /// <param name="locale">Язык, используемый для ответа. Если не указан, возвращается значение по умолчанию.</param>
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
        /// Отмените все ставки ИЛИ отмените все ставки на рынке ИЛИ полностью или частично отмените определенные заказы на рынке. Только ОГРАНИЧЕННЫЕ заказы могут быть отменены или частично отменены после размещения
        /// </summary>
        /// <param name="marketId">Если marketId и betId не указаны, все ставки отменяются. Обратите внимание: одновременные запросы на отмену всех ставок будут отклонены до тех пор, пока не будет завершен первоначальный запрос на отмену всех ставок.</param>
        /// <param name="instructions">Все инструкции должны быть на одном рынке. Если не предоставлено, все несогласованные ставки на рынке (если передан идентификатор рынка) полностью отменяются. Предел отмены инструкций для запроса составляет 60</param>
        /// <param name="customerRef">Необязательный параметр, позволяющий клиенту передавать уникальную строку (до 32 символов), которая используется для устранения ошибочных повторных представлений.</param>
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
        /// Эта операция логически является массовой отменой, за которой следует массовая замена. Отмена завершена сначала, затем новые заказы размещены. Новые ордера будут размещены атомарно в том смысле, что все они будут размещены или ни один не будет размещен. В случае если новые заказы не могут быть размещены, отмены не будут отменены.
        /// </summary>
        /// <param name="marketId">Рынок идентифицирует эти ордера</param>
        /// <param name="instructions">Номер инструкции по замене. Лимит инструкций по замене для каждого запроса составляет 60.</param>
        /// <param name="customerRef">Необязательный параметр, позволяющий клиенту передавать уникальную строку (до 32 символов), которая используется для устранения ошибочных повторных представлений.</param>
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
        /// Обновить поля, не меняющие экспозицию
        /// </summary>
        /// <param name="marketId">Рынок идентифицирует эти ордера</param>
        /// <param name="instructions">Количество инструкций по обновлению. Лимит инструкций по обновлению для каждого запроса составляет 60</param>
        /// <param name="customerRef">Необязательный параметр, позволяющий клиенту передавать уникальную строку (до 32 символов), которая используется для устранения ошибочных повторных представлений.</param>
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
        /// Возвращает информацию о сумме ставок, экспозиции и комиссии.
        /// </summary>
        /// <param name="wallet">Название кошелька под вопросом. Глобальный кошелек возвращается по умолчанию</param>
        /// <returns></returns>
        public AccountFundsResponse getAccountFunds(Wallet? wallet= null)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.WALLET] = wallet;
            return Invoke<AccountFundsResponse>(GET_ACCOUNT_FUNDS_METHOD, args);
        }

        /// <summary>
        /// Возвращает данные, относящиеся к вашей учетной записи, включая вашу ставку дисконтирования и баланс очков Betfair.
        /// </summary>
        /// <returns></returns>
        public AccountDetailsResponse getAccountDetails()
        {
            var args = new Dictionary<string, object>();
            return Invoke<AccountDetailsResponse>(GET_ACCOUNT_DETAILS, args);
        }

        /// <summary>
        /// Получить ключи приложения
        /// </summary>
        /// <returns></returns>
        public IList< DeveloperApp> getDeveloperAppKeys()
        {
            var args = new Dictionary<string, object>();
            return Invoke<IList<DeveloperApp>>(GET_DEVELORER_APPKEYS, args);
        }

        /// <summary>
        /// Создать ключ приложения
        /// </summary>
        /// <param name="appName">Отображаемое имя для приложения</param>
        /// <returns></returns>
        public DeveloperApp createDeveloperAppKeys(string appName)
        {
            var args = new Dictionary<string, object>();
            args[Parametrs.APP_NAME] = appName;
            return Invoke<DeveloperApp>(CREATE_DEVELORER_APPKEYS, args);
        }
    }
}
