using System.Net;
using System.Net.Http.Json;
using DanubeRS.DBCParser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DanubeRS.CANServer.Web.Tests;

public class DbcApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DbcApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static readonly string TestDbc = """
        VERSION ""

        NS_ :

        BS_:

        BU_: Vector

        BO_ 1 TestMsg: 8 Vector
         SG_ Unsigned8 : 0|8@1+ (1,0) [0|255] "" Vector
         SG_ Unsigned16 : 8|16@1+ (1,0) [0|65535] "" Vector


        """;

    [Fact]
    public async Task LoadDbc_ValidFile_ReturnsOk()
    {
        using var content = new MultipartFormDataContent();
        var dbcBytes = System.Text.Encoding.UTF8.GetBytes(TestDbc);
        content.Add(new ByteArrayContent(dbcBytes), "file", "test.dbc");

        var response = await _client.PostAsync("/api/dbc/load", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoadDbc_ThenGetMessages_ReturnsMessages()
    {
        var editService = _factory.Services.GetRequiredService<DbcEditService>();
        editService.LoadDbc(TestDbc);

        var response = await _client.GetAsync("/api/dbc/messages");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMessages_AfterLoad_ReturnsOk()
    {
        var editService = _factory.Services.GetRequiredService<DbcEditService>();
        if (!editService.IsLoaded)
            editService.LoadDbc(TestDbc);

        var response = await _client.GetAsync("/api/dbc/messages");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ExportDbc_AfterLoad_ReturnsFile()
    {
        var editService = _factory.Services.GetRequiredService<DbcEditService>();
        editService.LoadDbc(TestDbc);

        var response = await _client.GetAsync("/api/dbc/export");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
    }
}