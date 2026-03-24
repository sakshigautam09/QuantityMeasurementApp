// ============================================================
// PROJECT : QuantityMeasurementApp.API
// FILE    : Controllers/QuantityMeasurementController.cs
// UC-17   : Redis-first reads. SQL Server is permanent backup.
// ============================================================

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.ModelLayer.DTO;
using QuantityMeasurementApp.RepositoryLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;
using QuantityMeasurementApp.RepositoryLayer.Services;

namespace QuantityMeasurementApp.API.Controllers
{
    [ApiController]
    [Route("api/v1/quantities")]
    [Authorize]
    [Produces("application/json")]
    public class QuantityMeasurementController : ControllerBase
    {
        private readonly IQuantityMeasurementService    _service;
        private readonly IQuantityMeasurementRepository _repo;
        private readonly IRedisCache                    _cache;
        private readonly IEncryptionService             _encryptor;
        private readonly ILogger<QuantityMeasurementController> _logger;

        public QuantityMeasurementController(
            IQuantityMeasurementService    service,
            IQuantityMeasurementRepository repo,
            IRedisCache                    cache,
            IEncryptionService             encryptor,
            ILogger<QuantityMeasurementController> logger)
        {
            _service   = service;
            _repo      = repo;
            _cache     = cache;
            _encryptor = encryptor;
            _logger    = logger;
        }

        private string CurrentUser =>
            User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue("unique_name")
            ?? "unknown";

        [HttpPost("compare")]
        [ProducesResponseType(typeof(QuantityResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public IActionResult Compare([FromBody] CompareRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = _service.Compare(QuantityMapper.ToDTO(req.First), QuantityMapper.ToDTO(req.Second));
                bool equal = result.Value == 1.0;
                _logger.LogInformation("[{User}] Compare = {Result}", CurrentUser, equal);
                return Ok(new QuantityResponse { Success = true, Operation = "Compare", ResultValue = equal.ToString(), ResultUnit = "" });
            }
            catch (QuantityMeasurementException ex) { return BadRequest(Err(400, "Compare Error", ex.Message)); }
            catch (ArgumentException ex)            { return BadRequest(Err(400, "Invalid Input", ex.Message)); }
        }

        [HttpPost("convert")]
        [ProducesResponseType(typeof(QuantityResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public IActionResult Convert([FromBody] ConvertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = _service.Convert(QuantityMapper.ToDTO(req.Source), QuantityMapper.ToUnitHint(req.TargetUnit));
                return Ok(new QuantityResponse { Success = true, Operation = "Convert", ResultValue = result.Value.ToString("G6"), ResultUnit = result.UnitLabel });
            }
            catch (QuantityMeasurementException ex) { return BadRequest(Err(400, "Convert Error", ex.Message)); }
            catch (ArgumentException ex)            { return BadRequest(Err(400, "Invalid Input", ex.Message)); }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(QuantityResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public IActionResult Add([FromBody] AddRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = req.TargetUnit is null
                    ? _service.Add(QuantityMapper.ToDTO(req.First), QuantityMapper.ToDTO(req.Second))
                    : _service.AddWithTargetUnit(QuantityMapper.ToDTO(req.First), QuantityMapper.ToDTO(req.Second), QuantityMapper.ToUnitHint(req.TargetUnit));
                return Ok(new QuantityResponse { Success = true, Operation = "Add", ResultValue = result.Value.ToString("G6"), ResultUnit = result.UnitLabel });
            }
            catch (QuantityMeasurementException ex) { return BadRequest(Err(400, "Add Error", ex.Message)); }
            catch (ArgumentException ex)            { return BadRequest(Err(400, "Invalid Input", ex.Message)); }
        }

        [HttpPost("subtract")]
        [ProducesResponseType(typeof(QuantityResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public IActionResult Subtract([FromBody] SubtractRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = req.TargetUnit is null
                    ? _service.Subtract(QuantityMapper.ToDTO(req.First), QuantityMapper.ToDTO(req.Second))
                    : _service.SubtractWithTargetUnit(QuantityMapper.ToDTO(req.First), QuantityMapper.ToDTO(req.Second), QuantityMapper.ToUnitHint(req.TargetUnit));
                return Ok(new QuantityResponse { Success = true, Operation = "Subtract", ResultValue = result.Value.ToString("G6"), ResultUnit = result.UnitLabel });
            }
            catch (QuantityMeasurementException ex) { return BadRequest(Err(400, "Subtract Error", ex.Message)); }
            catch (ArgumentException ex)            { return BadRequest(Err(400, "Invalid Input", ex.Message)); }
        }

        [HttpPost("divide")]
        [ProducesResponseType(typeof(QuantityResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public IActionResult Divide([FromBody] DivideRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = _service.Divide(QuantityMapper.ToDTO(req.First), QuantityMapper.ToDTO(req.Second));
                return Ok(new QuantityResponse { Success = true, Operation = "Divide", ResultValue = result.Value.ToString("G6"), ResultUnit = "" });
            }
            catch (QuantityMeasurementException ex) { return BadRequest(Err(400, "Divide Error", ex.Message)); }
            catch (ArgumentException ex)            { return BadRequest(Err(400, "Invalid Input", ex.Message)); }
        }

        /// <summary>Get all history. Redis first, SQL fallback.</summary>
        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<HistoryRecord>), 200)]
        public async Task<IActionResult> History()
        {
            var cached = await _cache.GetListAsync(RedisCacheService.AllHistoryKey);
            if (cached.Count > 0)
            {
                _logger.LogInformation("History from Redis ({Count}).", cached.Count);
                return Ok(cached.Select(MapDecrypt));
            }
            _logger.LogInformation("Redis empty — loading from SQL Server.");
            var fromDb = _repo.FindAll();
            foreach (var e in fromDb.Reverse())
                await _cache.PushToListAsync(RedisCacheService.AllHistoryKey, e);
            _logger.LogInformation("Redis repopulated ({Count}).", fromDb.Count);
            return Ok(fromDb.Select(MapDecrypt));
        }

        /// <summary>History by operation. Redis first.</summary>
        [HttpGet("history/operation/{op}")]
        [ProducesResponseType(typeof(IEnumerable<HistoryRecord>), 200)]
        public async Task<IActionResult> HistoryByOperation(string op)
        {
            if (!Enum.TryParse<QuantityMeasurementEntity.OperationType>(op, true, out var opType))
                return BadRequest(Err(400, "Invalid Operation", $"Use: Compare, Convert, Add, Subtract, Divide"));
            var cached = await _cache.GetListAsync(RedisCacheService.AllHistoryKey);
            if (cached.Count > 0)
                return Ok(cached.Where(e => e.Operation == opType).Select(MapDecrypt));
            return Ok(_repo.FindByOperation(opType).Select(MapDecrypt));
        }

        /// <summary>History by type. Redis first.</summary>
        [HttpGet("history/type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<HistoryRecord>), 200)]
        public async Task<IActionResult> HistoryByType(string type)
        {
            var cached = await _cache.GetListAsync(RedisCacheService.AllHistoryKey);
            if (cached.Count > 0)
                return Ok(cached.Where(e => e.FirstOperand.Type.ToString() == type).Select(MapDecrypt));
            return Ok(_repo.FindByMeasurementType(type).Select(MapDecrypt));
        }

        /// <summary>Statistics from SQL Server.</summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(StatisticsResponse), 200)]
        public IActionResult Statistics()
        {
            var byOp = new Dictionary<string, int>();
            foreach (QuantityMeasurementEntity.OperationType op in Enum.GetValues<QuantityMeasurementEntity.OperationType>())
                byOp[op.ToString()] = _repo.GetCountByOperation(op);
            return Ok(new StatisticsResponse { TotalRecords = _repo.GetTotalCount(), ErrorCount = _repo.GetErrorCount(), ByOperation = byOp });
        }

        /// <summary>Clear all records from Redis and SQL.</summary>
        [HttpDelete("clear")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Clear()
        {
            _repo.Clear();
            await _cache.ClearHistoryCacheAsync();
            _logger.LogWarning("[{User}] Cleared Redis + SQL.", CurrentUser);
            return NoContent();
        }

        private HistoryRecord MapDecrypt(QuantityMeasurementEntity r)
        {
            string plain = r.ResultDisplay;
            try { plain = _encryptor.Decrypt(r.ResultDisplay); } catch { }
            return new HistoryRecord
            {
                Id = r.Id, Timestamp = r.Timestamp,
                Operation = r.Operation.ToString(),
                MeasurementType = r.FirstOperand.Type.ToString(),
                FirstOperand = r.FirstOperand.ToString(),
                SecondOperand = r.SecondOperand?.ToString(),
                TargetUnit = r.TargetUnit?.UnitLabel,
                Result = plain, HasError = r.HasError, ErrorMessage = r.ErrorMessage
            };
        }

        private static ApiErrorResponse Err(int code, string error, string msg)
            => new() { StatusCode = code, Error = error, Message = msg };
    }
}