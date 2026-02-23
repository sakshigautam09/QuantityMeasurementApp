using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class ExtendedUnitTestLength
    {
        [TestMethod]
        public void TestEquality_YardToYard_SameValue()
        {
            var q1 = new Length(1.0, LengthUnit.Yard);
            var q2 = new Length(1.0, LengthUnit.Yard);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_YardToYard_DifferentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Yard);
            var q2 = new Length(2.0, LengthUnit.Yard);

            Assert.IsFalse(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_YardToFeet_EquivalentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Yard);
            var q2 = new Length(3.0, LengthUnit.Feet);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_FeetToYard_EquivalentValue()
        {
            var q1 = new Length(3.0, LengthUnit.Feet);
            var q2 = new Length(1.0, LengthUnit.Yard);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_YardToInches_EquivalentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Yard);
            var q2 = new Length(36.0, LengthUnit.Inch);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_InchesToYard_EquivalentValue()
        {
            var q1 = new Length(36.0, LengthUnit.Inch);
            var q2 = new Length(1.0, LengthUnit.Yard);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_YardToFeet_NonEquivalentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Yard);
            var q2 = new Length(2.0, LengthUnit.Feet);

            Assert.IsFalse(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_CentimetersToInches_EquivalentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Centimeter);
            var q2 = new Length(0.393701, LengthUnit.Inch);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_CentimetersToFeet_NonEquivalentValue()
        {
            var q1 = new Length(1.0, LengthUnit.Centimeter);
            var q2 = new Length(1.0, LengthUnit.Feet);

            Assert.IsFalse(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_MultiUnit_TransitiveProperty()
        {
            var yard = new Length(1.0, LengthUnit.Yard);
            var feet = new Length(3.0, LengthUnit.Feet);
            var inches = new Length(36.0, LengthUnit.Inch);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }

        [TestMethod]
        public void TestEquality_YardSameReference()
        {
            var q = new Length(1.0, LengthUnit.Yard);

            Assert.IsTrue(q.Equals(q));
        }

        [TestMethod]
        public void TestEquality_YardNullComparison()
        {
            var q = new Length(1.0, LengthUnit.Yard);

            Assert.IsFalse(q.Equals(null));
        }

        [TestMethod]
        public void TestEquality_CentimetersSameReference()
        {
            var q = new Length(10.0, LengthUnit.Centimeter);

            Assert.IsTrue(q.Equals(q));
        }

        [TestMethod]
        public void TestEquality_CentimetersNullComparison()
        {
            var q = new Length(10.0, LengthUnit.Centimeter);

            Assert.IsFalse(q.Equals(null));
        }

        [TestMethod]
        public void TestEquality_AllUnits_ComplexScenario()
        {
            var yard = new Length(2.0, LengthUnit.Yard);
            var feet = new Length(6.0, LengthUnit.Feet);
            var inches = new Length(72.0, LengthUnit.Inch);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }
    }
}