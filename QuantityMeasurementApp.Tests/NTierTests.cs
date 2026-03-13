// // // ============================================================
// // // PROJECT : QuantityMeasurementApp.Tests
// // // FILE    : QuantityMeasurementAppUC15Tests.cs
// // //
// // // UC-15 : N-Tier Architecture Tests
// // //
// // // Covers:
// // //   • QuantityDTO construction + UnitLabel
// // //   • QuantityMeasurementEntity (all 3 constructors)
// // //   • Repository (Save / FindById / FindAll / Clear)
// // //   • Service – Compare / Convert / Add / Subtract / Divide
// // //     for Length, Weight, Volume, Temperature
// // //   • Service – cross-category rejection
// // //   • Service – Temperature arithmetic rejection
// // //   • Service – null input rejection
// // //   • Service – divide by zero
// // //   • Service – every operation saves to repository
// // //   • Controller – all PerformXxx() methods
// // //   • Controller – error path returns NaN DTO (does not throw)
// // //   • Layer separation / DIP
// // //
// // // NOTE : All UC1-UC14 tests in the existing test files remain
// // //        untouched.  These are purely ADDITIVE tests.
// // // ============================================================

// // using System;
// // using System.Collections.Generic;
// // using Microsoft.VisualStudio.TestTools.UnitTesting;
// // using QuantityMeasurementApp.BusinessLayer;
// // using QuantityMeasurementApp.Core.Services;
// // using QuantityMeasurementApp.ModelLayer;
// // using QuantityMeasurementApp.RepositoryLayer;

// // namespace QuantityMeasurementApp.Tests
// // {
// //     // ── In-memory stub (no disk I/O) ─────────────────────────────────────────────

// //     internal sealed class UC15InMemoryRepository : IQuantityMeasurementRepository
// //     {
// //         private readonly List<QuantityMeasurementEntity> _store = new();

// //         public void Save(QuantityMeasurementEntity e)           => _store.Add(e);
// //         public QuantityMeasurementEntity? FindById(Guid id)    => _store.Find(e => e.Id == id);
// //         public IReadOnlyList<QuantityMeasurementEntity> FindAll() => _store.AsReadOnly();
// //         public void Clear()                                     => _store.Clear();
// //         public int Count                                        => _store.Count;
// //     }

// //     // ── Test class ───────────────────────────────────────────────────────────────

// //     [TestClass]
// //     public class NTierTests
// //     {
// //         private UC15InMemoryRepository        _repo       = null!;
// //         private IQuantityMeasurementService   _service    = null!;
// //         private QuantityMeasurementController _controller = null!;

// //         private const double E = 1e-4;

// //         [TestInitialize]
// //         public void Setup()
// //         {
// //             _repo = new UC15InMemoryRepository();

// //             // Inject EXISTING Core services – exactly as the app does
// //             _service = new QuantityMeasurementServiceImpl(
// //                 new LengthService(),
// //                 new WeightService(),
// //                 new VolumeService(),
// //                 new TemperatureService(),
// //                 _repo);

// //             _controller = new QuantityMeasurementController(_service);
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // QuantityDTO
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void DTO_Length_StoresValueAndUnit()
// //         {
// //             var dto = new QuantityDTO(5.0, QuantityDTO.LengthUnit.Feet);
// //             Assert.AreEqual(5.0, dto.Value, E);
// //             Assert.AreEqual(QuantityDTO.MeasurementType.Length, dto.Type);
// //             Assert.AreEqual(QuantityDTO.LengthUnit.Feet, dto.LengthUnitValue);
// //             Assert.IsNull(dto.WeightUnitValue);
// //         }

// //         [TestMethod]
// //         public void DTO_Weight_StoresValueAndUnit()
// //         {
// //             var dto = new QuantityDTO(1000.0, QuantityDTO.WeightUnit.Gram);
// //             Assert.AreEqual(QuantityDTO.MeasurementType.Weight, dto.Type);
// //             Assert.AreEqual(QuantityDTO.WeightUnit.Gram, dto.WeightUnitValue);
// //         }

// //         [TestMethod]
// //         public void DTO_Volume_StoresValueAndUnit()
// //         {
// //             var dto = new QuantityDTO(2.5, QuantityDTO.VolumeUnit.Litre);
// //             Assert.AreEqual(QuantityDTO.MeasurementType.Volume, dto.Type);
// //         }

// //         [TestMethod]
// //         public void DTO_Temperature_StoresValueAndUnit()
// //         {
// //             var dto = new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius);
// //             Assert.AreEqual(QuantityDTO.MeasurementType.Temperature, dto.Type);
// //         }

// //         [TestMethod]
// //         public void DTO_UnitLabel_ReturnsCorrectString()
// //         {
// //             Assert.AreEqual("Yard", new QuantityDTO(1.0, QuantityDTO.LengthUnit.Yard).UnitLabel);
// //             Assert.AreEqual("Kilogram", new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram).UnitLabel);
// //             Assert.AreEqual("Millilitre", new QuantityDTO(1.0, QuantityDTO.VolumeUnit.Millilitre).UnitLabel);
// //             Assert.AreEqual("Celsius", new QuantityDTO(0.0, QuantityDTO.TemperatureUnit.Celsius).UnitLabel);
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // QuantityMeasurementEntity
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Entity_BinaryConstructor_StoresOperandsAndResult()
// //         {
// //             var d1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
// //             var d2 = new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch);
// //             var e  = new QuantityMeasurementEntity(
// //                 QuantityMeasurementEntity.OperationType.Compare, d1, d2, "True");

// //             Assert.IsFalse(e.HasError);
// //             Assert.AreEqual("True", e.ResultDisplay);
// //             Assert.AreSame(d1, e.FirstOperand);
// //             Assert.AreSame(d2, e.SecondOperand);
// //         }

// //         [TestMethod]
// //         public void Entity_SingleOperandConstructor_Works()
// //         {
// //             var d1   = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
// //             var hint = new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch);
// //             var e    = new QuantityMeasurementEntity(
// //                 QuantityMeasurementEntity.OperationType.Convert, d1, hint, "12 Inch");

// //             Assert.IsFalse(e.HasError);
// //             Assert.AreEqual("12 Inch", e.ResultDisplay);
// //         }

// //         [TestMethod]
// //         public void Entity_ErrorConstructor_SetsHasErrorAndMessage()
// //         {
// //             var d1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
// //             var e  = new QuantityMeasurementEntity(
// //                 QuantityMeasurementEntity.OperationType.Add, d1, null, "Unsupported", true);

// //             Assert.IsTrue(e.HasError);
// //             Assert.AreEqual("Unsupported", e.ErrorMessage);
// //             Assert.AreEqual("ERROR", e.ResultDisplay);
// //         }

// //         [TestMethod]
// //         public void Entity_ToString_ContainsOperationAndResult()
// //         {
// //             var d1 = new QuantityDTO(2.0, QuantityDTO.WeightUnit.Kilogram);
// //             var d2 = new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram);
// //             var e  = new QuantityMeasurementEntity(
// //                 QuantityMeasurementEntity.OperationType.Subtract, d1, d2, "1 Kilogram");

// //             Assert.IsTrue(e.ToString().Contains("Subtract"));
// //             Assert.IsTrue(e.ToString().Contains("1 Kilogram"));
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Repository
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Repo_Save_FindAll_CountIncreases()
// //         {
// //             var d = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
// //             var e = new QuantityMeasurementEntity(
// //                 QuantityMeasurementEntity.OperationType.Compare, d, d, "True");
// //             _repo.Save(e);
// //             Assert.AreEqual(1, _repo.FindAll().Count);
// //         }

// //         [TestMethod]
// //         public void Repo_FindById_ReturnsCorrectEntity()
// //         {
// //             var d = new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram);
// //             var e = new QuantityMeasurementEntity(
// //                 QuantityMeasurementEntity.OperationType.Convert, d,
// //                 new QuantityDTO(0, QuantityDTO.WeightUnit.Gram), "1000 Gram");
// //             _repo.Save(e);
// //             Assert.AreSame(e, _repo.FindById(e.Id));
// //         }

// //         [TestMethod]
// //         public void Repo_Clear_RemovesAll()
// //         {
// //             var d = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
// //             _repo.Save(new QuantityMeasurementEntity(
// //                 QuantityMeasurementEntity.OperationType.Compare, d, d, "True"));
// //             _repo.Clear();
// //             Assert.AreEqual(0, _repo.FindAll().Count);
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – COMPARE
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Service_Compare_Length_SameUnit_Equal()
// //         {
// //             var r = _service.Compare(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Compare_Length_CrossUnit_Equal()
// //         {
// //             var r = _service.Compare(
// //                 new QuantityDTO(1.0,  QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Compare_Weight_CrossUnit_Equal()
// //         {
// //             var r = _service.Compare(
// //                 new QuantityDTO(1.0,    QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(1000.0, QuantityDTO.WeightUnit.Gram));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Compare_Volume_CrossUnit_Equal()
// //         {
// //             var r = _service.Compare(
// //                 new QuantityDTO(1.0,    QuantityDTO.VolumeUnit.Litre),
// //                 new QuantityDTO(1000.0, QuantityDTO.VolumeUnit.Millilitre));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Compare_Temperature_CrossUnit_Equal()
// //         {
// //             var r = _service.Compare(
// //                 new QuantityDTO(0.0,  QuantityDTO.TemperatureUnit.Celsius),
// //                 new QuantityDTO(32.0, QuantityDTO.TemperatureUnit.Fahrenheit));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Compare_NotEqual_ReturnsZero()
// //         {
// //             var r = _service.Compare(
// //                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(2.0, QuantityDTO.WeightUnit.Kilogram));
// //             Assert.AreEqual(0.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Compare_CrossCategory_Throws()
// //         {
// //             _service.Compare(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram));
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – CONVERT
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Service_Convert_FeetToInch()
// //         {
// //             var r = _service.Convert(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
// //             Assert.AreEqual(12.0, r.Value, E);
// //             Assert.AreEqual(QuantityDTO.LengthUnit.Inch, r.LengthUnitValue);
// //         }

// //         [TestMethod]
// //         public void Service_Convert_KilogramToGram()
// //         {
// //             var r = _service.Convert(
// //                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(0.0, QuantityDTO.WeightUnit.Gram));
// //             Assert.AreEqual(1000.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Convert_LitreToMillilitre()
// //         {
// //             var r = _service.Convert(
// //                 new QuantityDTO(2.0, QuantityDTO.VolumeUnit.Litre),
// //                 new QuantityDTO(0.0, QuantityDTO.VolumeUnit.Millilitre));
// //             Assert.AreEqual(2000.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Convert_CelsiusToFahrenheit()
// //         {
// //             var r = _service.Convert(
// //                 new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
// //                 new QuantityDTO(0.0,   QuantityDTO.TemperatureUnit.Fahrenheit));
// //             Assert.AreEqual(212.0, r.Value, 0.01);
// //         }

// //         [TestMethod]
// //         public void Service_Convert_ZeroCelsiusToKelvin()
// //         {
// //             var r = _service.Convert(
// //                 new QuantityDTO(0.0, QuantityDTO.TemperatureUnit.Celsius),
// //                 new QuantityDTO(0.0, QuantityDTO.TemperatureUnit.Kelvin));
// //             Assert.AreEqual(273.15, r.Value, 0.01);
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – ADD
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Service_Add_Length_SameUnit()
// //         {
// //             var r = _service.Add(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
// //             Assert.AreEqual(3.0, r.Value, E);
// //             Assert.AreEqual(QuantityDTO.LengthUnit.Feet, r.LengthUnitValue);
// //         }

// //         [TestMethod]
// //         public void Service_Add_Length_CrossUnit_FeetPlusInch()
// //         {
// //             var r = _service.Add(
// //                 new QuantityDTO(1.0,  QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
// //             Assert.AreEqual(2.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Add_Length_WithTargetUnit_Yard()
// //         {
// //             var r = _service.Add(
// //                 new QuantityDTO(1.0,  QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch),
// //                 new QuantityDTO(0.0,  QuantityDTO.LengthUnit.Yard));
// //             Assert.AreEqual(0.6667, r.Value, 0.001);
// //             Assert.AreEqual(QuantityDTO.LengthUnit.Yard, r.LengthUnitValue);
// //         }

// //         [TestMethod]
// //         public void Service_Add_Weight_CrossUnit()
// //         {
// //             var r = _service.Add(
// //                 new QuantityDTO(1.0,    QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(1000.0, QuantityDTO.WeightUnit.Gram));
// //             Assert.AreEqual(2.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Add_Volume_WithTargetUnit()
// //         {
// //             var r = _service.Add(
// //                 new QuantityDTO(1.0,    QuantityDTO.VolumeUnit.Litre),
// //                 new QuantityDTO(1000.0, QuantityDTO.VolumeUnit.Millilitre),
// //                 new QuantityDTO(0.0,    QuantityDTO.VolumeUnit.Millilitre));
// //             Assert.AreEqual(2000.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Add_Temperature_Throws()
// //         {
// //             _service.Add(
// //                 new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
// //                 new QuantityDTO(50.0,  QuantityDTO.TemperatureUnit.Celsius));
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – SUBTRACT
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Service_Subtract_Length_SameUnit()
// //         {
// //             var r = _service.Subtract(
// //                 new QuantityDTO(5.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
// //             Assert.AreEqual(3.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Subtract_Length_WithTargetUnit_Inch()
// //         {
// //             var r = _service.Subtract(
// //                 new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(6.0, QuantityDTO.LengthUnit.Inch),
// //                 new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
// //             Assert.AreEqual(30.0, r.Value, E);   // 36in - 6in = 30in
// //         }

// //         [TestMethod]
// //         public void Service_Subtract_Weight_CrossUnit()
// //         {
// //             var r = _service.Subtract(
// //                 new QuantityDTO(2.0,   QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(500.0, QuantityDTO.WeightUnit.Gram));
// //             Assert.AreEqual(1.5, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Subtract_Volume_SameUnit()
// //         {
// //             var r = _service.Subtract(
// //                 new QuantityDTO(5.0, QuantityDTO.VolumeUnit.Litre),
// //                 new QuantityDTO(2.0, QuantityDTO.VolumeUnit.Litre));
// //             Assert.AreEqual(3.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Subtract_Temperature_Throws()
// //         {
// //             _service.Subtract(
// //                 new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
// //                 new QuantityDTO(50.0,  QuantityDTO.TemperatureUnit.Celsius));
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – DIVIDE
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Service_Divide_Length_SameUnit()
// //         {
// //             var r = _service.Divide(
// //                 new QuantityDTO(4.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
// //             Assert.AreEqual(2.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Divide_Length_CrossUnit()
// //         {
// //             // 2ft ÷ 12in  →  2ft ÷ 1ft  =  2
// //             var r = _service.Divide(
// //                 new QuantityDTO(2.0,  QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
// //             Assert.AreEqual(2.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Divide_Weight_SameUnit()
// //         {
// //             var r = _service.Divide(
// //                 new QuantityDTO(6.0, QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(2.0, QuantityDTO.WeightUnit.Kilogram));
// //             Assert.AreEqual(3.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Service_Divide_Volume_SameUnit()
// //         {
// //             var r = _service.Divide(
// //                 new QuantityDTO(4.0, QuantityDTO.VolumeUnit.Litre),
// //                 new QuantityDTO(2.0, QuantityDTO.VolumeUnit.Litre));
// //             Assert.AreEqual(2.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Divide_ByZero_Throws()
// //         {
// //             _service.Divide(
// //                 new QuantityDTO(5.0, QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(0.0, QuantityDTO.WeightUnit.Kilogram));
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Divide_Temperature_Throws()
// //         {
// //             _service.Divide(
// //                 new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
// //                 new QuantityDTO(50.0,  QuantityDTO.TemperatureUnit.Celsius));
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – cross-category rejection
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Add_CrossCategory_Throws()
// //         {
// //             _service.Add(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram));
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Subtract_CrossCategory_Throws()
// //         {
// //             _service.Subtract(
// //                 new QuantityDTO(1.0, QuantityDTO.VolumeUnit.Litre),
// //                 new QuantityDTO(1.0, QuantityDTO.TemperatureUnit.Celsius));
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – null safety
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Compare_NullFirst_Throws()
// //         {
// //             _service.Compare(null!, new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet));
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(QuantityMeasurementException))]
// //         public void Service_Add_NullSecond_Throws()
// //         {
// //             _service.Add(new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet), null!);
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(ArgumentNullException))]
// //         public void Service_NullRepository_ThrowsOnConstruct()
// //         {
// //             _ = new QuantityMeasurementServiceImpl(
// //                 new LengthService(), new WeightService(),
// //                 new VolumeService(), new TemperatureService(), null!);
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Service – every operation persists to repository
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Service_EachOperation_SavesOneEntityToRepo()
// //         {
// //             var d1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
// //             var d2 = new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch);

// //             _service.Compare(d1, d2);
// //             _service.Convert(d1, new QuantityDTO(0, QuantityDTO.LengthUnit.Inch));
// //             _service.Add(d1, d2);
// //             _service.Subtract(d2, d1);
// //             _service.Divide(d2, d1);

// //             Assert.AreEqual(5, _repo.Count);
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Controller – all PerformXxx methods
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void Controller_PerformCompare_Equal_Returns1()
// //         {
// //             var r = _controller.PerformCompare(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Controller_PerformConvert_FeetToInch()
// //         {
// //             var r = _controller.PerformConvert(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
// //             Assert.AreEqual(12.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Controller_PerformAdd_SameUnit()
// //         {
// //             var r = _controller.PerformAdd(
// //                 new QuantityDTO(2.0, QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(3.0, QuantityDTO.WeightUnit.Kilogram));
// //             Assert.AreEqual(5.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Controller_PerformAdd_WithTargetUnit()
// //         {
// //             var r = _controller.PerformAdd(
// //                 new QuantityDTO(1.0,  QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch),
// //                 new QuantityDTO(0.0,  QuantityDTO.LengthUnit.Yard));
// //             Assert.AreEqual(0.6667, r.Value, 0.001);
// //         }

// //         [TestMethod]
// //         public void Controller_PerformSubtract_SameUnit()
// //         {
// //             var r = _controller.PerformSubtract(
// //                 new QuantityDTO(5.0, QuantityDTO.VolumeUnit.Litre),
// //                 new QuantityDTO(2.0, QuantityDTO.VolumeUnit.Litre));
// //             Assert.AreEqual(3.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void Controller_PerformDivide_SameUnit()
// //         {
// //             var r = _controller.PerformDivide(
// //                 new QuantityDTO(6.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
// //             Assert.AreEqual(3.0, r.Value, E);
// //         }

// //         // ── Controller error-path: must NOT throw, must return NaN DTO ───────────────

// //         [TestMethod]
// //         public void Controller_PerformAdd_Temperature_DoesNotThrow_ReturnsNaN()
// //         {
// //             var r = _controller.PerformAdd(
// //                 new QuantityDTO(100.0, QuantityDTO.TemperatureUnit.Celsius),
// //                 new QuantityDTO(50.0,  QuantityDTO.TemperatureUnit.Celsius));
// //             Assert.IsTrue(double.IsNaN(r.Value));
// //         }

// //         [TestMethod]
// //         public void Controller_PerformCompare_CrossCategory_DoesNotThrow_ReturnsNaN()
// //         {
// //             var r = _controller.PerformCompare(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
// //                 new QuantityDTO(1.0, QuantityDTO.TemperatureUnit.Celsius));
// //             Assert.IsTrue(double.IsNaN(r.Value));
// //         }

// //         // ════════════════════════════════════════════════════════════════════════════
// //         // Layer separation / DIP
// //         // ════════════════════════════════════════════════════════════════════════════

// //         [TestMethod]
// //         public void LayerSeparation_ServiceWorksWithoutController()
// //         {
// //             var r = _service.Compare(
// //                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Yard),
// //                 new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         public void LayerSeparation_ControllerAcceptsDifferentServiceImpl()
// //         {
// //             var freshRepo = new UC15InMemoryRepository();
// //             var freshSvc  = new QuantityMeasurementServiceImpl(
// //                 new LengthService(), new WeightService(),
// //                 new VolumeService(), new TemperatureService(), freshRepo);
// //             var freshCtrl = new QuantityMeasurementController(freshSvc);

// //             var r = freshCtrl.PerformCompare(
// //                 new QuantityDTO(1.0,    QuantityDTO.WeightUnit.Kilogram),
// //                 new QuantityDTO(1000.0, QuantityDTO.WeightUnit.Gram));
// //             Assert.AreEqual(1.0, r.Value, E);
// //         }

// //         [TestMethod]
// //         [ExpectedException(typeof(ArgumentNullException))]
// //         public void Controller_NullService_ThrowsOnConstruct()
// //         {
// //             _ = new QuantityMeasurementController(null!);
// //         }
// //     }
// // }




// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using QuantityMeasurementApp.BusinessLayer;
// using QuantityMeasurementApp.RepositoryLayer;
// using QuantityMeasurementApp.RepositoryLayer.Interface;
// using QuantityMeasurementApp.BusinessLayer.Interface;
// using QuantityMeasurementApp.Core.Entities;
// using System;

// namespace QuantityMeasurementApp.Tests
// {
//     [TestClass]
//     public class NTierTests
//     {
//     private IQuantityMeasurementRepository _repository = null!;
//     private IQuantityMeasurementService _service = null!;

//         private const double epsilon = 0.0001;

//         [TestInitialize]
//         public void Setup()
//         {
//             _repository = new QuantityMeasurementRepository();
//             _service = new QuantityMeasurementService(_repository);
//         }

//         // -----------------------------
//         // Entity Tests
//         // -----------------------------

//         [TestMethod]
//         public void LengthEquality_FeetAndInch()
//         {
//             var l1 = new Length(1, LengthUnit.Feet);
//             var l2 = new Length(12, LengthUnit.Inch);

//             Assert.IsTrue(l1.Equals(l2));
//         }

//         [TestMethod]
//         public void WeightEquality_KgAndGram()
//         {
//             var w1 = new Weight(1, WeightUnit.Kilogram);
//             var w2 = new Weight(1000, WeightUnit.Gram);

//             Assert.IsTrue(w1.Equals(w2));
//         }

//         [TestMethod]
//         public void VolumeEquality_LitreAndMl()
//         {
//             var v1 = new Volume(1, VolumeUnit.Litre);
//             var v2 = new Volume(1000, VolumeUnit.Millilitre);

//             Assert.IsTrue(v1.Equals(v2));
//         }

//         // -----------------------------
//         // Repository Tests
//         // -----------------------------

//         [TestMethod]
//         public void Repository_Add_Length()
//         {
//             var l1 = new Length(1, LengthUnit.Feet);
//             var l2 = new Length(12, LengthUnit.Inch);

//             var result = _repository.Add(l1, l2);

//             Assert.AreEqual(2, result.Value, epsilon);
//         }

//         [TestMethod]
//         public void Repository_Subtract_Length()
//         {
//             var l1 = new Length(2, LengthUnit.Feet);
//             var l2 = new Length(12, LengthUnit.Inch);

//             var result = _repository.Subtract(l1, l2);

//             Assert.AreEqual(1, result.Value, epsilon);
//         }

//         [TestMethod]
//         public void Repository_Compare_Length()
//         {
//             var l1 = new Length(1, LengthUnit.Feet);
//             var l2 = new Length(12, LengthUnit.Inch);

//             var result = _repository.Compare(l1, l2);

//             Assert.IsTrue(result);
//         }

//         // -----------------------------
//         // Service Tests
//         // -----------------------------

//         [TestMethod]
//         public void Service_Add_Length()
//         {
//             var l1 = new Length(1, LengthUnit.Feet);
//             var l2 = new Length(12, LengthUnit.Inch);

//             var result = _service.Add(l1, l2);

//             Assert.AreEqual(2, result.Value, epsilon);
//         }

//         [TestMethod]
//         public void Service_Subtract_Length()
//         {
//             var l1 = new Length(2, LengthUnit.Feet);
//             var l2 = new Length(12, LengthUnit.Inch);

//             var result = _service.Subtract(l1, l2);

//             Assert.AreEqual(1, result.Value, epsilon);
//         }

//         [TestMethod]
//         public void Service_Compare_Length()
//         {
//             var l1 = new Length(1, LengthUnit.Feet);
//             var l2 = new Length(12, LengthUnit.Inch);

//             var result = _service.Compare(l1, l2);

//             Assert.IsTrue(result);
//         }

//         // -----------------------------
//         // Service Layer Dependency Test
//         // -----------------------------

//         [TestMethod]
//         public void Service_NullRepository_ShouldThrowException()
//         {
//             Assert.ThrowsException<ArgumentNullException>(() =>
//             {
//                 var service = new QuantityMeasurementService(null!);
//             });
//         }

//         // -----------------------------
//         // Cross Category Safety
//         // -----------------------------

//         [TestMethod]
//         public void LengthVsWeight_NotEqual()
//         {
//             var length = new Length(10, LengthUnit.Feet);
//             var weight = new Weight(10, WeightUnit.Kilogram);

//             Assert.IsFalse(length.Equals(weight));
//         }

//         [TestMethod]
//         public void LengthVsVolume_NotEqual()
//         {
//             var length = new Length(10, LengthUnit.Feet);
//             var volume = new Volume(10, VolumeUnit.Litre);

//             Assert.IsFalse(length.Equals(volume));
//         }
//     }
// }
