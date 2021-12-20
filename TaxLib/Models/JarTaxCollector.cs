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
using TaxLib.Models.Entities;

namespace TaxLib.Models
{

    // Tax Jar is a Sales Tax calculator that has a readily availble API. You can find the full API reference guide here.
    // https://developers.taxjar.com/api/reference/#sales-tax-api


    public class JarTaxCollector : ITaxCollector
    {
        // We'll probably want to move this to a config file in the future, but unless the Uri changes, it should be fine. 
        private string JarTaxCollectorUri = "https://api.taxjar.com/v2/";

        HttpClient client;


        /// <summary>
        /// This is the implementation of ITaxCollector interface for the TaxJar API.  
        /// </summary>
        /// <param name="ApiKey">This API key is provided by Tax Jar.</param>
        public JarTaxCollector(string ApiKey)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        }

        /// <summary>
        /// This function is meant to us to determine how much taxes will need to be collected based on an order.
        /// </summary>
        /// <param name="order">Your Order must include a to_country, and a shipping amount.
        /// If you are in the U.S., you must also include a Zip, and State. 
        /// If you're providing a Nexus address, you must include the Country, and state</param>
        /// <returns>This will return the amount of taxes that will be collected in the order as a float</returns>
        // You can find the API I'm calling for this here
        // https://developers.taxjar.com/api/reference/#post-calculate-sales-tax-for-an-order
        public async Task<float> PostTaxOnOrder(Order order)
        {
            var jarTaxResponse = await PostTaxOnOrderResponse(order);
            return jarTaxResponse.tax.amountToCollect;
        }


        /// <summary>
        /// This function makes a POST web call to the TaxJar API, and pulls the response into an object.
        /// </summary>
        /// <param name="order">Your Order must include a to_country, and a shipping amount.
        /// If you are in the U.S., you must also include a Zip, and State. 
        /// If you're providing a Nexus address, you must include the Country, and state</param>
        /// <returns>The returned Object JarJsonTaxOrderResponse includes every field in the Tax Jar Api's Taxes end point.</returns>
        private async Task<JarJsonTaxOrderResponse> PostTaxOnOrderResponse(Order order)
        {
            JarJsonTaxOrderRequest request = new JarJsonTaxOrderRequest(order);
            StringContent parameters = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request),Encoding.UTF8,"application/json");
            var jarTaxResponse = await AsyncTaxJarRequest<JarJsonTaxOrderResponse>(JarTaxCollectorUri + "taxes/", parameters );
            return jarTaxResponse;
        }

        /// <summary>
        /// This function is meant to provide the Tax service a way to call the Tax Jar Webapi to find the tax rate of a
        /// location.
        /// </summary>
        /// <param name="location">The Zipcode is the only required piece of this function, execpt in the case
        /// where the country is outside the U.S..</param>
        /// <returns>Returns a float repersentation of the tax rate for the requested location</returns>
        // You can find the API I'm calling for this here
        // https://developers.taxjar.com/api/reference/#rates
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


        // We should consider moving this function, and the BuildAddressFromParamaters function into a Utility tool.
        // This project is simply too small to warrant it at the moment.
        // We could also split this function into two, one post, and one get, but the code will look pretty much identical, so it makes sense to keep it contained here.
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
        /// This function is a helper function that can help you pass paramaters into the Uri, due Microsofts HTTP client
        /// having no easy way to do so.
        /// </summary>
        /// <param name="obj">The object in this function must be a flat object.  We cannot handled nested objects,
        /// It is unlikely that any website will ever ask for a nested object.</param>
        /// <returns>The the returned string can be appended to a ? after the base Uri</returns>
        private string BuildAddressFromParamaters(object obj)
        {
            var step1 = JsonConvert.SerializeObject(obj);

            var step2 = JsonConvert.DeserializeObject<IDictionary<string, string>>(step1);

            var step3 = step2.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));

            return string.Join("&", step3);
        }
    }
}
