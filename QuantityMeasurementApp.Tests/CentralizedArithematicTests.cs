using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class CentralizedArithematicTests
    {
        private const double epsilon = 0.0001;

        // --------------------
        // Equality Tests
        // --------------------

        [TestMethod]
        public void TestEquality_KilogramToKilogram_SameValue()
        {
            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(w1.Value == w2.Value && w1.Unit == w2.Unit);
        }

        [TestMethod]
        public void TestEquality_KilogramToKilogram_DifferentValue()
        {
            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(2.0, WeightUnit.Kilogram);

            Assert.IsFalse(w1.Value == w2.Value && w1.Unit == w2.Unit);
        }

        [TestMethod]
        public void TestEquality_KilogramToGram()
        {
            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);

            var w2Converted = w2.ConvertTo(WeightUnit.Kilogram);
            Assert.IsTrue(w1.Value == w2Converted.Value);
        }

        // --------------------
        // Conversion Tests
        // --------------------

        [TestMethod]
        public void TestConversion_KilogramToGram()
        {
            var w = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

            var result = w.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(1000.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_ZeroValue()
        {
            var w = new Quantity<WeightUnit>(0.0, WeightUnit.Kilogram);

            var result = w.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(0.0, result.Value, epsilon);
        }

        // --------------------
        // Addition Tests
        // --------------------

        [TestMethod]
        public void TestAddition_SameUnit()
        {
            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(2.0, WeightUnit.Kilogram);

            var result = w1.Add(w2);

            Assert.AreEqual(3.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_CrossUnit()
        {
            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);

            var result = w1.Add(w2);

            Assert.AreEqual(2.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_WithZero()
        {
            var w1 = new Quantity<WeightUnit>(5.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(0.0, WeightUnit.Gram);

            var result = w1.Add(w2);

            Assert.AreEqual(5.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_NegativeValues()
        {
            var w1 = new Quantity<WeightUnit>(5.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(-2000.0, WeightUnit.Gram);

            var result = w1.Add(w2);

            Assert.AreEqual(3.0, result.Value, epsilon);
        }
    }
}