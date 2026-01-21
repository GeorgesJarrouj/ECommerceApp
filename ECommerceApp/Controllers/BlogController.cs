using Microsoft.AspNetCore.Mvc;

public class BlogController : Controller
{
    public IActionResult Index() => View();
}
