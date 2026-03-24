// // ============================================================
// // PROJECT : QuantityMeasurementApp.Tests
// // FILE    : UC16Tests.cs
// //
// // Requires SQL Server running with QuantityMeasurementDb.
// // Set connection string via environment variable if needed:
// //   QMA_TEST_CONNECTION_STRING
// // ============================================================

// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using QuantityMeasurementApp.BusinessLayer;
// using QuantityMeasurementApp.Core.Services;
// using QuantityMeasurementApp.ModelLayer;
// using QuantityMeasurementApp.RepositoryLayer;
// using QuantityMeasurementApp.RepositoryLayer.Interface;

// using System;

// namespace QuantityMeasurementApp.Tests
// {
//     [TestClass]
//     public class DatabaseIntegrationTests
//     {
//         private IQuantityMeasurementRepository _repo    = null!;
//         private IQuantityMeasurementService    _service = null!;
//         private QuantityMeasurementController  _controller = null!;

//         private static readonly string ConnectionString =
//             Environment.GetEnvironmentVariable("QMA_TEST_CONNECTION_STRING")
//             ?? "Server=localhost\\SQLEXPRESS;Database=QuantityMeasurementDb;Trusted_Connection=True;TrustServerCertificate=True;";

//         [TestInitialize]
//         public void Setup()
//         {
//             _repo = new QuantityMeasurementDatabaseRepository(ConnectionString);
//             _repo.Clear();

//             _service = new QuantityMeasurementServiceImpl(
//                 new QuantityModelServiceImpl(),
//                 new TemperatureService(),
//                 _repo);

//             _controller = new QuantityMeasurementController(_service, _repo);
//         }

//         [TestCleanup]
//         public void Cleanup()
//         {
//             _repo.Clear();
//             _repo.ReleaseResources();
//         }

//         // ── Save to DB ────────────────────────────────────────────────────────────

//         [TestMethod]
//         public void Compare_SavedToDatabase()
//         {
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Convert_SavedToDatabase()
//         {
//             _service.Convert(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Add_SavedToDatabase()
//         {
//             _service.Add(
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Subtract_SavedToDatabase()
//         {
//             _service.Subtract(
//                 new QuantityDTO(5.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Divide_SavedToDatabase()
//         {
//             _service.Divide(
//                 new QuantityDTO(10.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void ErrorOperation_SavedWithHasErrorTrue()
//         {
//             try
//             {
//                 _service.Divide(
//                     new QuantityDTO(10.0, QuantityDTO.LengthUnit.Feet),
//                     new QuantityDTO(0.0, QuantityDTO.LengthUnit.Feet));
//             }
//             catch (QuantityMeasurementException) { }

//             Assert.AreEqual(1, _repo.GetErrorCount());
//         }

//         // ── FindAll (option 7) ────────────────────────────────────────────────────

//         [TestMethod]
//         public void FindAll_After3Operations_Returns3()
//         {
//             var l1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
//             var l2 = new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet);
//             _service.Compare(l1, l2);
//             _service.Add(l1, l2);
//             _service.Subtract(l2, l1);
//             Assert.AreEqual(3, _repo.FindAll().Count);
//         }

//         [TestMethod]
//         public void FindAll_EmptyDB_ReturnsEmpty()
//         {
//             Assert.AreEqual(0, _repo.FindAll().Count);
//         }

//         // ── FindByOperation (option 8) ────────────────────────────────────────────

//         [TestMethod]
//         public void FindByOperation_Compare_ReturnsOnlyCompareRecords()
//         {
//             var l1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
//             var l2 = new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet);
//             _service.Compare(l1, l2);
//             _service.Compare(l1, l2);
//             _service.Add(l1, l2);

//             var result = _repo.FindByOperation(QuantityMeasurementEntity.OperationType.Compare);
//             Assert.AreEqual(2, result.Count);
//         }

//         [TestMethod]
//         public void FindByOperation_NoMatch_ReturnsEmpty()
//         {
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));

//             var result = _repo.FindByOperation(QuantityMeasurementEntity.OperationType.Divide);
//             Assert.AreEqual(0, result.Count);
//         }

//         // ── FindByMeasurementType (option 9) ─────────────────────────────────────

//         [TestMethod]
//         public void FindByMeasurementType_Length_ReturnsOnlyLength()
//         {
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
//                 new QuantityDTO(2.0, QuantityDTO.WeightUnit.Kilogram));

//             Assert.AreEqual(1, _repo.FindByMeasurementType("Length").Count);
//         }

//         [TestMethod]
//         public void FindByMeasurementType_Weight_ReturnsOnlyWeight()
//         {
//             _service.Add(
//                 new QuantityDTO(1.0, QuantityDTO.WeightUnit.Kilogram),
//                 new QuantityDTO(2.0, QuantityDTO.WeightUnit.Kilogram));
//             _service.Add(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));

//             Assert.AreEqual(1, _repo.FindByMeasurementType("Weight").Count);
//         }

//         // ── Statistics (option 10) ────────────────────────────────────────────────

//         [TestMethod]
//         public void GetTotalCount_EmptyDB_ReturnsZero()
//         {
//             Assert.AreEqual(0, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void GetCountByOperation_ReturnsCorrectCount()
//         {
//             var l1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
//             var l2 = new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet);
//             _service.Compare(l1, l2);
//             _service.Compare(l1, l2);
//             _service.Add(l1, l2);

//             Assert.AreEqual(2, _repo.GetCountByOperation(
//                 QuantityMeasurementEntity.OperationType.Compare));
//             Assert.AreEqual(1, _repo.GetCountByOperation(
//                 QuantityMeasurementEntity.OperationType.Add));
//         }

//         [TestMethod]
//         public void GetErrorCount_NoErrors_ReturnsZero()
//         {
//             _service.Add(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(0, _repo.GetErrorCount());
//         }

//         // ── Clear (option 11) ─────────────────────────────────────────────────────

//         [TestMethod]
//         public void Clear_RemovesAllRecords()
//         {
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             _service.Add(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));

//             _repo.Clear();

//             Assert.AreEqual(0, _repo.GetTotalCount());
//         }

//         // ── Data persists ─────────────────────────────────────────────────────────

//         [TestMethod]
//         public void DataPersists_AfterNewRepositoryInstance()
//         {
//             _service.Compare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));

//             var newRepo = new QuantityMeasurementDatabaseRepository(ConnectionString);
//             Assert.AreEqual(1, newRepo.GetTotalCount());
//         }

//         // ── Controller saves to DB ────────────────────────────────────────────────

//         [TestMethod]
//         public void Controller_PerformCompare_SavesToDB()
//         {
//             _controller.PerformCompare(
//                 new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(12.0, QuantityDTO.LengthUnit.Inch));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Controller_PerformAdd_SavesToDB()
//         {
//             _controller.PerformAdd(
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(3.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Controller_PerformSubtract_SavesToDB()
//         {
//             _controller.PerformSubtract(
//                 new QuantityDTO(5.0, QuantityDTO.WeightUnit.Kilogram),
//                 new QuantityDTO(2.0, QuantityDTO.WeightUnit.Kilogram));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Controller_PerformDivide_SavesToDB()
//         {
//             _controller.PerformDivide(
//                 new QuantityDTO(10.0, QuantityDTO.LengthUnit.Feet),
//                 new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet));
//             Assert.AreEqual(1, _repo.GetTotalCount());
//         }

//         [TestMethod]
//         public void Controller_AllOperations_AllSavedToDB()
//         {
//             var l1 = new QuantityDTO(1.0, QuantityDTO.LengthUnit.Feet);
//             var l2 = new QuantityDTO(2.0, QuantityDTO.LengthUnit.Feet);

//             _controller.PerformCompare(l1, l2);
//             _controller.PerformAdd(l1, l2);
//             _controller.PerformSubtract(l2, l1);
//             _controller.PerformDivide(l2, l1);
//             _controller.PerformConvert(l1,
//                 new QuantityDTO(0.0, QuantityDTO.LengthUnit.Inch));

//             Assert.AreEqual(5, _repo.GetTotalCount());
//         }
//     }
// }