using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class RefactoredLengthUnitTests
    {
        private const double epsilon = 0.0001;

        // ---------------------------
        // LengthUnit Enum Tests
        // ---------------------------

        [TestMethod]
        public void TestLengthUnitEnum_FeetConstant()
        {
            Assert.AreEqual(1.0, LengthUnit.Feet.GetConversionFactor(), epsilon);
        }

        [TestMethod]
        public void TestLengthUnitEnum_InchesConstant()
        {
            Assert.AreEqual(1.0 / 12.0, LengthUnit.Inch.GetConversionFactor(), epsilon);
        }

        [TestMethod]
        public void TestLengthUnitEnum_YardsConstant()
        {
            Assert.AreEqual(3.0, LengthUnit.Yard.GetConversionFactor(), epsilon);
        }

        [TestMethod]
        public void TestLengthUnitEnum_CentimetersConstant()
        {
            Assert.AreEqual(1.0 / 30.48, LengthUnit.Centimeter.GetConversionFactor(), epsilon);
        }

        // ---------------------------
        // ConvertToBaseUnit Tests
        // ---------------------------

        [TestMethod]
        public void TestConvertToBaseUnit_FeetToFeet()
        {
            Assert.AreEqual(5.0, LengthUnit.Feet.ConvertToBaseUnit(5.0), epsilon);
        }

        [TestMethod]
        public void TestConvertToBaseUnit_InchesToFeet()
        {
            Assert.AreEqual(1.0, LengthUnit.Inch.ConvertToBaseUnit(12.0), epsilon);
        }

        [TestMethod]
        public void TestConvertToBaseUnit_YardsToFeet()
        {
            Assert.AreEqual(3.0, LengthUnit.Yard.ConvertToBaseUnit(1.0), epsilon);
        }

        [TestMethod]
        public void TestConvertToBaseUnit_CentimetersToFeet()
        {
            Assert.AreEqual(1.0, LengthUnit.Centimeter.ConvertToBaseUnit(30.48), epsilon);
        }

        // ---------------------------
        // ConvertFromBaseUnit Tests
        // ---------------------------

        [TestMethod]
        public void TestConvertFromBaseUnit_FeetToFeet()
        {
            Assert.AreEqual(2.0, LengthUnit.Feet.ConvertFromBaseUnit(2.0), epsilon);
        }

        [TestMethod]
        public void TestConvertFromBaseUnit_FeetToInches()
        {
            Assert.AreEqual(12.0, LengthUnit.Inch.ConvertFromBaseUnit(1.0), epsilon);
        }

        [TestMethod]
        public void TestConvertFromBaseUnit_FeetToYards()
        {
            Assert.AreEqual(1.0, LengthUnit.Yard.ConvertFromBaseUnit(3.0), epsilon);
        }

        [TestMethod]
        public void TestConvertFromBaseUnit_FeetToCentimeters()
        {
            Assert.AreEqual(30.48, LengthUnit.Centimeter.ConvertFromBaseUnit(1.0), epsilon);
        }

        // ---------------------------
        // Refactored Quantity Tests
        // ---------------------------

        [TestMethod]
        public void TestQuantityLengthRefactored_Equality()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestQuantityLengthRefactored_ConvertTo()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);

            var result = q1.ConvertTo(LengthUnit.Inch);

            Assert.AreEqual(12.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestQuantityLengthRefactored_Add()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Feet);

            Assert.AreEqual(2.0, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestQuantityLengthRefactored_AddWithTargetUnit()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Yard);

            Assert.AreEqual(0.6667, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Yard, result.Unit);
        }

        // ---------------------------
        // Validation Tests
        // ---------------------------

        [TestMethod]
        public void TestQuantityLengthRefactored_InvalidValue()
        {
            try
            {
                var q = new Length(double.NaN, LengthUnit.Feet);
                Assert.Fail("Expected ArgumentException not thrown.");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        // ---------------------------
        // Round Trip Conversion
        // ---------------------------

        [TestMethod]
        public void TestRoundTripConversion_RefactoredDesign()
        {
            double value = 5.0;

            double inches = LengthUnit.Inch.ConvertFromBaseUnit(
                LengthUnit.Feet.ConvertToBaseUnit(value));

            double backToFeet = LengthUnit.Feet.ConvertFromBaseUnit(
                LengthUnit.Inch.ConvertToBaseUnit(inches));

            Assert.AreEqual(value, backToFeet, epsilon);
        }

        // ---------------------------
        // Enum Immutability
        // ---------------------------

        [TestMethod]
        public void TestUnitImmutability()
        {
            var unit = LengthUnit.Feet;

            Assert.AreEqual(LengthUnit.Feet, unit);
            Assert.AreEqual(1.0, unit.GetConversionFactor(), epsilon);
        }
    }
}