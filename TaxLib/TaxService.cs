using System;
using System.Collections.Generic;
using System.Text;
using TaxLib.Models.Interfaces;

namespace TaxLib
{


    public struct Order
    {
        Location from_location; 
        Location to_location;
        float amount;
        float shipping;

    }

    public struct Location
    {
        public string country; 
        public string zip; 
        public string state;
        public string city;
        public string street;
    }
    public class TaxService
    {
        ITaxCollector m_myTaxCollector;
        public TaxService(ITaxCollector taxCollector)
        {
            m_myTaxCollector = taxCollector;
        }

        public float GetTaxRateForLocation(Location location)
        {
            return m_myTaxCollector.GetTaxRateForLocation(location);
        }

        public float GetTaxOnOrder(Order order)
        {
            return m_myTaxCollector.GetTaxOnOrder(order);
        }
    }
}
