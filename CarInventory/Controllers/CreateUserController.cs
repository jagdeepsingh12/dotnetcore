using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarInventory.Models;


namespace CarInventory.Controllers;

public class CreateUserController : Controller
{
    private readonly ILogger<CreateUserController> _logger;

    public CreateUserController(ILogger<CreateUserController> logger )
    {
        _logger = logger;

    } 
    public IActionResult Index( )
    {
        _logger.LogInformation("Get request create user " );
        UserAuth retobj = new UserAuth() ;
        return View( retobj ) ;
    }
    [HttpPost , ValidateAntiForgeryToken ]
    public IActionResult Index( UserAuth obj )
    {
        if( obj.save() ){ 
            Response.Cookies.Append(  "id", obj.id.ToString() ,
            new Microsoft.AspNetCore.Http.CookieOptions()  {  Path = "/"   } );
            return RedirectToAction(  "",  "");
        }
        ModelState.Clear();
        return View( obj ) ;
    }
}