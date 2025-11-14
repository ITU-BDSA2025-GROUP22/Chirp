using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

/**
    INSTALLATION MAY BE NEEDED TO RUN PLAYWRIGHT TESTS
        - in the terminal, run:
            bin/Debug/net8.0/playwright.ps1 install --with-deps

        - this is a PowerShell script - it might need execution permissions:
            Set-ExecutionPolicy -ExecutionPolicy RemoteSigned

    TESTING
    run command: bin/Debug/net8.0/playwright.ps1 codegen https://1bdsagroup22chirp.azurewebsites.net/
    to record End-To-End test
*/

public class EndToEndTests : PageTest
{
    [SetUp]
    public async Task Setup()
    {
        await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });
    }

    [Test]
    public async Task NewUserCanRegisterAccount()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("myusername");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("name@example.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("123ABCabc!");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync("123ABCabc!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
    }

    [Test]
    public async Task UnregisteredUserCannotAccessAuthors()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        await Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" })
            .GetByRole(AriaRole.Link).ClickAsync();
        var login = Page.GetByRole(AriaRole.Link, new() { Name = "login" });
        await Expect(login).ToHaveAttributeAsync("href", "/Identity/Account/Login");
        await Expect(login).ToBeVisibleAsync();
        await Expect(login).ToHaveTextAsync("Login");
        await Expect(login).ToHaveAttributeAsync("href", "/Identity/Account/Login");
    }
    
    // add more tests...
}
