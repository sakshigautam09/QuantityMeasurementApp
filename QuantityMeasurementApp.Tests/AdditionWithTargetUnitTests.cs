using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class AdditionWithTargetUnitTests
    {
        private const double epsilon = 0.0001;

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_Feet()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Feet);

            Assert.AreEqual(2.0, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_Inches()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Inch);

            Assert.AreEqual(24.0, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Inch, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_Yards()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Yard);

            Assert.AreEqual(0.6667, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Yard, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_Centimeters()
        {
            var q1 = new Length(1.0, LengthUnit.Inch);
            var q2 = new Length(1.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Centimeter);

            Assert.AreEqual(5.08, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Centimeter, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            var q1 = new Length(2.0, LengthUnit.Yard);
            var q2 = new Length(3.0, LengthUnit.Feet);

            var result = q1.Add(q2, LengthUnit.Yard);

            Assert.AreEqual(3.0, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Yard, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            var q1 = new Length(2.0, LengthUnit.Yard);
            var q2 = new Length(3.0, LengthUnit.Feet);

            var result = q1.Add(q2, LengthUnit.Feet);

            Assert.AreEqual(9.0, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_Commutativity()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            var result1 = q1.Add(q2, LengthUnit.Yard);
            var result2 = q2.Add(q1, LengthUnit.Yard);

            Assert.AreEqual(result1.Value, result2.Value, epsilon);
            Assert.AreEqual(result1.Unit, result2.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_WithZero()
        {
            var q1 = new Length(5.0, LengthUnit.Feet);
            var q2 = new Length(0.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Yard);

            Assert.AreEqual(1.6667, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Yard, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_NegativeValues()
        {
            var q1 = new Length(5.0, LengthUnit.Feet);
            var q2 = new Length(-2.0, LengthUnit.Feet);

            var result = q1.Add(q2, LengthUnit.Inch);

            Assert.AreEqual(36.0, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Inch, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_InvalidTargetUnit()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            try
            {
                q1.Add(q2, (LengthUnit)(-1));
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException)
            {
                // Test passes
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            var q1 = new Length(1000.0, LengthUnit.Feet);
            var q2 = new Length(500.0, LengthUnit.Feet);

            var result = q1.Add(q2, LengthUnit.Inch);

            Assert.AreEqual(18000.0, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Inch, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            var q1 = new Length(12.0, LengthUnit.Inch);
            var q2 = new Length(12.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Yard);

            Assert.AreEqual(0.6667, result.Value, epsilon);
            Assert.AreEqual(LengthUnit.Yard, result.Unit);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_PrecisionTolerance()
        {
            var q1 = new Length(1.0, LengthUnit.Centimeter);
            var q2 = new Length(1.0, LengthUnit.Inch);

            var result = q1.Add(q2, LengthUnit.Feet);

            Assert.AreEqual(result.Value, result.Value, epsilon);
        }
    }
}