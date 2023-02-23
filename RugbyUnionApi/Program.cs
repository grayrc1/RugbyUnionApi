/* *
 * 
 * Date:        2023-02-23
 * Author:      Richard Gray
 * Description: Sets up a web API using ASP.NET Core Framework.
 *              The API has endpoints for managing team and player data for a fictional rugby union.
 *              It uses an in memory "database" which it populates on start up with random data.
 * */

using Microsoft.EntityFrameworkCore;
using RugbyUnionApi;
using System.Text.Json;

const int NPlayers = 90; //number of random dummy players created on startup
const int NTeams = 6; //number of random teams
const int Teamsize = 15; //number of players per team

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ModelsDb>(opt => opt.UseInMemoryDatabase("ModelList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();


//map get, post, put, and delete requests to corresponding endpoints
var playerItems = app.MapGroup("/players");//end points are grouped into player and team items

playerItems.MapGet("/", GetAllPlayers); //gets a list of all players
playerItems.MapGet("/{id}", GetPlayer); //get a single player with the given ID
playerItems.MapPost("/", CreatePlayer); //creates new player
playerItems.MapPut("/{id}", UpdatePlayer); //updates player with given ID
playerItems.MapDelete("/{id}", DeletePlayer); //deletes player with given ID
playerItems.MapGet("/playerteam/{id}", GetPlayerTeam); //gets the team of a given player (by id)
playerItems.MapGet("/getage/{age}", GetAge); //gets all players of a given age

var teamItems = app.MapGroup("/teams");

teamItems.MapGet("/", GetAllTeams); //gets list of all teams
teamItems.MapGet("/{id}", GetTeam); //get team with given ID
teamItems.MapPost("/", CreateTeam); //create new team
teamItems.MapPut("/{id}", UpdateTeam); //update team with id
teamItems.MapDelete("/{id}", DeleteTeam); //delete team with id
teamItems.MapGet("/getcoachplayers/{coachname}", GetCoachPlayers); //get all players who are coached by given coach

app.MapGet("/", async context =>
    await context.Response.WriteAsync("Player Club Signing Application"));//maps title message to root

using (var db = new ModelsDb(new DbContextOptionsBuilder<ModelsDb>() //seeds in memory database with dummy data
    .UseInMemoryDatabase(databaseName: "ModelList")
    .Options))
{
    CreateDummyData(db); 
}

app.Run();

//Methods for performing database requests
static async Task<IResult> GetAllPlayers(ModelsDb db)
{
    return TypedResults.Ok(await db.Players.ToListAsync());
}

static async Task<IResult> GetPlayer(int id, ModelsDb db)
{
    return await db.Players.FindAsync(id)
        is Player player ? TypedResults.Ok(player) : TypedResults.NotFound();
}

static async Task<IResult> CreatePlayer(Player player, ModelsDb db)
{
    db.Players.Add(player);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/players/{player.Id}", player);
}

static async Task<IResult> UpdatePlayer(int id, Player inputPlayer, ModelsDb db)
{
    var player = await db.Players.FindAsync(id);

    if (player == null) return TypedResults.NotFound();

    player.Name = inputPlayer.Name;
    player.BirthDate = inputPlayer.BirthDate;
    player.Height = inputPlayer.Height;
    player.Weight = inputPlayer.Weight;
    player.PlaceOfBirth = inputPlayer.PlaceOfBirth;

    var team = await db.Teams.FindAsync(inputPlayer.TeamId)
        ?? throw new ArgumentException($"Team with id {inputPlayer.TeamId} does not exist");
    
    player.TeamId = team.Id;

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeletePlayer(int id, ModelsDb db)
{
    if (await db.Players.FindAsync(id) is Player player)
    {
        db.Players.Remove(player);
        await db.SaveChangesAsync ();
    }
    return TypedResults.NotFound();
}

static async Task<IResult> GetPlayerTeam(int id, ModelsDb db)
{
    var player = await db.Players.FindAsync(id);
    if (player == null) return TypedResults.NotFound();
    var team = await db.Teams.FindAsync(player.TeamId);
    if (team == null) return TypedResults.NotFound();
    return TypedResults.Ok(team);
}

static async Task<IResult> GetAge(int age, ModelsDb db)
{
    var players = await db.Players.Where(
        p => DateTime.Now.Year - p.BirthDate.Year == age).ToListAsync();
    return TypedResults.Ok(players);
}

static async Task<IResult> GetAllTeams(ModelsDb db)
{
    return TypedResults.Ok(await db.Teams.Include(t => t.Players).ToListAsync());
}

static async Task<IResult> GetTeam(int id, ModelsDb db)
{
    return await db.Teams.Include(t => t.Players)
        .FirstOrDefaultAsync(t => t.Id == id)
        is Team team ? Results.Ok(team) : Results.NotFound();
}

static async Task<IResult> CreateTeam(Team team, ModelsDb db)
{
    db.Teams.Add(team);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/teams/{team.Id}", team);
}

static async Task<IResult> UpdateTeam(int id, Team inputTeam, ModelsDb db)
{
    var team = await db.Teams.FindAsync(id);

    if (team == null) return TypedResults.NotFound();

    team.Name = inputTeam.Name;
    team.Ground = inputTeam.Ground;
    team.Coach = inputTeam.Coach;
    team.FoundedYear = inputTeam.FoundedYear;
    team.Region = inputTeam.Region;

    if (team.Players != null && team.Players.Any())
    {
        var playerIds = team.Players.Select(x => x.Id).ToList();
        var players = db.Players.Where(p => playerIds.Contains(p.Id)).ToList();

        if (players.Count != playerIds.Count)
        {
            throw new ArgumentException($"One or more players do not exist.");
        }

        team.Players.Clear();
        foreach (Player p in players)
        {
            team.Players.Add(p);
        }
    }

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTeam(int id, ModelsDb db)
{
    if (await db.Teams.FindAsync(id) is Team team)
    {
        db.Teams.Remove(team);
        await db.SaveChangesAsync();
    }
    return TypedResults.NotFound();
}

static async Task<IResult> GetCoachPlayers(string coachName, ModelsDb db)
{
    var teams = await db.Teams.Where(t => t.Coach == coachName)
        .SelectMany(t => t.Players).ToListAsync();
    return TypedResults.Ok(teams);
}

//Create random players and teams for diagnostics etc
static void CreateDummyData(ModelsDb db)
{
    var random = new Random();
    var players = new List<Player>();
    for (int i = 1; i <= NPlayers; i++)
    {
        var player = new Player
        {
            Id = i,
            Name = "Player " + i,
            BirthDate = DateTime.Now.AddYears(-random.Next(18, 40)).AddDays(-random.Next(0, 365)),
            Height = random.Next(170, 210),
            Weight = random.Next(60, 100),
            PlaceOfBirth = "City " + random.Next(1, 10),
            TeamId = null // Player not signed with any team yet
        };
        db.Players.Add(player);
        players.Add(player);
    }
    for (int i = 1; i <= NTeams; i++)
    {
        var team = new Team
        {
            Id = i,
            Name = "Team " + i,
            Ground = "Stadium " + random.Next(1, 5),
            Coach = "Coach " + random.Next(1, 10),
            FoundedYear = 1900 + random.Next(0, 122),
            Region = "Region " + random.Next(1, 5),
            Players = new List<Player>()
        };

        // Add players to the team
        int playerCount = players.Count > Teamsize ? Teamsize : players.Count;
        for (int j = 0; j < playerCount; j++)
        {
            int playerIndex = random.Next(0, players.Count);
            var player = players[playerIndex];
            player.TeamId = team.Id;
            team.Players.Add(player);
            players.RemoveAt(playerIndex);
        }

        db.Teams.Add(team);
    }
    db.SaveChanges();
}

