using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;
using System.Security.Claims;

namespace QuantityMeasurementWebAPI.Controllers
{
    /// <summary>
    /// UC17: Quantity operations controller.
    /// All operations save to SQL Server (SSMS) via EF Core.
    /// History reads served from Redis cache (5-min TTL).
    /// </summary>
    [ApiController]
    [Route("api/v1/quantities")]
    [Produces("application/json")]
    public class QuantitiesController : ControllerBase
    {
        private readonly IQuantityMeasurementWebService _svc;
        private readonly ILogger<QuantitiesController> _logger;

        public QuantitiesController(IQuantityMeasurementWebService svc, ILogger<QuantitiesController> logger)
        { _svc = svc; _logger = logger; }

        private int? UserId()
        {
            var c = User.FindFirst(ClaimTypes.NameIdentifier);
            return c is null ? null : int.TryParse(c.Value, out int id) ? id : null;
        }

        /// <summary>Compare two quantities. Returns resultString "true" / "false".</summary>
        [HttpPost("compare")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<IActionResult> Compare([FromBody] QuantityInputDto input)
        {
            _logger.LogInformation("[API] Compare");
            return Ok(await _svc.CompareAsync(input.ThisQuantityDTO, input.ThatQuantityDTO!, UserId()));
        }

        /// <summary>Convert a quantity to a different unit. ThatQuantityDTO.Unit = target unit.</summary>
        [HttpPost("convert")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), 200)]
        public async Task<IActionResult> Convert([FromBody] QuantityInputDto input)
            => Ok(await _svc.ConvertAsync(input.ThisQuantityDTO, input.ThatQuantityDTO!, UserId()));

        /// <summary>Add two quantities. Result unit = first operand unit.</summary>
        [HttpPost("add")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), 200)]
        public async Task<IActionResult> Add([FromBody] QuantityInputDto input)
            => Ok(await _svc.AddAsync(input.ThisQuantityDTO, input.ThatQuantityDTO!, UserId()));

        /// <summary>Subtract second quantity from first.</summary>
        [HttpPost("subtract")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), 200)]
        public async Task<IActionResult> Subtract([FromBody] QuantityInputDto input)
            => Ok(await _svc.SubtractAsync(input.ThisQuantityDTO, input.ThatQuantityDTO!, UserId()));

        /// <summary>Divide first quantity by second. Returns dimensionless scalar ratio.</summary>
        [HttpPost("divide")]
        [ProducesResponseType(typeof(QuantityMeasurementDto), 200)]
        public async Task<IActionResult> Divide([FromBody] QuantityInputDto input)
            => Ok(await _svc.DivideAsync(input.ThisQuantityDTO, input.ThatQuantityDTO!, UserId()));

        /// <summary>History by operation — Redis cached (5-min TTL), falls back to SQL Server.</summary>
        [HttpGet("history/operation/{operation}")]
        [ProducesResponseType(typeof(IReadOnlyList<QuantityMeasurementDto>), 200)]
        public async Task<IActionResult> ByOperation(string operation)
            => Ok(await _svc.GetHistoryByOperationAsync(operation.ToUpperInvariant()));

        /// <summary>History by measurement type (LENGTH/WEIGHT/VOLUME/TEMPERATURE).</summary>
        [HttpGet("history/type/{measurementType}")]
        [ProducesResponseType(typeof(IReadOnlyList<QuantityMeasurementDto>), 200)]
        public async Task<IActionResult> ByType(string measurementType)
            => Ok(await _svc.GetHistoryByCategoryAsync(measurementType.ToUpperInvariant()));

        /// <summary>Error history — requires JWT authentication.</summary>
        [HttpGet("history/errored")]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<QuantityMeasurementDto>), 200)]
        public async Task<IActionResult> Errored()
            => Ok(await _svc.GetErrorHistoryAsync());

        /// <summary>Count successful operations for a given operation type.</summary>
        [HttpGet("count/{operation}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> Count(string operation)
            => Ok(new { operation = operation.ToUpperInvariant(),
                        count     = await _svc.GetCountByOperationAsync(operation) });
    }
}
