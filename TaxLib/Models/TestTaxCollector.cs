using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaxLib.Models.Interfaces;

namespace TaxLib.Models
{
    public class TestTaxCollector : ITaxCollector
    {
        public async Task<float> GetTaxRateForLocation(Location location)
        {
            
            return (float)0.1025;
        }

        public async Task<float> PostTaxOnOrder(Order order)
        {

            return (float)1.43;
        }
    }
}
