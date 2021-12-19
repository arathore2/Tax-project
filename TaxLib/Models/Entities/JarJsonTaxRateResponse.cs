using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaxLib.Models.Entities
{
    class JarJsonTaxRateResponse
    {
        [JsonProperty("rate")]
        public Rate rate;
    }

    //I'm putting Rate inside the Response object, due to how Tax Jar returns objects.
    // This way, I can deseralize the objects far easier.


    public class Rate
    {
        [JsonProperty("zip")]
        public string zip;

        [JsonProperty("country")]
        public string country;

        [JsonProperty("country_rate")]
        public double country_rate;

        [JsonProperty("state")]
        public string state;

        [JsonProperty("state_rate")]
        public double state_rate;

        [JsonProperty("county")]
        public string county;

        [JsonProperty("county_rate")]
        public double county_rate;

        [JsonProperty("city")]
        public string city;

        [JsonProperty("city_rate")]
        public double city_rate;

        [JsonProperty("combined_distrct_rate")]
        public float combined_district_rate;

        [JsonProperty("combined_rate")]
        public float combined_rate;

        [JsonProperty("freight_taxable")]
        public bool freight_taxable;
    }

}
