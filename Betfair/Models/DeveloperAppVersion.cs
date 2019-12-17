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
        [DisplayName("������������")]
        [JsonProperty(PropertyName = "owner")]
        public string Owner { get; set; }

        [Browsable(false)]
        [DisplayName("������ id")]
        [JsonProperty(PropertyName = "versionId")]
        public long VersionId { get; set; }

        [Browsable(false)]
        [DisplayName("������")]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [DisplayName("����")]
        [JsonProperty(PropertyName = "applicationKey")]
        public string ApplicationKey { get; set; }

        [DisplayName("�������� ������")]
        [JsonProperty(PropertyName = "delayData")]
        public bool DelayData { get; set; }

        [DisplayName("����. ��������")]
        [JsonProperty(PropertyName = "subscriptionRequired")]
        public bool SubscriptionRequired { get; set; }

        [Browsable(false)]
        [DisplayName("����������")]
        [JsonProperty(PropertyName = "ownerManaged")]
        public bool OwnerManaged { get; set; }

        [DisplayName("������������")]
        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }

        [Browsable(false)]
        [DisplayName("�����. ��������")]
        [JsonProperty(PropertyName = "vendorId")]
        public string VendorId { get; set; }

        [Browsable(false)]
        [DisplayName("������. ��������")]
        [JsonProperty(PropertyName = "vendorSecret")]
        public string VendorSecret { get; set; }
    }
}
