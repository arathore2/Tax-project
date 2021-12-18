using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using TaxLib.Models.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;

namespace TaxLib.Models
{

    //Tax Jar is a Sales Tax calculator that has a readily availble API. You can find the full API reference guide here.
    //https://developers.taxjar.com/api/reference/#sales-tax-api


    public class JarTaxCollector : ITaxCollector
    {
        //I'll probably want to move this to an enviorment variable in the future that I intialize the class with, just so its configurable.  But unless the Uri changes, it should be fine. 
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
            public string zip { get; set; }

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

        HttpClient client;


        //This ApiKey should be taken from an enviorment variable inside the Project thats actually running this library.
        JarTaxCollector(string ApiKey)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);
        }

        // You can find the API I'm calling for this here
        //https://developers.taxjar.com/api/reference/#post-calculate-sales-tax-for-an-order
        public async Task<float> GetTaxOnOrder(Order order)
        {
            throw new NotImplementedException();
        }

        // You can find the API I'm calling for this here
        //https://developers.taxjar.com/api/reference/#rates
        public async Task<float> GetTaxRateForLocation(Location location)
        {
            JarJsonTaxRateRequest request = new JarJsonTaxRateRequest(location);
            StringContent parameters = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request));
            var JarTaxRate = await AsyncTaxJarGetRequest<JarJsonTaxRateResponse>(JarTaxCollectorUri + "/rates/" + location.zip, parameters);
            return JarTaxRate.rate.combined_rate;
        }

        private async Task<T> AsyncTaxJarGetRequest<T>(string url, StringContent parameters = null)
        {
            HttpResponseMessage response;
            if (parameters == null)
                response = await client.GetAsync(url);
            else 
                response = await client.PostAsync(url, parameters);

            string res = await response.Content.ReadAsStringAsync();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(res);
            return result;
        }
    }
}
