using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class TemperatureTests
    {
        private const double epsilon = 0.0001;

        // --------------------
        // Equality Tests
        // --------------------

        [TestMethod]
        public void TestEquality_CelsiusToCelsius_SameValue()
        {
            var t1 = new Temperature(0.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(0.0, TemperatureUnit.Celsius);

            Assert.IsTrue(t1.Equals(t2));
        }

        [TestMethod]
        public void TestEquality_FahrenheitToFahrenheit_SameValue()
        {
            var t1 = new Temperature(32.0, TemperatureUnit.Fahrenheit);
            var t2 = new Temperature(32.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(t1.Equals(t2));
        }

        [TestMethod]
        public void TestEquality_CelsiusToFahrenheit_Zero()
        {
            var t1 = new Temperature(0.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(32.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(t1.Equals(t2));
        }

        [TestMethod]
        public void TestEquality_CelsiusToFahrenheit_BoilingPoint()
        {
            var t1 = new Temperature(100.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(212.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(t1.Equals(t2));
        }

        [TestMethod]
        public void TestEquality_Negative40EqualPoint()
        {
            var t1 = new Temperature(-40.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(-40.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(t1.Equals(t2));
        }

        [TestMethod]
        public void TestEquality_SymmetricProperty()
        {
            var t1 = new Temperature(0.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(32.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(t1.Equals(t2));
            Assert.IsTrue(t2.Equals(t1));
        }

        [TestMethod]
        public void TestEquality_ReflexiveProperty()
        {
            var t = new Temperature(25.0, TemperatureUnit.Celsius);

            Assert.IsTrue(t.Equals(t));
        }

        [TestMethod]
        public void TestEquality_DifferentValues()
        {
            var t1 = new Temperature(50.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(100.0, TemperatureUnit.Celsius);

            Assert.IsFalse(t1.Equals(t2));
        }

        // --------------------
        // Conversion Tests
        // --------------------

        [TestMethod]
        public void TestConversion_CelsiusToFahrenheit()
        {
            var t = new Temperature(100.0, TemperatureUnit.Celsius);

            var result = t.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(212.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_FahrenheitToCelsius()
        {
            var t = new Temperature(32.0, TemperatureUnit.Fahrenheit);

            var result = t.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(0.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_CelsiusToKelvin()
        {
            var t = new Temperature(0.0, TemperatureUnit.Celsius);

            var result = t.ConvertTo(TemperatureUnit.Kelvin);

            Assert.AreEqual(273.15, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_KelvinToCelsius()
        {
            var t = new Temperature(273.15, TemperatureUnit.Kelvin);

            var result = t.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(0.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_RoundTrip()
        {
            var t = new Temperature(50.0, TemperatureUnit.Celsius);

            var result = t.ConvertTo(TemperatureUnit.Fahrenheit)
                          .ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(50.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_AbsoluteZero()
        {
            var t = new Temperature(-273.15, TemperatureUnit.Celsius);

            var result = t.ConvertTo(TemperatureUnit.Kelvin);

            Assert.AreEqual(0.0, result.Value, epsilon);
        }

        // --------------------
        // Unsupported Arithmetic Tests
        // --------------------

        [TestMethod]
        public void TestUnsupportedOperation_Add()
        {
            var t1 = new Temperature(100.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(50.0, TemperatureUnit.Celsius);

            try
            {
                t1.Add(t2);
                Assert.Fail("Expected NotSupportedException was not thrown.");
            }
            catch (NotSupportedException)
            {
            }
        }

        [TestMethod]
        public void TestUnsupportedOperation_Subtract()
        {
            var t1 = new Temperature(100.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(50.0, TemperatureUnit.Celsius);

            try
            {
                t1.Subtract(t2);
                Assert.Fail("Expected NotSupportedException was not thrown.");
            }
            catch (NotSupportedException)
            {
            }
        }

        [TestMethod]
        public void TestUnsupportedOperation_Divide()
        {
            var t1 = new Temperature(100.0, TemperatureUnit.Celsius);
            var t2 = new Temperature(50.0, TemperatureUnit.Celsius);

            try
            {
                t1.Divide(t2);
                Assert.Fail("Expected NotSupportedException was not thrown.");
            }
            catch (NotSupportedException)
            {
            }
        }

        // --------------------
        // Cross Category Safety
        // --------------------

        [TestMethod]
        public void TestTemperatureVsLength()
        {
            var temperature = new Temperature(100.0, TemperatureUnit.Celsius);
            var length = new Length(100.0, LengthUnit.Feet);

            Assert.IsFalse(temperature.Equals(length));
        }

        [TestMethod]
        public void TestTemperatureVsWeight()
        {
            var temperature = new Temperature(50.0, TemperatureUnit.Celsius);
            var weight = new Weight(50.0, WeightUnit.Kilogram);

            Assert.IsFalse(temperature.Equals(weight));
        }

        [TestMethod]
        public void TestTemperatureVsVolume()
        {
            var temperature = new Temperature(25.0, TemperatureUnit.Celsius);
            var volume = new Volume(25.0, VolumeUnit.Litre);

            Assert.IsFalse(temperature.Equals(volume));
        }
    }
}