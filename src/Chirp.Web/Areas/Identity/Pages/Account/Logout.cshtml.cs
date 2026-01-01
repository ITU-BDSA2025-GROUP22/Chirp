// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Chirp.Web.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    /// <summary>
    /// The logout model class
    /// </summary>
    /// <seealso cref="PageModel"/>
    public class LogoutModel : PageModel
    {
        /// <summary>
        /// The sign in manager
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<LogoutModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutModel"/> class
        /// </summary>
        /// <param name="signInManager">The sign in manager</param>
        /// <param name="logger">The logger</param>
        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Ons the post using the specified return url
        /// </summary>
        /// <param name="returnUrl">The return url</param>
        /// <returns>A task containing the action result</returns>
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
