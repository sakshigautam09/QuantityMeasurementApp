using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class VolumeTests
    {
        private const double epsilon = 0.0001;

        // --------------------
        // Equality Tests
        // --------------------

        [TestMethod]
        public void TestEquality_LitreToLitre_SameValue()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(1.0, VolumeUnit.Litre);

            Assert.IsTrue(v1.Equals(v2));
        }

        [TestMethod]
        public void TestEquality_LitreToLitre_DifferentValue()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(2.0, VolumeUnit.Litre);

            Assert.IsFalse(v1.Equals(v2));
        }

        [TestMethod]
        public void TestEquality_LitreToMillilitre()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(1000.0, VolumeUnit.Millilitre);

            Assert.IsTrue(v1.Equals(v2));
        }

        [TestMethod]
        public void TestEquality_MillilitreToLitre()
        {
            var v1 = new Volume(1000.0, VolumeUnit.Millilitre);
            var v2 = new Volume(1.0, VolumeUnit.Litre);

            Assert.IsTrue(v1.Equals(v2));
        }

        [TestMethod]
        public void TestEquality_LitreToGallon()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(0.264172, VolumeUnit.Gallon);

            Assert.IsTrue(v1.Equals(v2));
        }

        [TestMethod]
        public void TestEquality_GallonToLitre()
        {
            var v1 = new Volume(1.0, VolumeUnit.Gallon);
            var v2 = new Volume(3.78541, VolumeUnit.Litre);

            Assert.IsTrue(v1.Equals(v2));
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            var v = new Volume(1.0, VolumeUnit.Litre);

            Assert.IsFalse(v.Equals(null));
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            var v = new Volume(1.0, VolumeUnit.Litre);

            Assert.IsTrue(v.Equals(v));
        }

        // --------------------
        // Conversion Tests
        // --------------------

        [TestMethod]
        public void TestConversion_LitreToMillilitre()
        {
            var v = new Volume(1.0, VolumeUnit.Litre);

            var result = v.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(1000.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_MillilitreToLitre()
        {
            var v = new Volume(1000.0, VolumeUnit.Millilitre);

            var result = v.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(1.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_GallonToLitre()
        {
            var v = new Volume(1.0, VolumeUnit.Gallon);

            var result = v.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(3.78541, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_LitreToGallon()
        {
            var v = new Volume(3.78541, VolumeUnit.Litre);

            var result = v.ConvertTo(VolumeUnit.Gallon);

            Assert.AreEqual(1.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_RoundTrip()
        {
            var v = new Volume(1.5, VolumeUnit.Litre);

            var result = v.ConvertTo(VolumeUnit.Millilitre)
                          .ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(1.5, result.Value, epsilon);
        }

        [TestMethod]
        public void TestConversion_ZeroValue()
        {
            var v = new Volume(0.0, VolumeUnit.Litre);

            var result = v.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(0.0, result.Value, epsilon);
        }

        // --------------------
        // Addition Tests
        // --------------------

        [TestMethod]
        public void TestAddition_SameUnit_LitrePlusLitre()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(2.0, VolumeUnit.Litre);

            var result = v1.Add(v2);

            Assert.AreEqual(3.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_SameUnit_MillilitrePlusMillilitre()
        {
            var v1 = new Volume(500.0, VolumeUnit.Millilitre);
            var v2 = new Volume(500.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2);

            Assert.AreEqual(1000.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_CrossUnit_LitrePlusMillilitre()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(1000.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2);

            Assert.AreEqual(2.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_CrossUnit_MillilitrePlusLitre()
        {
            var v1 = new Volume(1000.0, VolumeUnit.Millilitre);
            var v2 = new Volume(1.0, VolumeUnit.Litre);

            var result = v1.Add(v2);

            Assert.AreEqual(2000.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_CrossUnit_GallonPlusLitre()
        {
            var v1 = new Volume(1.0, VolumeUnit.Gallon);
            var v2 = new Volume(3.78541, VolumeUnit.Litre);

            var result = v1.Add(v2);

            Assert.AreEqual(2.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_Litre()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(1000.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2, VolumeUnit.Litre);

            Assert.AreEqual(2.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_ExplicitTargetUnit_Millilitre()
        {
            var v1 = new Volume(1.0, VolumeUnit.Litre);
            var v2 = new Volume(1000.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2, VolumeUnit.Millilitre);

            Assert.AreEqual(2000.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_WithZero()
        {
            var v1 = new Volume(5.0, VolumeUnit.Litre);
            var v2 = new Volume(0.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2);

            Assert.AreEqual(5.0, result.Value, epsilon);
        }

        [TestMethod]
        public void TestAddition_NegativeValues()
        {
            var v1 = new Volume(5.0, VolumeUnit.Litre);
            var v2 = new Volume(-2000.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2);

            Assert.AreEqual(3.0, result.Value, epsilon);
        }
    }
}