using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class LengthAdditionTests
    {
        private const double EPSILON = 1e-6;

        [TestMethod]
        public void TestAddition_SameUnit_FeetPlusFeet()
        {
            var l1 = new Length(1.0, LengthUnit.Feet);
            var l2 = new Length(2.0, LengthUnit.Feet);

            var result = l1.Add(l2);

            Assert.AreEqual(3.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestAddition_SameUnit_InchPlusInch()
        {
            var l1 = new Length(6.0, LengthUnit.Inch);
            var l2 = new Length(6.0, LengthUnit.Inch);

            var result = l1.Add(l2);

            Assert.AreEqual(12.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Inch, result.Unit);
        }

        [TestMethod]
        public void TestAddition_CrossUnit_FeetPlusInches()
        {
            var l1 = new Length(1.0, LengthUnit.Feet);
            var l2 = new Length(12.0, LengthUnit.Inch);

            var result = l1.Add(l2);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestAddition_CrossUnit_InchPlusFeet()
        {
            var l1 = new Length(12.0, LengthUnit.Inch);
            var l2 = new Length(1.0, LengthUnit.Feet);

            var result = l1.Add(l2);

            Assert.AreEqual(24.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Inch, result.Unit);
        }

        [TestMethod]
        public void TestAddition_CrossUnit_YardPlusFeet()
        {
            var l1 = new Length(1.0, LengthUnit.Yard);
            var l2 = new Length(3.0, LengthUnit.Feet);

            var result = l1.Add(l2);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Yard, result.Unit);
        }

        [TestMethod]
        public void TestAddition_CrossUnit_CentimeterPlusInch()
        {
            var l1 = new Length(2.54, LengthUnit.Centimeter);
            var l2 = new Length(1.0, LengthUnit.Inch);

            var result = l1.Add(l2);

            Assert.AreEqual(5.08, result.Value, 1e-3);
            Assert.AreEqual(LengthUnit.Centimeter, result.Unit);
        }

        [TestMethod]
        public void TestAddition_Commutativity()
        {
            var a = new Length(1.0, LengthUnit.Feet);
            var b = new Length(12.0, LengthUnit.Inch);

            var result1 = a.Add(b);
            var result2 = b.Add(a);

            Assert.IsTrue(result1.Equals(result2));
        }

        [TestMethod]
        public void TestAddition_WithZero()
        {
            var l1 = new Length(5.0, LengthUnit.Feet);
            var l2 = new Length(0.0, LengthUnit.Inch);

            var result = l1.Add(l2);

            Assert.AreEqual(5.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestAddition_NegativeValues()
        {
            var l1 = new Length(5.0, LengthUnit.Feet);
            var l2 = new Length(-2.0, LengthUnit.Feet);

            var result = l1.Add(l2);

            Assert.AreEqual(3.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestAddition_NullSecondOperand()
        {
            var l1 = new Length(1.0, LengthUnit.Feet);

            Assert.Throws<ArgumentNullException>(() =>
            {
                l1.Add(null!);
            });
        }

        [TestMethod]
        public void TestAddition_LargeValues()
        {
            var l1 = new Length(1e6, LengthUnit.Feet);
            var l2 = new Length(1e6, LengthUnit.Feet);

            var result = l1.Add(l2);

            Assert.AreEqual(2e6, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void TestAddition_SmallValues()
        {
            var l1 = new Length(0.001, LengthUnit.Feet);
            var l2 = new Length(0.002, LengthUnit.Feet);

            var result = l1.Add(l2);

            Assert.AreEqual(0.003, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }
    }
}