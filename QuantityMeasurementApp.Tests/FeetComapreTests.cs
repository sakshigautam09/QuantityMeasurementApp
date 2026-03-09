using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Services;
using QuantityMeasurementApp.Core.Interfaces;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class FeetComparerTests
    {
        private IFeetComparer _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _service = new FeetComparerService();
        }

        [TestMethod]
        public void TestEquality_SameValue()
        {
            var f1 = new FeetMeasurement(1.0);
            var f2 = new FeetMeasurement(1.0);

            bool result = f1.Equals(f2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEquality_DifferentValue()
        {
            var f1 = new FeetMeasurement(1.0);
            var f2 = new FeetMeasurement(2.0);

            bool result = f1.Equals(f2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            var f1 = new FeetMeasurement(1.0);

            bool result = f1.Equals(null);

            Assert.IsFalse(result);
        }

        
        // Since Equals only accepts object, passing a different type should return false
        [TestMethod]
        public void TestEquality_NonNumericInput()
        {
            var f1 = new FeetMeasurement(1.0);

            object nonNumeric = "invalid";

            bool result = f1.Equals(nonNumeric);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            var f1 = new FeetMeasurement(1.0);

            bool result = f1.Equals(f1);

            Assert.IsTrue(result);
        }
    }
}