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
    public async Task NewUserCanRegisterAccountAndLogin() //Would be easier to split into 2 tests, but not feasible while database resets users 
    {

        //Register part
        //start from front page and click register page
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        
        //locator variables for register page
        var username = Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
        var email =  Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" });
        var password  = Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
        var confirmPassword  = Page.GetByRole(AriaRole.Textbox, new() { Name = "ConfirmPassword" });
        var registerButton = Page.GetByRole(AriaRole.Button, new() { Name = "Register" });
        var githubButton = Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" });

        //Check elements are visible
        await Expect(username).ToBeVisibleAsync();
        await Expect(email).ToBeVisibleAsync();
        await Expect(password).ToBeVisibleAsync();
        await Expect(confirmPassword).ToBeVisibleAsync();
        await Expect(registerButton).ToBeVisibleAsync();
        await Expect(githubButton).ToBeVisibleAsync();
        
        //type in account info and click register
        await username.ClickAsync();
        await username.FillAsync("myusername");
        await email.ClickAsync();
        await email.FillAsync("name@example.com");
        await password.ClickAsync();
        await password.FillAsync("123ABCabc!");
        await confirmPassword.ClickAsync();
        await confirmPassword.FillAsync("123ABCabc!");
        await registerButton.ClickAsync();
        
        //Check redirect url, confirm email, then click confirm email
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Identity/Account/RegisterConfirmation\\b.*"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Register confirmation" })).ToBeVisibleAsync(); 
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        //check redirect url again
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Identity/Account/ConfirmEmail\\b.*"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm email" })).ToBeVisibleAsync();
        
        
        
        //Login part
        //Begin by directing to login page
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        
        //check all page elements are visible
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Log in" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Forgot your password?" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register as a new user" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Resend email confirmation" })).ToBeVisibleAsync();
        
        //fill out account info and check off remember me, then click log in
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("myusername");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("123ABCabc!");
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" })).Not.ToBeCheckedAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" }).CheckAsync();
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" })).ToBeCheckedAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        //Check redirect has sent us to the homepage and "my timeline", "logout" and "my data" tabs are visible and reachable by click
        await Expect(Page).ToHaveURLAsync("/");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "logout [myusername]" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my data" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("/myusername");
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [myusername]" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("/Logout");
        await Page.GetByRole(AriaRole.Link, new() { Name = "my data" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("/ManageData");
    }
    
    [Test]
    public async Task UnregisteredAccountCannotLogin()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        var username = Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
        var password = Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
        var loginButton = Page.GetByRole(AriaRole.Textbox, new() { Name = "Log in" });
        await Expect(username).ToBeVisibleAsync();
        await Expect(username).ToHaveTextAsync("Username");
        await Expect(password).ToBeVisibleAsync();
        await Expect(password).ToHaveTextAsync("Password");
        await Expect(loginButton).ToBeVisibleAsync();
        await Expect(loginButton).ToHaveTextAsync("Log in");
        
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Forgot your password?" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register as a new user" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Resend email confirmation" })).ToBeVisibleAsync();
        
        //try to log in without filling out information
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByText("The Username field is")).ToBeVisibleAsync();
        await Expect(Page.GetByText("The Password field is required.")).ToBeVisibleAsync();
        
        //Try to log in with wrong password
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("jakob");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("NotAPassword");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByText("Invalid login attempt.")).ToBeVisibleAsync();
        
        //Try to log in without password
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("jakob");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByText("The Password field is")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UnregisteredUserCannotAccessAuthors()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        await Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" })
            .GetByRole(AriaRole.Link).ClickAsync();
        var login = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });
        await Expect(login).ToHaveAttributeAsync("href", "/Identity/Account/Login");
        await Expect(login).ToBeVisibleAsync();
        await Expect(login).ToHaveTextAsync("login");
        await Expect(login).ToHaveAttributeAsync("href", "/Identity/Account/Login");
    }

    [Test]
    public async Task UnregisteredUserCannotLikeCheep()
    {
        //redirect to login page when liking cheeps without being logged in
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync("/Identity/Account/Login");
        
        //logging in
        var username = Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
        var password = Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
        var login = Page.GetByRole(AriaRole.Textbox, new() { Name = "Log in" });
        await Expect(username).ToBeVisibleAsync();
        await Expect(username).ToHaveTextAsync("Username");
        await Expect(password).ToBeVisibleAsync();
        await Expect(password).ToHaveTextAsync("Password");
        await Expect(login).ToBeVisibleAsync();
        await Expect(login).ToHaveTextAsync("Log in");
        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("jakob");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Jakob.1");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).PressAsync("Enter");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        //go to public timeline and like posts
        await Expect(Page).ToHaveURLAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👎" }).Nth(1).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).Nth(2).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).Nth(4).ClickAsync();
        
        //test that cheeps are liked/disliked
        await Expect(Page).ToHaveURLAsync("/");
        var likeButton = Page.GetByRole(AriaRole.Button, new() { Name = "👍" });
        var dislikeButton = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions() { Name = "👎" });
        await Expect(likeButton.First).ToHaveTextAsync("👍 1");
        await Expect(dislikeButton.First).ToHaveTextAsync("👎");
        await Expect(dislikeButton.Nth(1)).ToHaveTextAsync("👎 1");
        await Expect(likeButton.Nth(2)).ToHaveTextAsync("👍 1");
        await Expect(likeButton.Nth(4)).ToHaveTextAsync("👍 1");
        
        //dislike a liked post and like a disliked post and check that opposite attribute (like/dislike) has been removed 
        await dislikeButton.First.ClickAsync();
        await Expect(likeButton.First).ToHaveTextAsync("👍 0");
        await Expect(dislikeButton.First).ToHaveTextAsync("👎 1");
        await likeButton.Nth(1).ClickAsync();
        await Expect(dislikeButton.Nth(1)).ToHaveTextAsync("👎 0");
        await Expect(likeButton.Nth(1)).ToHaveTextAsync("👍 1");
    }

    [Test]
    public async Task PostCheepAndLike()
    {
        //share post and like it
        Page.Locator("#CheepText").ClickAsync();
        await Page.Locator("#CheepText").FillAsync("Who likes their own post?");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        
        //Test that post is visible on timeline
        var cheep = Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "Who likes their own post?" });
        await Expect(cheep).ToBeVisibleAsync();
        await Expect(cheep).ToHaveTextAsync("Who likes their own post?");
            
        //Click on own name and go to my Timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "jakob", Exact = true }).ClickAsync();
        await Expect(cheep).ToBeVisibleAsync();
    }

    [Test]
    public async Task FollowAndCheckTimeline()
    {
        
    }

    [Test]
    public async Task ChangeTheme()
    {
        
    }

    [Test]
    public async Task DeleteAccount()
    {
        
    }

    [Test]
    public async Task DownloadUserData()
    {
        
    }

    [Test]
    public async Task NewUserCanRegisterAccountViaGithubOAuth()
    {
        
    }

    [Test]
    public async Task RegisterAccountErrorTextVisibility()
    {
        
    }
}
