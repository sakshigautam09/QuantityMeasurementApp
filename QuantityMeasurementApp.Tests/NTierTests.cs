// // ============================================================
// // PROJECT : QuantityMeasurementApp.Tests
// // FILE    : UC15Tests.cs
// // ============================================================

// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using QuantityMeasurementApp.Console;
// using QuantityMeasurementApp.BusinessLayer;
// using QuantityMeasurementApp.Core.Entities;
// using QuantityMeasurementApp.Core.Services;
// using QuantityMeasurementApp.ModelLayer;
// using QuantityMeasurementApp.RepositoryLayer;
// using System;

// namespace QuantityMeasurementApp.Tests
// {
//     [TestClass]
//     public class NTierTests
//     {
//         private IQuantityMeasurementService    _service    = null!;
//         private IQuantityMeasurementRepository _repository = null!;
//         private QuantityMeasurementController  _controller = null!;

//         [TestInitialize]
//         public void Setup()
//         {
//             _repository = QuantityMeasurementCacheRepository.Instance;
//             _repository.Clear();

//             _service = new QuantityMeasurementServiceImpl(
//                 new QuantityModelServiceImpl(),
//                 new TemperatureService(),
//                 _repository);

//             _controller = new QuantityMeasurementController(_service, _repository);
//         }

//         // ── Compare ──────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Compare_1Feet_12Inch_AreEqual()
//         {
//             var result = _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
//             Assert.AreEqual(1.0, result.Value);
//         }

//         [TestMethod]
//         public void Compare_1Feet_2Feet_AreNotEqual()
//         {
//             var result = _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(0.0, result.Value);
//         }

//         [TestMethod]
//         public void Compare_1Kg_1000Gram_AreEqual()
//         {
//             var result = _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
//                 new QuantityDTO(1000.0, QuantityDTO.WeightUnit.Gram));
//             Assert.AreEqual(1.0, result.Value);
//         }

//         [TestMethod]
//         public void Compare_1Litre_1000Millilitre_AreEqual()
//         {
//             var result = _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.VolumeUnit.Litre),
//                 new QuantityDTO(1000.0, QuantityDTO.VolumeUnit.Millilitre));
//             Assert.AreEqual(1.0, result.Value);
//         }

//         [TestMethod]
//         public void Compare_100Celsius_212Fahrenheit_AreEqual()
//         {
//             var result = _service.Compare(
//                 new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
//                 new QuantityDTO(212.0, QuantityDTO.TemperatureUnit.Fahrenheit));
//             Assert.AreEqual(1.0, result.Value);
//         }

//         [TestMethod]
//         public void Compare_LengthAndWeight_ThrowsException()
//         {
//             Assert.ThrowsException<QuantityMeasurementException>(() =>
//                 _service.Compare(
//                     new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                     new QuantityDTO(1.0, QuantityDTO.WeightUnit.Gram)));
//         }

//         // ── Convert ──────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Convert_1Feet_ToInch_Returns12()
//         {
//             var result = _service.Convert(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
//             Assert.AreEqual(12.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Convert_1Kg_ToGram_Returns1000()
//         {
//             var result = _service.Convert(
//                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
//                 new QuantityDTO(0.0, QuantityDTO.WeightUnit.Gram));
//             Assert.AreEqual(1000.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Convert_1Litre_ToMillilitre_Returns1000()
//         {
//             var result = _service.Convert(
//                 new QuantityDTO(1.0, QuantityDTO.VolumeUnit.Litre),
//                 new QuantityDTO(0.0, QuantityDTO.VolumeUnit.Millilitre));
//             Assert.AreEqual(1000.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Convert_100Celsius_ToFahrenheit_Returns212()
//         {
//             var result = _service.Convert(
//                 new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
//                 new QuantityDTO(0.0, QuantityDTO.TemperatureUnit.Fahrenheit));
//             Assert.AreEqual(212.0, result.Value, 0.0001);
//         }

//         // ── Add ───────────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Add_2Feet_3Feet_Returns5()
//         {
//             var result = _service.Add(
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(5.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Add_1Feet_12Inch_WithTargetInch_Returns24()
//         {
//             var result = _service.AddWithTargetUnit(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch),
//                 new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
//             Assert.AreEqual(24.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Add_500Gram_500Gram_Returns1Kg()
//         {
//             var result = _service.AddWithTargetUnit(
//                 new QuantityDTO(500.0, QuantityDTO.WeightUnit.Gram),
//                 new QuantityDTO(500.0, QuantityDTO.WeightUnit.Gram),
//                 new QuantityDTO(0.0, QuantityDTO.WeightUnit.Kilogram));
//             Assert.AreEqual(1.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Add_Temperature_ThrowsException()
//         {
//             Assert.ThrowsException<QuantityMeasurementException>(() =>
//                 _service.Add(
//                     new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
//                     new QuantityDTO(50.0, QuantityDTO.TemperatureUnit.Celsius)));
//         }

//         // ── Subtract ─────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Subtract_5Feet_2Feet_Returns3()
//         {
//             var result = _service.Subtract(
//                 new QuantityDTO(5.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(3.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Subtract_1Kg_200Gram_WithTargetGram_Returns800()
//         {
//             var result = _service.SubtractWithTargetUnit(
//                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
//                 new QuantityDTO(200.0, QuantityDTO.WeightUnit.Gram),
//                 new QuantityDTO(0.0, QuantityDTO.WeightUnit.Gram));
//             Assert.AreEqual(800.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Subtract_Temperature_ThrowsException()
//         {
//             Assert.ThrowsException<QuantityMeasurementException>(() =>
//                 _service.Subtract(
//                     new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
//                     new QuantityDTO(50.0, QuantityDTO.TemperatureUnit.Celsius)));
//         }

//         // ── Divide ────────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Divide_10Feet_2Feet_Returns5()
//         {
//             var result = _service.Divide(
//                 new QuantityDTO(10.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(5.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Divide_ByZero_ThrowsException()
//         {
//             Assert.ThrowsException<QuantityMeasurementException>(() =>
//                 _service.Divide(
//                     new QuantityDTO(10.0, QuantityDTO.LengthUnit.Feet),
//                     new QuantityDTO(0.0, QuantityDTO.LengthUnit.Feet)));
//         }

//         [TestMethod]
//         public void Divide_Temperature_ThrowsException()
//         {
//             Assert.ThrowsException<QuantityMeasurementException>(() =>
//                 _service.Divide(
//                     new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
//                     new QuantityDTO(50.0, QuantityDTO.TemperatureUnit.Celsius)));
//         }

//         // ── Repository ────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Repository_AfterCompare_CountIsOne()
//         {
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(1, _repository.GetTotalCount());
//         }

//         [TestMethod]
//         public void Repository_After3Operations_CountIs3()
//         {
//             var l1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
//             var l2 = new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet);
//             _service.Compare(l1, l2);
//             _service.Add(l1, l2);
//             _service.Subtract(l2, l1);
//             Assert.AreEqual(3, _repository.GetTotalCount());
//         }

//         [TestMethod]
//         public void Repository_Clear_CountIsZero()
//         {
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet));
//             _repository.Clear();
//             Assert.AreEqual(0, _repository.GetTotalCount());
//         }

//         // ── Controller ────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Controller_PerformCompare_EqualValues_ReturnsOne()
//         {
//             var result = _controller.PerformCompare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
//             Assert.AreEqual(1.0, result.Value);
//         }

//         [TestMethod]
//         public void Controller_PerformAdd_ReturnsSumCorrectly()
//         {
//             var result = _controller.PerformAdd(
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(5.0, result.Value, 0.0001);
//         }

//         [TestMethod]
//         public void Controller_PerformCompare_CrossCategory_ReturnsNaN()
//         {
//             var result = _controller.PerformCompare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Gram));
//             Assert.IsTrue(double.IsNaN(result.Value));
//         }
//     }
// }







// ============================================================
// PROJECT : QuantityMeasurementApp.Tests
// FILE    : NTierTests.cs
// ============================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.Console;
using QuantityMeasurementApp.Core.Services;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    [DoNotParallelize] // CacheRepository is a singleton — must run sequentially
    public class NTierTests
    {
        private IQuantityMeasurementService    _service    = null!;
        private IQuantityMeasurementRepository _repository = null!;
        private QuantityMeasurementController  _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = QuantityMeasurementCacheRepository.Instance;
            _repository.Clear();

            _service = new QuantityMeasurementServiceImpl(
                new QuantityModelServiceImpl(),
                new TemperatureService(),
                _repository);

            _controller = new QuantityMeasurementController(_service, _repository);
        }

        // ── Compare ──────────────────────────────────────────────────────────────

        [TestMethod]
        public void Compare_1Feet_12Inch_AreEqual()
        {
            var result = _service.Compare(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
            Assert.AreEqual(1.0, result.Value);
        }

        [TestMethod]
        public void Compare_1Feet_2Feet_AreNotEqual()
        {
            var result = _service.Compare(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
            Assert.AreEqual(0.0, result.Value);
        }

        [TestMethod]
        public void Compare_1Kg_1000Gram_AreEqual()
        {
            var result = _service.Compare(
                new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
                new QuantityDTO(1000.0, QuantityDTO.WeightUnit.Gram));
            Assert.AreEqual(1.0, result.Value);
        }

        [TestMethod]
        public void Compare_1Litre_1000Millilitre_AreEqual()
        {
            var result = _service.Compare(
                new QuantityDTO(1.0, QuantityDTO.VolumeUnit.Litre),
                new QuantityDTO(1000.0, QuantityDTO.VolumeUnit.Millilitre));
            Assert.AreEqual(1.0, result.Value);
        }

        [TestMethod]
        public void Compare_100Celsius_212Fahrenheit_AreEqual()
        {
            var result = _service.Compare(
                new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
                new QuantityDTO(212.0, QuantityDTO.TemperatureUnit.Fahrenheit));
            Assert.AreEqual(1.0, result.Value);
        }

        [TestMethod]
        public void Compare_LengthAndWeight_ThrowsException()
        {
            Assert.ThrowsExactly<QuantityMeasurementException>(() =>
                _service.Compare(
                    new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                    new QuantityDTO(1.0, QuantityDTO.WeightUnit.Gram)));
        }

        // ── Convert ──────────────────────────────────────────────────────────────

        [TestMethod]
        public void Convert_1Feet_ToInch_Returns12()
        {
            var result = _service.Convert(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
            Assert.AreEqual(12.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Convert_1Kg_ToGram_Returns1000()
        {
            var result = _service.Convert(
                new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
                new QuantityDTO(0.0, QuantityDTO.WeightUnit.Gram));
            Assert.AreEqual(1000.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Convert_1Litre_ToMillilitre_Returns1000()
        {
            var result = _service.Convert(
                new QuantityDTO(1.0, QuantityDTO.VolumeUnit.Litre),
                new QuantityDTO(0.0, QuantityDTO.VolumeUnit.Millilitre));
            Assert.AreEqual(1000.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Convert_100Celsius_ToFahrenheit_Returns212()
        {
            var result = _service.Convert(
                new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
                new QuantityDTO(0.0, QuantityDTO.TemperatureUnit.Fahrenheit));
            Assert.AreEqual(212.0, result.Value, 0.0001);
        }

        // ── Add ───────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Add_2Feet_3Feet_Returns5()
        {
            var result = _service.Add(
                new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));
            Assert.AreEqual(5.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Add_1Feet_12Inch_WithTargetInch_Returns24()
        {
            var result = _service.AddWithTargetUnit(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch),
                new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
            Assert.AreEqual(24.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Add_500Gram_500Gram_Returns1Kg()
        {
            var result = _service.AddWithTargetUnit(
                new QuantityDTO(500.0, QuantityDTO.WeightUnit.Gram),
                new QuantityDTO(500.0, QuantityDTO.WeightUnit.Gram),
                new QuantityDTO(0.0, QuantityDTO.WeightUnit.Kilogram));
            Assert.AreEqual(1.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Add_Temperature_ThrowsException()
        {
            Assert.ThrowsExactly<QuantityMeasurementException>(() =>
                _service.Add(
                    new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
                    new QuantityDTO(50.0, QuantityDTO.TemperatureUnit.Celsius)));
        }

        // ── Subtract ─────────────────────────────────────────────────────────────

        [TestMethod]
        public void Subtract_5Feet_2Feet_Returns3()
        {
            var result = _service.Subtract(
                new QuantityDTO(5.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
            Assert.AreEqual(3.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Subtract_1Kg_200Gram_WithTargetGram_Returns800()
        {
            var result = _service.SubtractWithTargetUnit(
                new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
                new QuantityDTO(200.0, QuantityDTO.WeightUnit.Gram),
                new QuantityDTO(0.0, QuantityDTO.WeightUnit.Gram));
            Assert.AreEqual(800.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Subtract_Temperature_ThrowsException()
        {
            Assert.ThrowsExactly<QuantityMeasurementException>(() =>
                _service.Subtract(
                    new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
                    new QuantityDTO(50.0, QuantityDTO.TemperatureUnit.Celsius)));
        }

        // ── Divide ────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Divide_10Feet_2Feet_Returns5()
        {
            var result = _service.Divide(
                new QuantityDTO(10.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
            Assert.AreEqual(5.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Divide_ByZero_ThrowsException()
        {
            Assert.ThrowsExactly<QuantityMeasurementException>(() =>
                _service.Divide(
                    new QuantityDTO(10.0, QuantityDTO.LengthUnit.Feet),
                    new QuantityDTO(0.0, QuantityDTO.LengthUnit.Feet)));
        }

        [TestMethod]
        public void Divide_Temperature_ThrowsException()
        {
            Assert.ThrowsExactly<QuantityMeasurementException>(() =>
                _service.Divide(
                    new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
                    new QuantityDTO(50.0, QuantityDTO.TemperatureUnit.Celsius)));
        }

        // ── Repository ────────────────────────────────────────────────────────────

        [TestMethod]
        public void Repository_AfterCompare_CountIsOne()
        {
            _service.Compare(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet));
            Assert.AreEqual(1, _repository.GetTotalCount());
        }

        [TestMethod]
        public void Repository_After3Operations_CountIs3()
        {
            var l1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
            var l2 = new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet);
            _service.Compare(l1, l2);
            _service.Add(l1, l2);
            _service.Subtract(l2, l1);
            Assert.AreEqual(3, _repository.GetTotalCount());
        }

        [TestMethod]
        public void Repository_Clear_CountIsZero()
        {
            _service.Compare(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet));
            _repository.Clear();
            Assert.AreEqual(0, _repository.GetTotalCount());
        }

        // ── Controller ────────────────────────────────────────────────────────────

        [TestMethod]
        public void Controller_PerformCompare_EqualValues_ReturnsOne()
        {
            var result = _controller.PerformCompare(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
            Assert.AreEqual(1.0, result.Value);
        }

        [TestMethod]
        public void Controller_PerformAdd_ReturnsSumCorrectly()
        {
            var result = _controller.PerformAdd(
                new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));
            Assert.AreEqual(5.0, result.Value, 0.0001);
        }

        [TestMethod]
        public void Controller_PerformCompare_CrossCategory_ReturnsNaN()
        {
            var result = _controller.PerformCompare(
                new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
                new QuantityDTO(1.0, QuantityDTO.WeightUnit.Gram));
            Assert.IsTrue(double.IsNaN(result.Value));
        }
    }
}