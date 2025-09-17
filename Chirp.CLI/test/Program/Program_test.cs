using System;
using System.IO;
using Xunit;
using Chirp.CLI.SimpleDB;
using Chirp.CLI;

namespace test;


public class Program_test
{
    [Fact]
    public void Test_Cheep_Result()
    { 
        var cheep = new Program.Cheep("Alice", "Hello World", "2025-09-17 12:34:56");
        Assert.Equal("Alice", cheep.Author);
        Assert.Equal("Hello World", cheep.Message);
        Assert.Equal("2025-09-17 12:34:56", cheep.Timestamp);
    }
}
