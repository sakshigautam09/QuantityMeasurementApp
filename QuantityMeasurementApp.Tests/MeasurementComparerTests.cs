using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Services;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class MeasurementComparerTests
    {
        private IMeasurementComparer _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _service = new MeasurementComparerService();
        }

        // ================= FEET TESTS =================

        [TestMethod]
        public void FeetEquality_SameValue_ShouldReturnTrue()
        {
            var f1 = _service.CreateFeet(1.0);
            var f2 = _service.CreateFeet(1.0);

            bool result = _service.AreFeetEqual(f1, f2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FeetEquality_DifferentValue_ShouldReturnFalse()
        {
            var f1 = _service.CreateFeet(1.0);
            var f2 = _service.CreateFeet(2.0);

            bool result = _service.AreFeetEqual(f1, f2);

            Assert.IsFalse(result);
        }

        // ================= INCH TESTS =================

        [TestMethod]
        public void InchEquality_SameValue_ShouldReturnTrue()
        {
            var i1 = _service.CreateInch(12.0);
            var i2 = _service.CreateInch(12.0);

            bool result = _service.AreInchesEqual(i1, i2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InchEquality_DifferentValue_ShouldReturnFalse()
        {
            var i1 = _service.CreateInch(12.0);
            var i2 = _service.CreateInch(24.0);

            bool result = _service.AreInchesEqual(i1, i2);

            Assert.IsFalse(result);
        }

        // ================= FEET & INCH COMPARISON =================

        [TestMethod]
        public void FeetAndInch_EqualValues_ShouldReturnTrue()
        {
            var feet = _service.CreateFeet(1.0);
            var inch = _service.CreateInch(12.0);

            bool result = _service.AreFeetAndInchEqual(feet, inch);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FeetAndInch_NotEqualValues_ShouldReturnFalse()
        {
            var feet = _service.CreateFeet(1.0);
            var inch = _service.CreateInch(24.0);

            bool result = _service.AreFeetAndInchEqual(feet, inch);

            Assert.IsFalse(result);
        }

        // ================= VALIDATION TEST =================

        [TestMethod]
        public void CreateFeet_WithNegativeValue_ShouldThrowException()
        {
            try
            {
                _service.CreateFeet(-1.0);

                // If no exception is thrown, test should fail
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException)
            {
                // Test passes
                Assert.IsTrue(true);
            }
        }
    }
}