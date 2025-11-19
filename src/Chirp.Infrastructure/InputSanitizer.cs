using System.Text.RegularExpressions;
using System.Web;
using Ganss.Xss; // Used for XSS cleaning

namespace Chirp.Infrastructure;

public interface IInputSanitizer
{
    string SanitizeCheepText(string input);
}

public class InputSanitizer : IInputSanitizer
{
    /**
     * Sanitizes user input for malicious content like XSS attacks.
     * 
     * Relies on HtmlSanitizer package.
     * 
     * Takes the users input string as parameter and returns the "safe" string back.
     * 
     * Used in Public.cshtml.cs for method OnPostAsync before creating the cheep
     * and adding it to the database.
     */
    public string SanitizeCheepText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }
        
        var sanitizer = new HtmlSanitizer();
        var cleanHtml = sanitizer.Sanitize(input);
        return cleanHtml;
    }
}