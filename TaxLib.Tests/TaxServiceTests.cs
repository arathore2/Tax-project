using System;
using System.Collections.Generic;
using System.Text;
using TaxLib.Models;
using System.Runtime;
using TaxLib.Models.Interfaces;
using TaxLib;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace TaxLib.Tests
{
    public class TaxServiceTests
    {

        IConfigurationRoot config;

        public TaxServiceTests()
        {
            config = new ConfigurationBuilder().AddUserSecrets<TaxLib.Tests.TaxServiceTests>().Build();

        }

        [Fact]
        public void CanInitalizeTaxService()
        {
            //Arrange
            JarTaxCollector jTax = new JarTaxCollector(Environment.GetEnvironmentVariable("JarApiString"));
            //Act
            TaxService tax = new TaxService(jTax);
            //Assert
            Assert.NotNull(tax);
            Assert.NotNull(jTax);
        }

        [Fact]
        public async void CanGetTaxRateForLA()
        {
            //Arrange
            JarTaxCollector jarTax = new JarTaxCollector(Environment.GetEnvironmentVariable("JarApiString"));
            TaxService tax = new TaxService(jarTax);

            Location location = new Location
            {
                city = "Santa%20Monica",
                state = "CA",
                country = "US",
                zip = "90404"
            };
            //Act
            double calculatedTaxrate = await tax.GetTaxRateForLocation(location);
            //Assert
            Assert.Equal((double)0.0975, calculatedTaxrate);

        }

        [Fact]
        public async void CanCheckTaxForOrderInLA()
        {
            //Arange
            JarTaxCollector jarTax = new JarTaxCollector(Environment.GetEnvironmentVariable("JarApiString"));
            TaxService tax = new TaxService(jarTax);

            Location from_location = new Location()
            {
                country = "US",
                zip = "92093",
                state = "CA",
                city = "La Jolla",
                street = "9500 Gilman Drive"
            };
            Location to_location = new Location()
            {
                country = "US",
                zip = "90002",
                state = "CA",
                city = "Los Angeles",
                street = "1335 E 103rd St"
            };
            Order order = new Order
            {
                from_location = from_location,
                to_location = to_location,
                amount = 15,
                shipping = (float)1.5,
                nexusAddresses = new Nexus[] { new Nexus() { id = "Main Location", country = "U.S.", city = "La Jolla", state = "CA", zip = "92093", street = "9500 Gilman Drive" } },
                lineItems = new LineItem[] {new LineItem() { id = "1", product_tax_code = "20010", unit_price = 15, quantity = 1, discount = 0} }
            };
            //Act
            float result = await tax.PostTaxOnOrder(order);
            //Assert
            Assert.Equal(1.35, result);
        }
    }
}
