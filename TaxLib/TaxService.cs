using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaxLib.Models.Interfaces;

namespace TaxLib
{


    public struct Nexus
    {
        public string id;
        public string country;
        public string zip;
        public string state;
        public string city;
        public string street;
    }

    public struct Order
    {
        public Location from_location;
        public Location to_location;
        public float amount;
        public float shipping;
        public string customer_id;
        public string exemption_type;
        public Nexus[] nexusAddresses;
        public LineItem[] lineItems;

    }

    public struct LineItem
    {
        public string id;
        public int quantity;
        public string product_tax_code;
        public float unit_price;
        public float discount;
    }

    public struct Location
    {
        public string country; 
        public string zip; 
        public string state;
        public string city;
        public string street;
    }

    /// <summary>
    /// The TaxService provides a way to find tax information based on given inputs.
    /// The Tax Service must be initalized using a Tax Collector that implements the ITaxCollector interface.  
    /// </summary>
    public class TaxService
    {
        ITaxCollector m_myTaxCollector;
        public TaxService(ITaxCollector taxCollector)
        {
            m_myTaxCollector = taxCollector;
        }

        public Task<float> GetTaxRateForLocation(Location location)
        {
            return m_myTaxCollector.GetTaxRateForLocation(location);
        }

        public Task<float> PostTaxOnOrder(Order order)
        {
            return m_myTaxCollector.PostTaxOnOrder(order);
        }
    }
}
