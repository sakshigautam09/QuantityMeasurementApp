using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class LengthTests
    {
        [TestMethod]
        public void TestEquality_FeetToFeet_SameValue()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(1.0, LengthUnit.Feet);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_InchToInch_SameValue()
        {
            var q1 = new Length(1.0, LengthUnit.Inch);
            var q2 = new Length(1.0, LengthUnit.Inch);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_FeetToInch_EquivalentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(12.0, LengthUnit.Inch);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_InchToFeet_EquivalentValue()
        {
            var q1 = new Length(12.0, LengthUnit.Inch);
            var q2 = new Length(1.0, LengthUnit.Feet);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_FeetToFeet_DifferentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Feet);
            var q2 = new Length(2.0, LengthUnit.Feet);

            Assert.IsFalse(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_InchToInch_DifferentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Inch);
            var q2 = new Length(2.0, LengthUnit.Inch);

            Assert.IsFalse(q1.Equals(q2));
        }

        [TestMethod]
        public void Test_InvalidInput_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var length = new Length(-1, LengthUnit.Feet);
            });
        }

        [TestMethod]
        public void TestEquality_InvalidUnit_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var q = new Length(1.0, (LengthUnit)(-1));
                q.ToFeet();
            });
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            var q = new Length(5.0, LengthUnit.Feet);

            Assert.IsTrue(q.Equals(q));
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            var q = new Length(1.0, LengthUnit.Feet);

            Assert.IsFalse(q.Equals(null));
        }
    }
}