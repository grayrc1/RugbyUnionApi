namespace RugbyUnionApi.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

public class Tests
{
    private ModelsDb _db;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ModelsDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new ModelsDb(options);
        _db.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }

    [Fact]
    public async Task TestRootEndpoint()
        {
        await using var application = new WebApplicationFactory<Program>();
        using var client  = application.CreateClient();

        var response = await client.GetStringAsync("/");

        Assert.AreEqual("Player Club Signing Application", response);
    }
    
    [Fact]
    public async Task GetAllPlayers_Returns_Players()
    {
        // Arrange
        await using var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();

        // Act
        var result = await client.GetAsync("/players");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

        var content = await result.Content.ReadAsStringAsync();
        var players = JsonSerializer.Deserialize<List<Player>>(content);

        Assert.NotNull(players);
    }

    [Fact]
    public async Task GetPlayer_Returns_SinglePlayer()
    {
        // Arrange
        await using var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();
        var player = new Player
        {
            Name = "John Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Height = 180,
            Weight = 80,
            PlaceOfBirth = "New York"
        };
        var createResult = await client.PostAsJsonAsync("/players", player);
        var createdPlayer = await createResult.Content.ReadFromJsonAsync<Player>();

        // Act
        var result = await client.GetAsync($"/players/{createdPlayer.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

        var content = await result.Content.ReadAsStringAsync();
        var returnedPlayer = JsonSerializer.Deserialize<Player>(
            content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(returnedPlayer);
        
        Assert.AreEqual(createdPlayer.Id, returnedPlayer.Id);
        Assert.AreEqual(createdPlayer.Name, returnedPlayer.Name);
    }

    [Fact]
    public async Task CreatePlayer_Creates_NewPlayer()
    {
        // Arrange
        await using var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();
        var player = new Player
        {
            Name = "John Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Height = 180,
            Weight = 80,
            PlaceOfBirth = "New York"
        };

        // Act
        var result = await client.PostAsJsonAsync("/players", player);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

        var content = await result.Content.ReadAsStringAsync();
        var createdPlayer = JsonSerializer.Deserialize<Player>(
            content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(createdPlayer);
        Assert.AreEqual(player.Name, createdPlayer.Name);
        Assert.AreEqual(player.BirthDate, createdPlayer.BirthDate);
        Assert.AreEqual(player.Height, createdPlayer.Height);
        Assert.AreEqual(player.Weight, createdPlayer.Weight);
        Assert.AreEqual(player.PlaceOfBirth, createdPlayer.PlaceOfBirth);
    }

    [Fact]
    public async Task UpdatePlayer_Updates_Player()
    {
        // Arrange
        await using var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();
        var player = new Player
        {
            Name = "John Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Height = 180,
            Weight = 80,
            PlaceOfBirth = "New York"
        };
        var createResult = await client.PostAsJsonAsync("/players", player);
        var createdPlayer = await createResult.Content.ReadFromJsonAsync<Player>();
        var updatedPlayer = new Player
        {
            Name = "Jane Doe",
            BirthDate = new DateTime(1995, 1, 1),
            Height = 170,
            Weight = 60,
            PlaceOfBirth = "Los Angeles",
            TeamId = 1
        };

        // Act
        var result = await client.PutAsJsonAsync($"/players/{createdPlayer.Id}", updatedPlayer);

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);

        var getResult = await client.GetAsync($"/players/{createdPlayer.Id}");
        var content = await getResult.Content.ReadAsStringAsync();
        var returnedPlayer = JsonSerializer.Deserialize<Player>(
            content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.AreEqual(updatedPlayer.Name, returnedPlayer.Name);
        Assert.AreEqual(updatedPlayer.BirthDate, returnedPlayer.BirthDate);
    }
}
