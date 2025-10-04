namespace Chirp.Tests;

using Xunit;
using Chirp.Razor;

public class DBFacadeTests
{
    private DBFacade db = new ();
    [Fact]
    public void TestGetPagedCheeps()
    {
        var cheeps = db.GetPagedCheeps(1, 32);

        Assert.True(cheeps.Count <= 32);
    }

    [Fact]
    public void TestGetCheepsByAuthor()
    {
        var cheeps = db.GetPagedCheepsByAuthor("Adrian", 1, 32);
        
        Assert.Equal("Adrian", cheeps[0].Author);
    }
}

