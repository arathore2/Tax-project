using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaxLib.Models.Entities
{
    public class JarJsonTaxOrderResponse
    {
        [JsonProperty("tax")]
        public Tax tax;
    }

    public struct Tax
    {
        [JsonProperty("order_total_amount")]
        public float orderTotalAmount;

        [JsonProperty("shipping")]
        public float shipping;

        [JsonProperty("taxable_amount")]
        public float taxableAmount;

        [JsonProperty("amount_to_collect")]
        public float amountToCollect;

        [JsonProperty("rate")]
        public float rate;

        [JsonProperty("has_nexus")]
        public bool hasNexus;

        [JsonProperty("freight_taxable")]
        public bool freightTaxable;

        [JsonProperty("jurisdictions")]
        public Jurisdiction jurisdictions;

        [JsonProperty("breakdown")]
        public Breakdown breakdown;
    }

    public struct Jurisdiction
    {
        [JsonProperty("country")]
        public string country;

        [JsonProperty("state")]
        public string state;

        [JsonProperty("county")]
        public string county;

        [JsonProperty("city")]
        public string city;
    }

    public struct Breakdown
    {
        [JsonProperty("taxable_amount")]
        public float taxableAmount;
        [JsonProperty("tax_collectable")]
        public float taxCollectable;
        [JsonProperty("combined_tax_rate")]
        public float combinedTaxrate;
        [JsonProperty("state_taxable_amount")]
        public float stateTaxableAmount;
        [JsonProperty("state_tax_rate")]
        public float stateTaxRate;
        [JsonProperty("state_tax_collectable")]
        public float stateTaxCollectable;
        [JsonProperty("city_taxable_amount")]
        public float cityTaxableAmount;
        [JsonProperty("city_tax_rate")]
        public float cityTaxRate;
        [JsonProperty("special_district_tax_collectable")]
        public float specialDistrictTaxCollectable;
        [JsonProperty("shipping")]
        public float shipping;
        [JsonProperty("line_items")]
        public LineItem[] lineItems;
    }

}
