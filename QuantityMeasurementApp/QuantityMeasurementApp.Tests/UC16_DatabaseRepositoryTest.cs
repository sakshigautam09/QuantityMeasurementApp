// ============================================================
// UC16: MSTest tests for QuantityMeasurementDatabaseRepository
//
// Prerequisites:
//   1. SQL Server running (local or Docker).
//   2. db/schema.sql executed against QuantityMeasurementDB.
//   3. Connection string set in appsettings.json under
//      "QuantityMeasurementDb" key.
//
// Run:  dotnet test --filter "Category=Database"
// ============================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel;
using QuantityMeasurementRepository;
using QuantityMeasurementRepository.Database;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    [TestCategory("Database")]
    [DoNotParallelize]   // ← prevents interference between DB tests
    public class UC16_DatabaseRepositoryTest
    {
        private static QuantityMeasurementDatabaseRepository _repo = null!;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            _repo = new QuantityMeasurementDatabaseRepository();
        }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestInit() => _repo.Clear();   // clean slate before each test

        // ── Entity builders ──────────────────────────────────────────

        private static QuantityMeasurementEntity ConvertEntity()
            => new("CONVERT",
                   new QuantityDTO(1,  "FEET",   "LENGTH"),
                   new QuantityDTO(12, "INCHES", "LENGTH"));

        private static QuantityMeasurementEntity CompareEntity()
            => new("COMPARE",
                   new QuantityDTO(1,  "FEET",   "LENGTH"),
                   new QuantityDTO(12, "INCHES", "LENGTH"),
                   new QuantityDTO(1,  "BOOL",   "LENGTH"));

        private static QuantityMeasurementEntity AddEntity()
            => new("ADD",
                   new QuantityDTO(1,   "KILOGRAM", "WEIGHT"),
                   new QuantityDTO(500, "GRAM",     "WEIGHT"),
                   new QuantityDTO(1.5, "KILOGRAM", "WEIGHT"));

        // ── Save & GetAll ────────────────────────────────────────────

        [TestMethod]
        public void Save_SingleEntity_GetAllReturnsOne()
        {
            _repo.Save(ConvertEntity());
            IReadOnlyList<QuantityMeasurementEntity> all = _repo.GetAllMeasurements();
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual("CONVERT", all[0].OperationType);
        }

        [TestMethod]
        public void Save_MultipleEntities_GetAllReturnsAll()
        {
            _repo.Save(ConvertEntity());
            _repo.Save(CompareEntity());
            _repo.Save(AddEntity());
            Assert.AreEqual(3, _repo.GetAllMeasurements().Count);
        }

        [TestMethod]
        public void Save_NullEntity_ThrowsArgumentNullException()
            => Assert.Throws<ArgumentNullException>(() => _repo.Save(null!));

        // ── GetByOperation ───────────────────────────────────────────

        [TestMethod]
        public void GetByOperation_FilterConvert_ReturnsOnlyConvert()
        {
            _repo.Save(ConvertEntity());
            _repo.Save(CompareEntity());
            _repo.Save(AddEntity());

            IReadOnlyList<QuantityMeasurementEntity> result = _repo.GetByOperation("CONVERT");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("CONVERT", result[0].OperationType);
        }

        [TestMethod]
        public void GetByOperation_NoMatch_ReturnsEmpty()
        {
            _repo.Save(ConvertEntity());
            Assert.AreEqual(0, _repo.GetByOperation("SUBTRACT").Count);
        }

        // ── GetByCategory ────────────────────────────────────────────

        [TestMethod]
        public void GetByCategory_FilterLength_ReturnsLengthEntities()
        {
            _repo.Save(ConvertEntity());
            _repo.Save(CompareEntity());
            _repo.Save(AddEntity());   // WEIGHT

            IReadOnlyList<QuantityMeasurementEntity> result = _repo.GetByCategory("LENGTH");
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetByCategory_FilterWeight_ReturnsWeightEntities()
        {
            _repo.Save(ConvertEntity());
            _repo.Save(AddEntity());
            Assert.AreEqual(1, _repo.GetByCategory("WEIGHT").Count);
        }

        // ── GetTotalCount ────────────────────────────────────────────

        [TestMethod]
        public void GetTotalCount_AfterSavingThree_ReturnsThree()
        {
            _repo.Save(ConvertEntity());
            _repo.Save(CompareEntity());
            _repo.Save(AddEntity());
            Assert.AreEqual(3, _repo.GetTotalCount());
        }

        [TestMethod]
        public void GetTotalCount_EmptyTable_ReturnsZero()
            => Assert.AreEqual(0, _repo.GetTotalCount());

        // ── Clear ────────────────────────────────────────────────────

        [TestMethod]
        public void Clear_AfterSaving_CountBecomesZero()
        {
            _repo.Save(ConvertEntity());
            _repo.Save(AddEntity());
            _repo.Clear();
            Assert.AreEqual(0, _repo.GetTotalCount());
        }

        // ── Error entity round-trip ──────────────────────────────────

        [TestMethod]
        public void Save_ErrorEntity_RoundTripsCorrectly()
        {
            QuantityMeasurementEntity err =
                new("CONVERT", new QuantityDTO(1, "FEET", "LENGTH"), null, "Unsupported unit");
            _repo.Save(err);

            IReadOnlyList<QuantityMeasurementEntity> all = _repo.GetAllMeasurements();
            Assert.AreEqual(1, all.Count);
            Assert.IsTrue(all[0].HasError);
            Assert.AreEqual("Unsupported unit", all[0].ErrorMessage);
        }

        // ── SQL injection prevention ─────────────────────────────────

        [TestMethod]
        public void GetByOperation_SqlInjectionAttempt_TreatedAsLiteralValue()
        {
            _repo.Save(ConvertEntity());
            IReadOnlyList<QuantityMeasurementEntity> result =
                _repo.GetByOperation("' OR '1'='1");
            Assert.AreEqual(0, result.Count);
        }

        // ── Large dataset ────────────────────────────────────────────

        [TestMethod]
        public void Save_HundredEntities_AllRetrievedCorrectly()
        {
            for (int i = 0; i < 100; i++)
                _repo.Save(ConvertEntity());
            Assert.AreEqual(100, _repo.GetTotalCount());
        }
    }
}