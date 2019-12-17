using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Betfair.Models
{
    public class DeveloperAppVersion
    {
        [DisplayName("Пользователь")]
        [JsonProperty(PropertyName = "owner")]
        public string Owner { get; set; }

        [Browsable(false)]
        [DisplayName("Версия id")]
        [JsonProperty(PropertyName = "versionId")]
        public long VersionId { get; set; }

        [Browsable(false)]
        [DisplayName("Версия")]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [DisplayName("Ключ")]
        [JsonProperty(PropertyName = "applicationKey")]
        public string ApplicationKey { get; set; }

        [DisplayName("Задержка данных")]
        [JsonProperty(PropertyName = "delayData")]
        public bool DelayData { get; set; }

        [DisplayName("Треб. подписка")]
        [JsonProperty(PropertyName = "subscriptionRequired")]
        public bool SubscriptionRequired { get; set; }

        [Browsable(false)]
        [DisplayName("Управление")]
        [JsonProperty(PropertyName = "ownerManaged")]
        public bool OwnerManaged { get; set; }

        [DisplayName("Использовать")]
        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }

        [Browsable(false)]
        [DisplayName("Индеф. продавца")]
        [JsonProperty(PropertyName = "vendorId")]
        public string VendorId { get; set; }

        [Browsable(false)]
        [DisplayName("Секрет. продавца")]
        [JsonProperty(PropertyName = "vendorSecret")]
        public string VendorSecret { get; set; }
    }
}
