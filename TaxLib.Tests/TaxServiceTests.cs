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



        [Fact]
        public void CanInitalizeJarTaxService()
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
        public async void CanGetTestTaxRate()
        {
            //Arrange
            TestTaxCollector testTax = new TestTaxCollector();
            TaxService service = new TaxService(testTax);
            Location location = new Location
            {
                city = "Santa%20Monica",
                state = "CA",
                country = "US",
                zip = "90404"
            };
            //Act
            double calculatedTaxrate = await service.GetTaxRateForLocation(location);
            //Assert
            Assert.Equal(0.00, Math.Round(calculatedTaxrate,2));
        }

        [Fact]
        public async void CanGetTestTaxOrder()
        {
            //Arrange
            TestTaxCollector testTax = new TestTaxCollector();
            TaxService service = new TaxService(testTax);
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
                lineItems = new LineItem[] { new LineItem() { id = "1", product_tax_code = "20010", unit_price = 15, quantity = 1, discount = 0 } }
            };
            //Act
            double calculatedTaxAmount = await service.PostTaxOnOrder(order);
            //Assert
            Assert.Equal(10.0, Math.Round(calculatedTaxAmount, 4));
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
            Assert.Equal(0.1025, Math.Round(calculatedTaxrate,4));
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
            double result = await tax.PostTaxOnOrder(order);
            //Assert
            Assert.Equal(1.43, Math.Round(result,2));
        }
    }
}
