using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarInventory.Models;


namespace CarInventory.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger )
    {
        _logger = logger;
    } 
    public IActionResult Index( )
    {
        UserAuth retobj = new UserAuth() ;
        return View( retobj ) ;
    }
    [HttpPost , ValidateAntiForgeryToken ]
    public IActionResult Index( UserAuth obj )
    {
        if( obj.login() ){ 
            Response.Cookies.Append(  "id", obj.id.ToString() ,
            new Microsoft.AspNetCore.Http.CookieOptions()  {  Path = "/"   } );
            return RedirectToAction(  "",  "");
        }
        ModelState.Clear();
        return View( obj ) ;
    }
}