using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betfair.Models;

namespace Betfair
{
    public interface IClient
    {
        IList<EventTypeResult> listEventTypes(MarketFilter marketFilter, string locale = null);

        IList<CompetitionResult> listCompetitions(MarketFilter marketFilter, string locale = null);

        IList<TimeRangeResult> listTimeRanges(MarketFilter marketFilter, TimeGranularity granularity);

        IList<EventResult> listEvents (MarketFilter marketFilter, string locale = null);

        IList<MarketTypeResult> listMarketTypes(MarketFilter marketFilter, string locale = null);

        IList<CountryCodeResult> listCountries(MarketFilter marketFilter, string locale = null);

        IList<VenueResult> listVenues(MarketFilter marketFilter, string locale = null);

        IList<MarketCatalogue> listMarketCatalogue(MarketFilter marketFilter, ISet<MarketProjection> marketProjections, 
            MarketSort? marketSort=null, string maxResult = "1", string locale = null);

        IList<MarketBook> listMarketBook(IList<string> marketIds, PriceProjection priceProjection, OrderProjection? 
            orderProjection = null, MatchProjection? matchProjection = null, string currencyCode = null, string locale = null);

        IList<MarketBook> listRunnerBook(string marketId, long selectionId,  double? handicap = null, PriceProjection priceProjection = null,
            OrderProjection? orderProjection = null, MatchProjection? matchProjection = null, bool includeOverallPosition = false,
            bool partitionMatchedByStrategyRef = false, List<string> customerStrategyRefs = null, string currencyCode = null, string locale = null,
            DateTime? matchedSince = null);

        IList<MarketProfitAndLoss> listMarketProfitAndLoss(IList<string> marketIds, bool includeSettledBets = false, 
            bool includeBspBets = false, bool netOfCommission = false);

        CurrentOrderSummaryReport listCurrentOrders(ISet<String> betIds, ISet<String> marketIds, OrderProjection? 
            orderProjection = null, TimeRange placedDateRange = null, OrderBy? orderBy = null, SortDir? sortDir = null, int? 
            fromRecord = null, int? recordCount = null);

        ClearedOrderSummaryReport listClearedOrders(BetStatus betStatus, ISet<string> eventTypeIds = null, 
            ISet<string> eventIds = null, ISet<string> marketIds = null, ISet<RunnerId> runnerIds = null, ISet<string> betIds = null, Side? side = null, TimeRange settledDateRange = null, GroupBy? groupBy = null, bool? includeItemDescription = null, String locale = null, int? fromRecord = null, int? recordCount = null);


        PlaceExecutionReport placeOrders(string marketId, string customerRef, IList<PlaceInstruction> placeInstructions, string locale = null);

        CancelExecutionReport cancelOrders(string marketId, IList<CancelInstruction> instructions, string customerRef);

        ReplaceExecutionReport replaceOrders(String marketId, IList<ReplaceInstruction> instructions, String customerRef);

        UpdateExecutionReport updateOrders(String marketId, IList<UpdateInstruction> instructions, String customerRef);
    }
}
