using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class WeightTests
    {
        private const double epsilon = 0.0001;

        // --------------------
        // Equality Tests
        // --------------------

        [TestMethod]
        public void TestEquality_KilogramToKilogram_SameValue()
        {
            var w1 = new Weight(1.0, WeightUnit.Kilogram);
            var w2 = new Weight(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(w1.Equals(w2));
        }

        [TestMethod]
        public void TestEquality_KilogramToKilogram_DifferentValue()
        {
            var w1 = new Weight(1.0, WeightUnit.Kilogram);
            var w2 = new Weight(2.0, WeightUnit.Kilogram);

            Assert.IsFalse(w1.Equals(w2));
        }

        [TestMethod]
        public void TestEquality_KilogramToGram()
        {
            var w1 = new Weight(1.0, WeightUnit.Kilogram);
            var w2 = new Weight(1000.0, WeightUnit.Gram);

            Assert.IsTrue(w1.Equals(w2));
        }

        [TestMethod]
        public void TestEquality_GramToKilogram()
        {
            var w1 = new Weight(1000.0, WeightUnit.Gram);
            var w2 = new Weight(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(w1.Equals(w2));
        }


        [TestMethod]
        public void TestEquality_NullComparison()
        {
            var w = new Weight(1.0, WeightUnit.Kilogram);

            Assert.IsFalse(w.Equals(null));
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            var w = new Weight(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(w.Equals(w));
        }

        // --------------------
        // Conversion Tests
        // --------------------

        [TestMethod]
        public void TestConversion_KilogramToGram()
        {
            var w = new Weight(1.0, WeightUnit.Kilogram);

            var result = w.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(1000.0, result.Value, epsilon);
        }

        
        [TestMethod]
        public void TestConversion_ZeroValue()
        {
            var w = new Weight(0.0, WeightUnit.Kilogram);

            var result = w.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(0.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_RoundTrip()
        {
            var w = new Weight(1.5, WeightUnit.Kilogram);

            var result = w.ConvertTo(WeightUnit.Gram)
                          .ConvertTo(WeightUnit.Kilogram);

            Assert.AreEqual(1.5, result.Value, epsilon);
        }

        // --------------------
        // Addition Tests
        // --------------------

        [TestMethod]
        public void TestAddition_SameUnit()
        {
            var w1 = new Weight(1.0, WeightUnit.Kilogram);
            var w2 = new Weight(2.0, WeightUnit.Kilogram);

            var result = w1.Add(w2);

            Assert.AreEqual(3.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_CrossUnit()
        {
            var w1 = new Weight(1.0, WeightUnit.Kilogram);
            var w2 = new Weight(1000.0, WeightUnit.Gram);

            var result = w1.Add(w2);

            Assert.AreEqual(2.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit()
        {
            var w1 = new Weight(1.0, WeightUnit.Kilogram);
            var w2 = new Weight(1000.0, WeightUnit.Gram);

            var result = w1.Add(w2, WeightUnit.Gram);

            Assert.AreEqual(2000.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_WithZero()
        {
            var w1 = new Weight(5.0, WeightUnit.Kilogram);
            var w2 = new Weight(0.0, WeightUnit.Gram);

            var result = w1.Add(w2);

            Assert.AreEqual(5.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_NegativeValues()
        {
            var w1 = new Weight(5.0, WeightUnit.Kilogram);
            var w2 = new Weight(-2000.0, WeightUnit.Gram);

            var result = w1.Add(w2);

            Assert.AreEqual(3.0, result.Value, epsilon);
        }
    }
}