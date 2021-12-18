using System;
using System.Collections.Generic;
using System.Text;
using TaxLib.Models;
using TaxLib.Models.Interfaces;
using TaxLib;
using Xunit;

namespace TaxLib.Tests
{
    public class TaxServiceTests
    {
        [Fact]
        public void CanInitalizeTaxService()
        {
            //Arrange
            JarTaxCollector jTax = new JarTaxCollector();
            //Act
            TaxService tax = new TaxService(jTax);
            //Assert
            Assert.NotNull(tax);
            Assert.NotNull(jTax);
        }
    }
}
