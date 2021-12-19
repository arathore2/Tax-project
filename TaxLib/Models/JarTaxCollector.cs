using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using TaxLib.Models.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web;
using System.Linq;
using System.Net.Http.Headers;

namespace TaxLib.Models
{

    //Tax Jar is a Sales Tax calculator that has a readily availble API. You can find the full API reference guide here.
    //https://developers.taxjar.com/api/reference/#sales-tax-api


    public class JarTaxCollector : ITaxCollector
    {
        //I'll probably want to move this to an environment  variable in the future that I intialize the class with, just so its configurable.  But unless the Uri changes, it should be fine. 
        private string JarTaxCollectorUri = "https://api.taxjar.com/v2/";

        //I'm putting Rate inside the Response object, due to how Tax Jar returns objects.
        // This way, I can deseralize the objects far easier.
        class JarJsonTaxRateResponse
        {
            [JsonProperty("rate")]
            public Rate rate;
        }


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
                for(int i = 0; i < myorder.nexusAddresses.Length; i++)
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

        HttpClient client;


        //This ApiKey should be taken from an enviorment variable inside the Project thats actually running this library.

        /// <summary>
        /// This is the implementation of ITaxCollector interface for the TaxJar API.  
        /// </summary>
        /// <param name="ApiKey"></param>
        public JarTaxCollector(string ApiKey)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        }

        /// <summary>
        /// This Function 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        // You can find the API I'm calling for this here
        //https://developers.taxjar.com/api/reference/#post-calculate-sales-tax-for-an-order
        public async Task<float> PostTaxOnOrder(Order order)
        {
            var jarTaxResponse = await PostTaxOnOrderResponse(order);
            return jarTaxResponse.tax.amountToCollect;
        }

        public async Task<JarJsonTaxOrderResponse> PostTaxOnOrderResponse(Order order)
        {
            JarJsonTaxOrderRequest request = new JarJsonTaxOrderRequest(order);
            StringContent parameters = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request),Encoding.UTF8,"application/json");
            var jarTaxResponse = await AsyncTaxJarRequest<JarJsonTaxOrderResponse>(JarTaxCollectorUri + "taxes/", parameters );
            return jarTaxResponse;
        }

        // You can find the API I'm calling for this here
        //https://developers.taxjar.com/api/reference/#rates
        public async Task<float> GetTaxRateForLocation(Location location)
        {
            var JarTaxRate = await GetRateResponse(location);
            return JarTaxRate.rate.combined_rate;
        }

        private async Task<JarJsonTaxRateResponse> GetRateResponse(Location location)
        {
            JarJsonTaxRateRequest request = new JarJsonTaxRateRequest(location);
            string paramters = BuildAddressFromParamaters(request);
            string paramaterizedurl = JarTaxCollectorUri + "rates/" + location.zip + '?' + paramters;
            var JarTaxRate = await AsyncTaxJarRequest<JarJsonTaxRateResponse>(paramaterizedurl, null);
            return JarTaxRate;
        }

        private async Task<T> AsyncTaxJarRequest<T>(string url, StringContent parameters = null)
        {
            HttpResponseMessage response;
            try
            {
                if (parameters == null)
                {
                    response = await client.GetAsync(url);
                }
                else
                {
                    response = await client.PostAsync(url, parameters);
                }
                response.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
            
            string res = await response.Content.ReadAsStringAsync();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(res);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string BuildAddressFromParamaters(object obj)
        {
            var step1 = JsonConvert.SerializeObject(obj);

            var step2 = JsonConvert.DeserializeObject<IDictionary<string, string>>(step1);

            var step3 = step2.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));

            return string.Join("&", step3);
        }
    }
}
