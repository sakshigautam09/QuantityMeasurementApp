// ============================================================
// PROJECT : QuantityMeasurementApp.API
// FILE    : Controllers/UserController.cs
// UC-17   : Authentication endpoints — renamed from AuthController.
//           Logic lives in BusinessLayer (mentor's rule).
//
//  POST /api/auth/register  — Sign Up
//  POST /api/auth/login     — Sign In
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.BusinessLayer.Interface;  
using QuantityMeasurementApp.ModelLayer.DTO;           

namespace QuantityMeasurementApp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService            _auth;
        private readonly ILogger<UserController> _logger;

        public UserController(IAuthService auth, ILogger<UserController> logger)
        {
            _auth   = auth;
            _logger = logger;
        }

        // ── POST /api/auth/register ───────────────────────────────────────────────
        /// <summary>
        /// Register a new user (Sign Up).
        /// Password is hashed with BCrypt before storage.
        /// Returns a JWT token — use as: Authorization: Bearer {token}
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/register
        ///     {
        ///         "username": "john_doe",
        ///         "email":    "john@example.com",
        ///         "password": "SecurePass123"
        ///     }
        ///
        /// </remarks>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse),     StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _auth.RegisterAsync(req);

            if (!result.Success)
                return BadRequest(new ApiErrorResponse
                {
                    StatusCode = 400,
                    Error      = "Registration Failed",
                    Message    = result.Message
                });

            return StatusCode(201, result);
        }

        // ── POST /api/auth/login ──────────────────────────────────────────────────
        /// <summary>
        /// Login with username and password (Sign In).
        /// BCrypt verifies password against stored hash.
        /// Returns a JWT token — stateless, no session stored on server.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/login
        ///     {
        ///         "username": "john_doe",
        ///         "password": "SecurePass123"
        ///     }
        ///
        /// Use the token: Authorization: Bearer eyJhbGci...
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse),     StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _auth.LoginAsync(req);

            if (!result.Success)
                return Unauthorized(new ApiErrorResponse
                {
                    StatusCode = 401,
                    Error      = "Authentication Failed",
                    Message    = result.Message
                });

            return Ok(result);
        }
    }
}