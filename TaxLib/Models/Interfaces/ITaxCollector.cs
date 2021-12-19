using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaxLib.Models.Interfaces
{
    public interface ITaxCollector
    {
        //Since both of these functions in most implementations will be making Webcalls, lets go ahead and make them compatible with Async functions.
        Task<float> GetTaxRateForLocation(Location location);

        Task<float> PostTaxOnOrder(Order order);
    }
}
