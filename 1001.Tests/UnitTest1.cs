using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notesbin.Controllers;
using _1001;

namespace _1001.Tests;

//Tests look good, I like the seperation of logic and where everything is setup

/// <summary>
/// Backend Unit Tests for 1001 TrackList Application
/// </summary>
public class BackendTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SetsController _controller;

    public BackendTests()
    {
        // Create an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new SetsController(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var artist = new Artist
        {
            ArtistId = 1,
            DisplayName = "Carl Cox",
            Country = "UK"
        };

        var venue = new Venue
        {
            VenueId = 1,
            Name = "Printworks London",
            Capacity = 5000,
            Address = "London, UK"
        };

        var djSet = new DjSet
        {
            DjSetId = 1,
            ArtistId = 1,
            Title = "Space Ibiza 2015",
            SetDatetime = new DateTime(2015, 8, 15, 22, 0, 0, DateTimeKind.Utc),
            DurationMinutes = 180,
            VenueId = 1
        };
        //Space Ibiza is a great test here. Looks good

        var analytics = new SetAnalytics
        {
            DjSetId = 1,
            TicketsSold = 500,
            AttendanceCount = 480,
            StreamCount = 150000,
            LikeCount = 8500
        };

        _context.Artists.Add(artist);
        _context.Venues.Add(venue);
        _context.DjSets.Add(djSet);
        _context.SetAnalytics.Add(analytics);
        _context.SaveChanges();
    }

    /// <summary>
    /// Backend Test 1: Verify that SetsController Index returns all DJ sets with proper data
    /// </summary>
    [Fact]
    public async Task SetsController_Index_ReturnsViewWithAllSets()
    {
        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<DjSet>>(viewResult.Model);
        
        var djSets = model.ToList();
        Assert.Single(djSets);
        
        var set = djSets[0];
        Assert.Equal("Space Ibiza 2015", set.Title);
        Assert.NotNull(set.Artist);
        Assert.Equal("Carl Cox", set.Artist.DisplayName);
        Assert.NotNull(set.Venue);
        Assert.Equal("Printworks London", set.Venue.Name);
        Assert.NotNull(set.SetAnalytics);
        Assert.Equal(500, set.SetAnalytics.TicketsSold);
    }

    /// <summary>
    /// Backend Test 2: Verify that Artist model initializes correctly with navigation properties
    /// </summary>
    [Fact]
    public void Artist_Model_InitializesCorrectly()
    {
        // Arrange & Act
        var artist = new Artist
        {
            ArtistId = 2,
            DisplayName = "Nina Kraviz",
            Country = "Russia"
        };

        // Assert
        Assert.Equal(2, artist.ArtistId);
        Assert.Equal("Nina Kraviz", artist.DisplayName);
        Assert.Equal("Russia", artist.Country);
        
        // Verify navigation properties are initialized
        Assert.NotNull(artist.SongArtists);
        Assert.Empty(artist.SongArtists);
        Assert.NotNull(artist.DjSets);
        Assert.Empty(artist.DjSets);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _controller.Dispose();
    }
}
