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
    /// <summary>
    /// Setup this instance
    /// </summary>
    [SetUp]
    public async Task Setup()
    {
        await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });
        Page.SetDefaultNavigationTimeout(60000);
        Page.SetDefaultTimeout(60000);
    }

    //THEME TEST
    /// <summary>
    /// Tests that theme button changes
    /// </summary>
    [Test]
    public async Task ThemeButtonChanges()
    {
        
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        
        //check visibility and value of theme button changes when clicked
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "☀️" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#theme-toggle")).ToContainTextAsync("☀️");
        await Page.GetByRole(AriaRole.Button, new() { Name = "☀️" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "🌙" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#theme-toggle")).ToContainTextAsync("🌙");
        await Page.GetByRole(AriaRole.Button, new() { Name = "🌙" }).ClickAsync();
        await Expect(Page.Locator("#theme-toggle")).ToContainTextAsync("☀️");
    }

    
    //UNREGISTERED USER TESTS
    /// <summary>
    /// Tests that unregistered user cannot access authors
    /// </summary>
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

    /// <summary>
    /// Tests that unregistered user cannot like cheeps
    /// </summary>
    [Test]
    public async Task UnregisteredUserCannotLikeCheeps()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        
        //check that register tab (user is not logged in) and cheep like button is available/visible
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First).ToBeVisibleAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👍 0");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "register" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Navigation)).ToContainTextAsync("register");
        
        //click like button then check url is now /login
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
    }

    /// <summary>
    /// Tests that unregistered user can view other account timelines
    /// </summary>
    [Test]
    public async Task UnregisteredUserCanViewOtherAccountTimelines()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        
        //check that post on global timeline is visible, then click on profile name
        await Expect(Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link)).ToBeVisibleAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Jacqualine Gilcoine");
        await Expect(Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link)).ToHaveAttributeAsync("href", "/Jacqualine Gilcoine");
        
        await Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Jacqualine%20Gilcoine/?$"));
    }

    //REGISTER TESTS
    /// <summary>
    /// Tests that register page element visibility
    /// </summary>
    [Test]
    public async Task RegisterPageElementVisibility()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Register");
        
        //checks visibility and asserts string value of page elements
        await Expect(Page.GetByText("Create a new account. Username Email Password Confirm Password Register")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Register" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" })).ToBeVisibleAsync();
        
        await Expect(Page.Locator("#registerForm")).ToContainTextAsync("Username");
        await Expect(Page.Locator("#registerForm")).ToContainTextAsync("Email");
        await Expect(Page.Locator("#registerForm")).ToContainTextAsync("Password");
        await Expect(Page.Locator("#registerForm")).ToContainTextAsync("Confirm Password");
        await Expect(Page.Locator("h3")).ToContainTextAsync("Use another service to register.");
        
        await Expect(Page.Locator("h3")).ToContainTextAsync("Use another service to register.");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Register", Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Use another service to" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#registerForm").GetByRole(AriaRole.Separator)).ToBeVisibleAsync();
        await Expect(Page.Locator("section").GetByRole(AriaRole.Separator)).ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that register user
    /// </summary>
    [Test]

    public async Task RegisterUser()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Register");
        
        //Generate random user info
        var randomUsername = GenerateUsername();
        var RandomPassword = GeneratePassword();
        var email = randomUsername + "@gmail.com";
        
        //Type in userinfo
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(randomUsername);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(RandomPassword);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync(RandomPassword);
        
        //assert info boxes have been filled out
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })).ToHaveValueAsync(randomUsername);
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })).ToHaveValueAsync(email);
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true })).ToHaveValueAsync(RandomPassword);
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" })).ToHaveValueAsync(RandomPassword);
        
        //click register, check url, then confirm email 
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Identity/Account/RegisterConfirmation\\b.*"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Register confirmation" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" })).ToBeVisibleAsync();
        
        //Check link to microsoft documentation works then go back and click confirm email link
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "these docs" })).ToHaveAttributeAsync("href", "https://aka.ms/aspaccountconf");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "these docs" })).ToHaveAttributeAsync("href", "https://aka.ms/aspaccountconf");
        await Page.GetByRole(AriaRole.Link, new() { Name = "these docs" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-10.0&tabs=visual-studio");
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/RegisterConfirmation?email=" + email + "&returnUrl=%2F");
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Identity/Account/RegisterConfirmation\\b.*"));
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        //check url again
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Identity/Account/ConfirmEmail\\b.*"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm email" })).ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that register info fill warnings
    /// </summary>
    [Test]
    public async Task RegisterInfoFillWarnings()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Register");
        
        //Fill no boxes
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByText("The UserName field is")).ToBeVisibleAsync();
        await Expect(Page.GetByText("The Email field is required.")).ToBeVisibleAsync();
        await Expect(Page.GetByText("The Password field is")).ToBeVisibleAsync();

        //password is too short
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("s");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByText("The Password must be at least")).ToBeVisibleAsync();
        await Page.GetByText("The password and confirmation").ClickAsync();
        
        //check all password warnings
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("user");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("user@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("abcdef");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync("abcdef");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByText("Passwords must have at least one non alphanumeric character.")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Passwords must have at least one digit ('0'-'9')")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Passwords must have at least one uppercase ('A'-'Z')")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("123456");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync("123456");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByText("Passwords must have at least one non alphanumeric character.")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Passwords must have at least one lowercase ('a'-'z')")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Passwords must have at least one uppercase ('A'-'Z')")).ToBeVisibleAsync();
        
        //try to register with username that is already taken
        var username = GenerateUsername();
        var password = GeneratePassword();
        await RegisterHelper(username, password);
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Register");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(username);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("username@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByText("Username '" + username + "' is already taken.")).ToBeVisibleAsync();

    }


    //LOGIN TESTS
    /// <summary>
    /// Tests that login page element visibility
    /// </summary>
    [Test]
    public async Task LoginPageElementVisibility()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Username")).ToBeVisibleAsync();
        await Page.GetByText("Password", new() { Exact = true }).ClickAsync();

        await Expect(Page.GetByText("Remember me?")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Log in" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Forgot your password?" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register as a new user" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Resend email confirmation" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" })).ToBeVisibleAsync();
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");

    }
    
    /// <summary>
    /// Tests that log into account
    /// </summary>
    [Test]
    public async Task LogIntoAccount()
    {
        //register new account first
        var username = GenerateUsername();
        var password = GeneratePassword();
        await RegisterHelper(username, password);
        
        //fill out info and click log in
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(username);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })).ToHaveValueAsync(username);
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })).ToHaveValueAsync(password);
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" }).CheckAsync();
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" })).ToBeCheckedAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" }).UncheckAsync();
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" })).Not.ToBeCheckedAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Remember me?" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        
        //assert url is /logout, and "my timeline", "logout" and "my data" tabs are visible after logging in
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "logout ["+ username + "]" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my data" })).ToBeVisibleAsync();
        
        //Go to log out page and log out, then check redirect to /login works
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [" + username + "]" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Logout");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login?ReturnUrl=%2FIdentity%2FAccount%2FLogout");
    }

    /// <summary>
    /// Tests that login info fill warnings
    /// </summary>
    [Test]
    public async Task LoginInfoFillWarnings()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
        
        //Click login with no username or password filled
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByText("The Username field is")).ToBeVisibleAsync();
        await Expect(Page.GetByText("The Password field is")).ToBeVisibleAsync();
        
        //invalid login
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("j");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("j");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByText("Invalid login attempt.")).ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that forgot your password link
    /// </summary>
    [Test]
    public async Task ForgotYourPasswordLink()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
        
        //check visibility and url once clicked and redirected to /forgotPassword
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Forgot your password?" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Forgot your password?" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/ForgotPassword");
        
        //check visibility of elements and fill out email, then click reset password button
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Email", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Reset Password" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Reset Password" }).ClickAsync();
        await Expect(Page.GetByText("The Email field is required.")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("name@example.com");
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })).ToHaveValueAsync("name@example.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Reset Password" }).ClickAsync();
        
        //check redirected url
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/ForgotPasswordConfirmation");
    }

    /// <summary>
    /// Tests that register as a new user link
    /// </summary>
    [Test]
    public async Task RegisterAsANewUserLink()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
        
        //check visibility and confirm url is "/register?returnURL" after click
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register as a new user" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register as a new user" }).ClickAsync();
        await Expect(Page)
            .ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Register?returnUrl=%2F");
    }

    /// <summary>
    /// Tests that resend email confirmation link
    /// </summary>
    [Test]
    public async Task ResendEmailConfirmationLink()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
        
        //check visibility and confirm url after click
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Resend email confirmation" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Resend email confirmation" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/ResendEmailConfirmation");

        //fill out email and check visibility of verification confirmation
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })).ToBeEmptyAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Resend" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("name@example.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Resend" }).ClickAsync();
        await Expect(Page.GetByText("Verification email sent.")).ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that login with github button
    /// </summary>
    [Test]
    public async Task LoginWithGithubButton()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Register");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("https://github.com/login\\b.*"));
    }


    //USER TIMELINE TESTS
    /// <summary>
    /// Tests that user can follow users and view posts in personal timeline
    /// </summary>
    [Test]
    public async Task UserCanFollowUsersAndViewPostsInPersonalTimeline()
    {
        //Register and login
        var username =  GenerateUsername();
        var password = GeneratePassword();
        await LoginHelper(username, password);
        
        //Check visibility of user on public timeline, then click their username link
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        await Expect(Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link)).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link).ClickAsync();
        
        //assert url redirect, then check visibility of user timeline and follow button 
        await Expect(Page).ToHaveURLAsync(new Regex("https://1bdsagroup22chirp.azurewebsites.net/Jacqualine%20Gilcoine\\b.*"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Follow" })).ToBeVisibleAsync();
        await Expect(Page.Locator("form")).ToContainTextAsync("Follow");
        
        //check that button changes to unfollow after clicking it
        await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();
        await Expect(Page.Locator("form")).ToContainTextAsync("Unfollow");

        //Go to users personal timeline and assert that followed user's posts shows up on timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Jacqualine Gilcoine • Following Starbuck now is what we hear the worst. — 08/01" }).GetByRole(AriaRole.Link)).ToBeVisibleAsync();
        await Expect(Page.GetByText(username + "'s Timeline Jacqualine")).ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that user can like cheeps
    /// </summary>
    [Test]
    public async Task UserCanLikeCheeps()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        
        //Register and login
        var username =  GenerateUsername();
        var password = GeneratePassword();
        await LoginHelper(username, password);
        
        //Go to public timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        
        //Assert visibility and value of like/dislike buttons
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "👎" }).First).ToBeVisibleAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👍 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👎 0");
        
        //Like the post, then unlike the post
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👍 1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍 1" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👍 0");
        
        //click like then dislike to remove like automatically
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👎" }).First.ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👍 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👎 1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👎 0");
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👍 0");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("👎 0");
    }

    /// <summary>
    /// Tests that user can post cheep and view in personal timeline
    /// </summary>
    [Test]
    public async Task UserCanPostCheepAndViewInPersonalTimeline()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");

        //Register and login
        var username =  GenerateUsername();
        var password = GeneratePassword();
        await LoginHelper(username, password);
        
        //Go to public timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        
        //Check visibility of share elements 
        await Expect(Page.GetByText("What's on your mind " + username + "? Share")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();
        
        //Post cheep and check visibility
        await Page.Locator("#CheepText").ClickAsync();
        await Page.Locator("#CheepText").FillAsync("Hello world");
        await Expect(Page.Locator("#CheepText")).ToHaveValueAsync("Hello world");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = username + " Hello world" }).Locator("div")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" })).ToBeVisibleAsync();
        
        //Go to personal timeline, assert url, then check that the cheep is visible
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/" + username);
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = username + "'s Timeline" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToBeVisibleAsync();
        await Expect(Page.GetByText(username + " Hello world")).ToBeVisibleAsync();
        await Expect(Page.Locator("div").Nth(2)).ToBeVisibleAsync();
    }

    //MY DATA PAGE TESTS
    /// <summary>
    /// Tests that my data page visibility
    /// </summary>
    [Test]
    public async Task MyDataPageVisibility()
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/");

        //Register and login
        var username =  GenerateUsername();
        var password = GeneratePassword();
        await LoginHelper(username, password);
        
        //Go to "My Data" page and assert url
        await Page.GetByRole(AriaRole.Link, new() { Name = "my data" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/ManageData");
        
        //Assert visibility of page elements
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "My Data" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Download your account data" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Download my data" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Delete account" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Yes, delete my account" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })).ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that user can delete their userdata
    /// </summary>
    [Test]
    public async Task UserCanDeleteTheirUserdata()
    {
        //Register and login
        var username =  GenerateUsername();
        var password = GeneratePassword();
        await LoginHelper(username, password);
        
        //Assert account tabs are visible once logged in
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "logout [" + username + "]" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my data" })).ToBeVisibleAsync();
        
        //Go to timeline, like another users cheep and publish a cheep
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "👍" }).First.ClickAsync();
        await Page.Locator("#CheepText").FillAsync("I wont exist in a sec");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = username + " I wont exist in a sec" }).Locator("div")).ToBeVisibleAsync();
        
        //Go to "My Data" page and assert url
        await Page.GetByRole(AriaRole.Link, new() { Name = "my data" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/ManageData");
        
        //Delete userdata
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Yes, delete my account" }).ClickAsync();
        
        //Assert url is frontpage now that userdata is deleted
        await Expect(Page).ToHaveURLAsync("https://1bdsagroup22chirp.azurewebsites.net/");
        
        //Assert account tabs are not visible anymore
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" })).Not.ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "logout [" + username + "]" })).Not.ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "my data" })).Not.ToBeVisibleAsync();
        
        //Assert users cheep has been deleted and is not part of public timeline anymore
        await Expect(Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = username + " I wont exist in a sec" }).Locator("div")).Not.ToBeVisibleAsync();
    }

    /// <summary>
    /// Tests that user cant login after deleting userdata
    /// </summary>
    [Test]
    public async Task UserCantLoginAfterDeletingUserdata()
    {
        //Register and login
        var username = GenerateUsername();
        var password = GeneratePassword();
        await LoginHelper(username, password);
        
        //Go to "My Data" page and assert url
        await Page.GetByRole(AriaRole.Link, new() { Name = "my data" }).ClickAsync();
        
        //Delete userdata
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Yes, delete my account" }).ClickAsync();
        
        //Assert login info is not recognized by chirp anymore
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(username);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByText("Invalid login attempt.")).ToBeVisibleAsync();
        
    }

    //Helper functions

    /// <summary>
    /// Generates the password
    /// </summary>
    /// <returns>The string</returns>
    private string GeneratePassword()
    {
        var rnd = Guid.NewGuid().ToString("N").Substring(0, 6);
        return $"A.{rnd}";
    }

    /// <summary>
    /// Generates the username
    /// </summary>
    /// <returns>The string</returns>
    private string GenerateUsername()
    {
        return "user_" + Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    /// <summary>
    /// Registers the helper using the specified username
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    private async Task RegisterHelper(String username, String password)
    {
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Register");
        
        //Generate random user info
        var randomUsername = username;
        var RandomPassword = password;
        var email = randomUsername + "@gmail.com";
        
        //Type in userinfo
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(randomUsername);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(RandomPassword);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync(RandomPassword);
        
        //Click register, check url, then confirm email 
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        //Click back to public timeline 
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
    }

    /// <summary>
    /// Logins the helper using the specified username
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    private async Task LoginHelper(String username, String password)
    {
        //Register user
        await RegisterHelper(username, password);
        
        //go to login and fill out info, then go to public timeline
        await Page.GotoAsync("https://1bdsagroup22chirp.azurewebsites.net/Identity/Account/Login");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(username);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
    }
}