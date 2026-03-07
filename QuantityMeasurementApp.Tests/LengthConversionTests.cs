using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class LengthConversionTests
    {
        private const double EPSILON = 1e-6;

        [TestMethod]
        public void TestConversion_FeetToInches()
        {
            double result = Length.Convert(1.0, LengthUnit.Feet, LengthUnit.Inch);
            Assert.AreEqual(12.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_InchesToFeet()
        {
            double result = Length.Convert(24.0, LengthUnit.Inch, LengthUnit.Feet);
            Assert.AreEqual(2.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_YardsToInches()
        {
            double result = Length.Convert(1.0, LengthUnit.Yard, LengthUnit.Inch);
            Assert.AreEqual(36.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_InchesToYards()
        {
            double result = Length.Convert(72.0, LengthUnit.Inch, LengthUnit.Yard);
            Assert.AreEqual(2.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_CentimetersToInches()
        {
            double result = Length.Convert(2.54, LengthUnit.Centimeter, LengthUnit.Inch);
            Assert.AreEqual(1.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_FeetToYard()
        {
            double result = Length.Convert(6.0, LengthUnit.Feet, LengthUnit.Yard);
            Assert.AreEqual(2.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_RoundTrip_PreservesValue()
        {
            double original = 5.5;

            double converted = Length.Convert(original, LengthUnit.Feet, LengthUnit.Inch);
            double back = Length.Convert(converted, LengthUnit.Inch, LengthUnit.Feet);

            Assert.AreEqual(original, back, EPSILON);
        }

        [TestMethod]
        public void TestConversion_ZeroValue()
        {
            double result = Length.Convert(0.0, LengthUnit.Feet, LengthUnit.Inch);
            Assert.AreEqual(0.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_NegativeValue()
        {
            double result = Length.Convert(-1.0, LengthUnit.Feet, LengthUnit.Inch);
            Assert.AreEqual(-12.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConversion_InvalidUnit_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var invalid = (LengthUnit)999;
                Length.Convert(1.0, invalid, LengthUnit.Feet);
            });
        }

        [TestMethod]
        public void TestConversion_NaNOrInfinite_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(double.NaN, LengthUnit.Feet, LengthUnit.Inch);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(double.PositiveInfinity, LengthUnit.Feet, LengthUnit.Inch);
            });
        }
        [TestMethod]
        public void TestConversion_PrecisionTolerance()
        {
            double result = Length.Convert(1.0, LengthUnit.Centimeter, LengthUnit.Inch);

            Assert.IsTrue(Math.Abs(result - 0.393701) < EPSILON);
        }
    }
}