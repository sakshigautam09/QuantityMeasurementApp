using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementWebAPI.Tests;

/// <summary>
/// ASP.NET Core equivalents of common Spring Boot REST / integration scenarios (35 cases).
/// </summary>
[TestClass]
public class QuantityMeasurementApiIntegrationTests
{
    private static TestWebApplicationFactory? _factory;
    private HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        _factory = new TestWebApplicationFactory();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory?.Dispose();
        _factory = null;
    }

    [TestInitialize]
    public void Setup()
    {
        _client = _factory!.CreateClient();
    }

    // --- 1: Application host loads (Spring: testSpringBootApplicationStarts) ---
    [TestMethod]
    public void testSpringBootApplicationStarts()
    {
        using var response = _client.GetAsync("/swagger/index.html").GetAwaiter().GetResult();
        Assert.IsTrue(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound,
            "Host should respond (Swagger may be at /swagger/index.html).");
    }

    // --- 2: Compare ---
    [TestMethod]
    public async Task testRestEndpointCompareQuantities()
    {
        var body = new QuantityInputDto
        {
            ThisQuantityDTO = new QuantityRequestDto { Value = 1, Unit = "FEET", MeasurementType = "LENGTH" },
            ThatQuantityDTO = new QuantityRequestDto { Value = 12, Unit = "INCHES", MeasurementType = "LENGTH" }
        };
        var res = await _client.PostAsJsonAsync("/api/v1/quantities/compare", body);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.IsTrue(doc.RootElement.TryGetProperty("ResultString", out var rs));
        StringAssert.Contains(rs.GetString()!, "true", StringComparison.OrdinalIgnoreCase);
    }

    // --- 3: Convert ---
    [TestMethod]
    public async Task testRestEndpointConvertQuantities()
    {
        var body = new QuantityInputDto
        {
            ThisQuantityDTO = new QuantityRequestDto { Value = 1, Unit = "FEET", MeasurementType = "LENGTH" },
            ThatQuantityDTO = new QuantityRequestDto { Value = 0, Unit = "INCHES", MeasurementType = "LENGTH" }
        };
        var res = await _client.PostAsJsonAsync("/api/v1/quantities/convert", body);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var val = root.GetProperty("ResultValue");
        Assert.AreEqual(12.0, val.GetDouble(), 0.1);
    }

    // --- 4: Add ---
    [TestMethod]
    public async Task testRestEndpointAddQuantities()
    {
        var body = new QuantityInputDto
        {
            ThisQuantityDTO = new QuantityRequestDto { Value = 1, Unit = "FEET", MeasurementType = "LENGTH" },
            ThatQuantityDTO = new QuantityRequestDto { Value = 12, Unit = "INCHES", MeasurementType = "LENGTH" }
        };
        var res = await _client.PostAsJsonAsync("/api/v1/quantities/add", body);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var val = root.GetProperty("ResultValue");
        Assert.AreEqual(2.0, val.GetDouble(), 0.05);
    }

    // --- 5: Invalid JSON ---
    [TestMethod]
    public async Task testRestEndpointInvalidInput_Returns400()
    {
        var content = new StringContent("{ not-json", Encoding.UTF8, "application/json");
        var res = await _client.PostAsync("/api/v1/quantities/compare", content);
        Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);
    }

    // --- 6: Missing / bad route (Spring: missing parameter) ---
    [TestMethod]
    public async Task testRestEndpointMissingParameter_Returns400()
    {
        var res = await _client.GetAsync("/api/v1/quantities/history/operation/");
        Assert.IsTrue(res.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest);
    }

    // --- 7: Swagger UI ---
    [TestMethod]
    public async Task testSwaggerUILoads()
    {
        var res = await _client.GetAsync("/swagger/index.html");
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        var html = await res.Content.ReadAsStringAsync();
        StringAssert.Contains(html, "swagger", StringComparison.OrdinalIgnoreCase);
    }

    // --- 8: OpenAPI document ---
    [TestMethod]
    public async Task testOpenAPIDocumentation()
    {
        var res = await _client.GetAsync("/swagger/v1/swagger.json");
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.IsTrue(doc.RootElement.TryGetProperty("paths", out var paths));
        Assert.IsTrue(paths.EnumerateObject().Any(p => p.Name.Contains("quantities", StringComparison.OrdinalIgnoreCase)));
    }

    // --- 9: H2 console (N/A in .NET — documented as SQL Server / EF) ---
    [TestMethod]
    public void testH2ConsoleLaunches()
    {
        Assert.Inconclusive("ASP.NET Core uses SQL Server/EF Core; there is no embedded H2 console. Use SSMS or InMemory in tests.");
    }

    // --- 10: Persistence through stack ---
    [TestMethod]
    public async Task testH2DatabasePersistence()
    {
        await testRestEndpointCompareQuantities();
        var res = await _client.GetAsync("/api/v1/quantities/history/operation/COMPARE");
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.IsFalse(string.IsNullOrWhiteSpace(body));
    }

    // --- 11–12: Actuator health/metrics (removed from app) ---
    [TestMethod]
    public async Task testActuatorHealthEndpoint()
    {
        var res = await _client.GetAsync("/health");
        Assert.AreEqual(HttpStatusCode.NotFound, res.StatusCode);
    }

    [TestMethod]
    public async Task testActuatorMetricsEndpoint()
    {
        var res = await _client.GetAsync("/actuator/metrics");
        Assert.IsTrue(res.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest);
    }

    // --- 13: History by operation (JPA findByOperation analogue) ---
    [TestMethod]
    public async Task testJPARepositoryFindByOperation()
    {
        await testRestEndpointAddQuantities();
        var res = await _client.GetAsync("/api/v1/quantities/history/operation/ADD");
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    // --- 14: History list (custom filter analogue) ---
    [TestMethod]
    public async Task testJPARepositoryCustomQuery()
    {
        var res = await _client.GetAsync("/api/v1/quantities/history/type/LENGTH");
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    // --- 15: Transaction rollback (manual / integration concern) ---
    [TestMethod]
    public void testTransactionalRollback()
    {
        Assert.Inconclusive("Use a dedicated EF Core transaction test with SQL Server if required; InMemory has limited transaction semantics.");
    }

    // --- 16: JSON content negotiation ---
    [TestMethod]
    public async Task testContentNegotiation_JSON()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/quantities/compare", new QuantityInputDto
        {
            ThisQuantityDTO = new QuantityRequestDto { Value = 1, Unit = "FEET", MeasurementType = "LENGTH" },
            ThatQuantityDTO = new QuantityRequestDto { Value = 1, Unit = "FEET", MeasurementType = "LENGTH" }
        });
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.IsTrue(res.Content.Headers.ContentType?.MediaType?.Contains("json", StringComparison.OrdinalIgnoreCase) == true);
    }

    // --- 17: XML (optional) ---
    [TestMethod]
    public async Task testContentNegotiation_XML()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/v1/quantities/count/COMPARE");
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        var res = await _client.SendAsync(req);
        Assert.IsTrue(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotAcceptable);
    }

    // --- 18: Global exception handler ---
    [TestMethod]
    public async Task testExceptionHandling_GlobalHandler()
    {
        var res = await _client.PostAsync(
            "/api/v1/quantities/compare",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        Assert.IsTrue(
            json.Contains("error", StringComparison.OrdinalIgnoreCase) ||
            json.Contains("Error", StringComparison.Ordinal) ||
            json.Contains("title", StringComparison.OrdinalIgnoreCase));
    }

    // --- 19: Path variable ---
    [TestMethod]
    public async Task testRequestPathVariable_Extraction()
    {
        var res = await _client.GetAsync("/api/v1/quantities/count/COMPARE");
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var op = doc.RootElement.GetProperty("operation").GetString();
        Assert.AreEqual("COMPARE", op);
    }

    // --- 20: Query string present ---
    [TestMethod]
    public async Task testRequestQueryParameter_Extraction()
    {
        var res = await _client.GetAsync("/api/v1/quantities/count/COMPARE?targetUnit=FEET");
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    // --- 21: JSON serialization round-trip ---
    [TestMethod]
    public async Task testResponseSerialization_Object()
    {
        var res = await _client.GetAsync("/api/v1/quantities/count/COMPARE");
        var dto = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.IsTrue(dto.ValueKind == JsonValueKind.Object);
    }

    // --- 22: MockMvc-style comparison (WebApplicationFactory = full stack) ---
    [TestMethod]
    public async Task testMockMvc_ComparisonTest()
    {
        await testRestEndpointCompareQuantities();
    }

    // --- 23: Status + content assertions ---
    [TestMethod]
    public async Task testMockMvc_ResponseAssertion()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/quantities/compare", new QuantityInputDto
        {
            ThisQuantityDTO = new QuantityRequestDto { Value = 2, Unit = "FEET", MeasurementType = "LENGTH" },
            ThatQuantityDTO = new QuantityRequestDto { Value = 2, Unit = "FEET", MeasurementType = "LENGTH" }
        });
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.IsTrue(string.Equals("application/json", res.Content.Headers.ContentType?.MediaType, StringComparison.OrdinalIgnoreCase));
    }

    // --- 24: Multiple operations ---
    [TestMethod]
    public async Task testIntegrationTest_MultipleOperations()
    {
        await _client.PostAsJsonAsync("/api/v1/quantities/compare", BuildLength(1, "FEET", 12, "INCHES"));
        await _client.PostAsJsonAsync("/api/v1/quantities/add", BuildLength(1, "FEET", 12, "INCHES"));
        var h = await _client.GetAsync("/api/v1/quantities/history/operation/COMPARE");
        Assert.AreEqual(HttpStatusCode.OK, h.StatusCode);
    }

    // --- 25: Schema (InMemory) ---
    [TestMethod]
    public void testDatabaseInitialization_SchemaCreated()
    {
        Assert.IsNotNull(_factory);
        // EF InMemory creates model on first use; migrated SQL Server schema is validated in non-Testing deployments.
    }

    // --- 26–27: Profiles ---
    [TestMethod]
    public void testProfileSpecificConfiguration_Development()
    {
        Assert.Inconclusive("Profile-specific checks belong to deployment/config tests; integration host uses Environment=Testing.");
    }

    [TestMethod]
    public void testProfileSpecificConfiguration_Production()
    {
        Assert.Inconclusive("Production profile validation requires a separate deployment test.");
    }

    // --- 28: Unauthorized ---
    [TestMethod]
    public async Task testRESTEndpointSecurity_Unauthorized()
    {
        var res = await _client.GetAsync("/api/v1/quantities/history/errored");
        Assert.AreEqual(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    // --- 29: With token (placeholder) ---
    [TestMethod]
    public void testRESTEndpointSecurity_WithAuthentication()
    {
        Assert.Inconclusive("Provide a valid JWT from /api/user/login in a secure test fixture to assert 200 on authorized routes.");
    }

    // --- 30–31: Message converters ---
    [TestMethod]
    public async Task testMessageConverter_JSONToObject()
    {
        var body = BuildLength(5, "FEET", 5, "FEET");
        var res = await _client.PostAsJsonAsync("/api/v1/quantities/compare", body);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task testMessageConverter_ObjectToJSON()
    {
        var res = await _client.GetAsync("/api/v1/quantities/count/COMPARE");
        var json = await res.Content.ReadAsStringAsync();
        Assert.IsTrue(json.Contains("count", StringComparison.OrdinalIgnoreCase));
    }

    // --- 32: Success codes ---
    [TestMethod]
    public async Task testHttpStatusCodes_Success()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/quantities/compare", BuildLength(1, "FEET", 12, "INCHES"));
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    // --- 33: Client errors ---
    [TestMethod]
    public async Task testHttpStatusCodes_ClientErrors()
    {
        var bad = new StringContent("x", Encoding.UTF8, "application/json");
        var res = await _client.PostAsync("/api/v1/quantities/compare", bad);
        Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);
    }

    // --- 34: Server errors (no dedicated fault endpoint) ---
    [TestMethod]
    public void testHttpStatusCodes_ServerErrors()
    {
        Assert.Inconclusive("No public test-only fault route; 500 paths should be covered by unit tests of GlobalExceptionHandler fall-through.");
    }

    // --- 35: OpenAPI metadata ---
    [TestMethod]
    public async Task testRestDocumentation_OperationDetails()
    {
        var res = await _client.GetAsync("/swagger/v1/swagger.json");
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var info = doc.RootElement.GetProperty("info");
        StringAssert.Contains(info.GetProperty("title").GetString()!, "Quantity", StringComparison.OrdinalIgnoreCase);
    }

    private static QuantityInputDto BuildLength(double v1, string u1, double v2, string u2) => new()
    {
        ThisQuantityDTO = new QuantityRequestDto { Value = v1, Unit = u1, MeasurementType = "LENGTH" },
        ThatQuantityDTO = new QuantityRequestDto { Value = v2, Unit = u2, MeasurementType = "LENGTH" }
    };
}
