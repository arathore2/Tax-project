using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaxLib.Models.Entities
{
    public struct JarJsonTaxOrderRequest
    {
        [JsonProperty("from_country")]
        public string fromCountry;

        [JsonProperty("from_zip")]
        public string fromZip;

        [JsonProperty("from_state")]
        public string fromState;

        [JsonProperty("from_city")]
        public string fromCity;

        [JsonProperty("from_street")]
        public string fromStreet;

        [JsonProperty("to_country")]
        public string toCountry;

        [JsonProperty("to_zip")]
        public string toZip;

        [JsonProperty("to_state")]
        public string toState;

        [JsonProperty("to_city")]
        public string toCity;

        [JsonProperty("to_street")]
        public string toStreet;

        [JsonProperty("amount")]
        public float amount;

        [JsonProperty("shipping")]
        public float shipping;

        [JsonProperty("customer_id")]
        public string customerId;

        [JsonProperty("exemption_type")]
        public string exemptionType;

        [JsonProperty("nexus_addresses")]
        public Nexus[] nexusAddresses;

        [JsonProperty("line_items")]
        public LineItem[] lineItems;

        public JarJsonTaxOrderRequest(Order myorder)
        {
            fromCountry = myorder.from_location.country;
            fromCity = myorder.from_location.city;
            fromState = myorder.from_location.state;
            fromStreet = myorder.from_location.street;
            fromZip = myorder.from_location.zip;
            toCountry = myorder.to_location.country;
            toCity = myorder.to_location.city;
            toState = myorder.to_location.state;
            toStreet = myorder.to_location.street;
            toZip = myorder.to_location.zip;
            amount = myorder.amount;
            shipping = myorder.shipping;
            customerId = myorder.customer_id;
            exemptionType = myorder.exemption_type;
            lineItems = myorder.lineItems;

            nexusAddresses = new Nexus[myorder.nexusAddresses.Length];
            for (int i = 0; i < myorder.nexusAddresses.Length; i++)
            {
                nexusAddresses[i].id = myorder.nexusAddresses[i].id;
                nexusAddresses[i].state = myorder.nexusAddresses[i].state;
                nexusAddresses[i].city = myorder.nexusAddresses[i].city;
                nexusAddresses[i].country = myorder.nexusAddresses[i].country;
                nexusAddresses[i].street = myorder.nexusAddresses[i].street;
                nexusAddresses[i].zip = myorder.nexusAddresses[i].zip;
            }
        }
    }

    public struct Nexus
    {
        [JsonProperty("id")]
        public string id;

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
    }
}
