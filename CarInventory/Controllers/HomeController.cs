using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarInventory.Models;
// ---
using Microsoft.EntityFrameworkCore;


namespace CarInventory.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IWebHostEnvironment _hostingEnvironment; 
    public HomeController(ILogger<HomeController> logger , IWebHostEnvironment environment )
    {
        _logger = logger;
       _hostingEnvironment = environment;
    } 
    public IActionResult savecar( string code ){ 
        string[]  inputdata = code.Split("±") ;
        string resptext = "" ;  
        string id = Request.Cookies["id"] ;
        string[] crds = Accounts.CheckUserId( id ) ;
        // _logger.LogInformation("geeting data : {0} -- {1}", inputdata.Length ,  crds.Length );
        if( inputdata.Length == 6 && crds.Length > 2 ){
            string modifyid = inputdata[0] ;
            string brand = inputdata[1] ;
            string model = inputdata[2] ;
            string year = inputdata[3] ;
            string price = inputdata[4] ;
            string isnew  = inputdata[5] ;
            if( brand == "" ){
                resptext = resptext + "Brand ,";
            }
            if( model == "" ){
                resptext = resptext + "Model ,";
            }
            if( year == "" ){
                resptext = resptext + "Year ,";
            }
            if( price == "" ){
                resptext = resptext + "Price ," ;
            }
            if( resptext != "" ){
                resptext = resptext.Remove(resptext.Length-1);
                return Ok(resptext) ;
            }
            int carid = 0 ;
            if( modifyid != "" ){
                try{    carid = Int32.Parse( modifyid ) ; }
                catch( Exception E ){ _logger.LogInformation("error in converting id to int" ); }
            }
            _logger.LogInformation("integer id : {0}", carid  );
            Car findcar =  new Car() ;
            if( carid > 0 ){
                using (var db = new carinventoryContext())
                {
                    findcar = db.Cars.Single(b => b.CarId == carid );
                    findcar.Brand =  brand ;
                    findcar.Model = model ;
                    findcar.Year = Int32.Parse( year ) ;
                    findcar.Price = System.Convert.ToDecimal( price ) ;
                    findcar.New  = isnew == "1" ? true : false ;   
                    db.Entry(findcar).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }else{ 
                using (var db = new carinventoryContext())
                {
                    findcar = new Car() ;
                    findcar.Brand =  brand ;
                    findcar.Model = model ;
                    findcar.Year = Int32.Parse( year ) ;
                    findcar.Price = System.Convert.ToDecimal( price ) ;
                    findcar.New  = isnew == "1" ? true : false ;   
                    db.Entry(findcar).State = EntityState.Added;
                    db.SaveChanges();
                } 
            }
            Accounts.AssoiciateCarToUser( findcar.CarId.ToString()  , id  ) ;
            resptext = "Done" ;
        } 
        return Ok(resptext) ;
    }
    public IActionResult Index( )
    { 
        Accounts.checkFile() ;
        string id = Request.Cookies["id"] ;
        string[] carids = Accounts.CheckUserId( id ) ;
        if( carids.Length < 3 ){
            return RedirectToAction(  "index",  "Login" );
        }
        List<Car>  listof_inv_cars = new List<Car>() ;
        if( carids.Length > 3 ){
            string ids = carids[3] ;
            string[] carid_sep = ids.Split(","); 
            using (var db = new carinventoryContext())
            {
                listof_inv_cars = db.Cars.Where(b => carid_sep.Contains(b.CarId.ToString()) ) .ToList();
            }
        }  
        return View( listof_inv_cars );
    }
    [HttpPost , ValidateAntiForgeryToken ]
    public IActionResult Index( string brandsearch , string modelserach  )
    {  
        if( brandsearch == null ){ brandsearch = "" ; }
        if( modelserach == null ){ modelserach = "" ; }
        ViewBag.brandsearch = brandsearch ;
        ViewBag.modelserach = modelserach ;
        Accounts.checkFile() ;
        string id = Request.Cookies["id"] ;
        string[] carids = Accounts.CheckUserId( id ) ;
        if( carids.Length < 3 ){
            return RedirectToAction(  "index",  "Login" );
        }
      
        List<Car>  listof_inv_cars = new List<Car>() ;
        if( carids.Length > 3 ){
            string ids = carids[3] ;
            string[] carid_sep = ids.Split(","); 
            using (var db = new carinventoryContext())
            {
                listof_inv_cars = db.Cars.Where(b => ( carid_sep.Contains(b.CarId.ToString()) && 
                ( b.Brand.Contains( brandsearch ) && b.Model.Contains( modelserach )  ) ) ) .ToList();
            }
        }  
        return View( listof_inv_cars );
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Logout( )
    { 

       Response.Cookies.Append(  "id", ""  ,
            new Microsoft.AspNetCore.Http.CookieOptions()  {  Path = "/"   } );
        return RedirectToAction(  "",  "");
    }
    
}

public class Accounts{
    public static string startupPath =  System.IO.Directory.GetCurrentDirectory() ; 
    public static string path = Path.Combine(startupPath , "AccountsFile.txt" ) ;
    public static  void checkFile( ){
        if (!(File.Exists(path)))
        {
            File.CreateText(path);
        }
    }
    public static string[] readLines(){
        string[] lines ;
        using (StreamReader streamReader = File.OpenText(path))
        {
            string text = streamReader.ReadToEnd();
            lines = text.Split(Environment.NewLine);
        }
        return  lines ;
    }
    public static string[] CheckUserId(string id ){
        string[] car_inv_ids  = new string[0] ;
        string[] lines =  readLines() ;
        foreach (string line in lines) {
            string[] crds = line.Split("±"); 
            if( crds.Length > 2  && id == crds[0] ){
                car_inv_ids  = crds ;
                break ;
            }
        }
        return car_inv_ids ; 
    }
    public static string[] CheckUserForLogin(string username , string password  ){
        string[] car_inv_ids  = new string[0] ;
        string[] lines =  readLines() ;
        foreach (string line in lines) {
            string[] crds = line.Split("±"); 
            if( crds.Length > 2  && username == crds[1] && password == crds[2] ){
                car_inv_ids  = crds ;
                break ;
            }
        }
        return car_inv_ids ;
    }
    public static bool UserNameIsUsed( string username , ref int id_inc ){
        if( username != "" ){
            int checkholdid = 1 ;
            string[] lines = readLines() ;
            foreach (string line in lines) {
                string[] crds = line.Split("±"); 
                if( crds.Length > 2 ){
                    if( username == crds[1] ){
                        return true ;
                    }
                    checkholdid = Int32.Parse( crds[0] ) ;
                    if( checkholdid > id_inc ){
                        id_inc = checkholdid ;
                    }
                }  
            }
            id_inc = id_inc + 1 ;
        }
        return false ;
    }
    public static void AssoiciateCarToUser( string carId , string userid ){
        string[] arrLine = File.ReadAllLines(path);
        for(int i = 0; i < arrLine.Length ; i++)
        {
            string[] crds = arrLine[i].Split("±"); 
            if( crds.Length > 2  && userid== crds[0] ){
                if( crds.Length > 3  ){   arrLine[i] = arrLine[i] + carId + "," ;  }
                else{    arrLine[i] = arrLine[i] + "±"  + carId + "," ;  }
                
                break ;
            } 
        }
        File.WriteAllLines(path, arrLine);
    }
}
public class UserAuth{
    public int id  { get; set; }
    public string username  { get; set; }
    public string password  { get; set; } 
    public string validateError { get; set; } 
    public string app_msg { get; set; } 
    public UserAuth( string username , string password ){
        this.username = username ;
        this.password = password;
        this.id = 0 ;
        this.app_msg = "0" ;
        this.validateError = "" ;
    } 
    public UserAuth( string [] fileread  ){
        if( fileread.Length  > 2 ){ 
            this.id = Int32.Parse( fileread[0] )  ;
            this.username = fileread[1] ;
            this.password = fileread[2] ;
        }else{
            this.username = ""  ;
            this.password = "" ; 
        }
        this.id = 0 ;
        this.validateError = "" ;
        this.app_msg = "0" ;
    }
    public UserAuth( ){
        this.username = ""  ;
        this.password = "" ;
        this.id = 0 ;
        this.app_msg = "0" ;
        this.validateError = "" ;
    } 
    public bool login(){
        string[] userids = Accounts.CheckUserForLogin( this.username , this.password  ) ;
        if( userids.Length > 2  ){
            this.id = Int32.Parse( userids[0] ) ;
            return true ;
        }
        this.validateError = "Please check your user name and password." ;
        this.app_msg = "1" ;
        return false ;
    }
    public bool save(){
        string validate_build = "" ;
        int save_next_id = 1 ;
        if( this.username == "" || this.username == null  ){ 
            validate_build = validate_build + "User Name ," ;
        }
        if( this.password == "" || this.password == null ){
            validate_build = validate_build + " Password ," ;
        }
        if( validate_build == "" && Accounts.UserNameIsUsed( this.username , ref save_next_id )  ){
            this.validateError = "User Name already taken" ;
            this.app_msg = "1" ;
            return false ;
        }
        else if( validate_build.Length  > 0  ){
            validate_build = validate_build.Remove(validate_build.Length-1); 
            this.validateError = validate_build ; 
            return false ;
        }else{  saveExact( save_next_id );  }
        return true  ;
    } 
    public bool isId( int id ){
        if( id == this.id ){ return true ; }
        return false ;
    }
    public bool isId( string id ){
        if( this.id.ToString() == id ){ return true ; }
        return false   ;
    } 
    private void saveExact( int id ){
        using (StreamWriter outputFile = new StreamWriter( Accounts.path , true))
        {
            outputFile.WriteLine( id.ToString() + "±" + this.username + "±" + this.password );
        }
        this.id = id ;
    }
    
}