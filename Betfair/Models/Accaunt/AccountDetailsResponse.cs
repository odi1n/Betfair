using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Betfair.Models
{
    public class AccountDetailsResponse
    {
        [JsonProperty(PropertyName = "currencyCode")]
        public string ÑurrencyCode { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "localeCode")]
        public string LocaleCode { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }
        
        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }

        [JsonProperty(PropertyName = "discountRate")]
        public double DiscountRate { get; set; }

        [JsonProperty(PropertyName = "pointsBalance")]
        public int PointsBalance { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return new StringBuilder().AppendFormat("{0}", "AccountDetailsResponse")
                        .AppendFormat(" : ÑurrencyCode={0}", ÑurrencyCode)
                        .AppendFormat(" : FirstName={0}", FirstName)
                        .AppendFormat(" : LastName={0}", LastName)
                        .AppendFormat(" : LocaleCode={0}", LocaleCode)
                        .AppendFormat(" : Region={0}", Region)
                        .AppendFormat(" : Timezone={0}", Timezone)
                        .AppendFormat(" : DiscountRate={0}", DiscountRate)
                        .AppendFormat(" : PointsBalance={0}", PointsBalance)
                        .AppendFormat(" : CountryCode={0}", CountryCode)
                        .ToString();
        }
    }
}
