using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace truthfulls.com.Pages
{
    public class ErrorModel : PageModel
    {
        public string Message { get; private set; } = "Error getting requested page";
        public void OnGet(string? error = null)
        {
            if (error != null) 
            {
                this.Message = error;
            }
            
        }
    }
}
