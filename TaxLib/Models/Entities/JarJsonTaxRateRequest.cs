using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaxLib.Models.Entities
{
    class JarJsonTaxRateRequest
    {
        [JsonProperty("country")]
        public string country;

        [JsonProperty("zip")]
        public string zip;

        [JsonProperty("state")]
        public string state;

        [JsonProperty("city")]
        public string city;

        [JsonProperty("street")]
        public string street;

        public JarJsonTaxRateRequest(Location location)
        {
            country = location.country;
            state = location.state;
            city = location.city;
            street = location.street;
        }
    }
}
