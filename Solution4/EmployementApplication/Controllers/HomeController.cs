using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using EmployementApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using WebGrease.Css.Ast.Selectors;

namespace EmployementApplication.Controllers
{
    [HandleError]
 //[AllowAnonymous]
    
    public class HomeController : Controller
    {
            private readonly UserManager<AppUser> userManager;
            IEmployeesRepository _employeesRepository;
            IDepartmentsRepository _departmentsRepository;

            public HomeController()
                : this(new EFEmployeesRepository(), new EFDepartmentsRepository(), Startup.UserManagerFactory.Invoke())
            {
                //_employeesRepository = new EFEmployeesRepository();
                //_departmentsRepository = new EFDepartmentsRepository();

            }
            public HomeController(IEmployeesRepository employeesRepository, IDepartmentsRepository departmentsRepository, UserManager<AppUser> userManager)
            {
                _employeesRepository = employeesRepository;
                _departmentsRepository = departmentsRepository;
                this.userManager = userManager;
            }
            

            
            public ViewResult Index()
            {
                return View();
            }

            
           

            [HttpGet]
            
            public PartialViewResult LogIn()
            {
                return PartialView("LogIn");
            }

            [HttpPost]

            public async Task<ActionResult> LogIn(LogInModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var user = await userManager.FindAsync(model.Email, model.Password);

                if (user != null)
                {
                    var identity = await userManager.CreateIdentityAsync(
                        user, DefaultAuthenticationTypes.ApplicationCookie);

                    GetAuthenticationManager().SignIn(identity);

                  return Redirect("http://10.1.81.226:1043/Home/Index");
                }
                
                    // user authN failed
                    //return Redirect("http://10.1.81.226:1043/#Login?err=1");
                //ModelState.AddModelError("", "Invalid email or password");
                return View();
                
            }
          
     
            public async Task<ActionResult> Logout(LogInModel model)
            {
                await SignOut();
                return RedirectToAction("Index", "Home");
            }

        [HttpGet]
        
            public PartialViewResult Register()
            {
                return PartialView("register");
            }

     
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new AppUser
            {
                UserName = model.Email,
                
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SignIn(user);
                return Redirect("http://10.1.81.226:1043/Home/Index");
            }

            return View();
        }
       
  
        
        private async Task SignIn(AppUser user)
        {
            var identity = await userManager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);
            GetAuthenticationManager().SignIn(identity);
        }

        
        private async Task SignOut()
        {
            GetAuthenticationManager().SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }
        //private string GetRedirectUrl()
        //{
        //    return Redirect("http://10.1.81.226:1043/Home/Index");
            
        //}

        
        //----------------------------------------------------------------------------------------------------------------------

        [HttpPost]
        [OverRidingAuthorizationAttribute]
            public JsonResult  CreateEmployee(Employees employee)
            {
                string res = "";

                if (ModelState.IsValid)
                {
                    _employeesRepository.CreateEmployee(employee);
                     res = JsonConvert.SerializeObject(employee);
                    
                }
                else
                {
                    ModelState.AddModelError("","");
                }
                return Json(res);
            }

            [HttpGet]
            [OverRidingAuthorizationAttribute]
            public JsonResult GetAllEmployee()
            {

                IEnumerable<Employees> employee = _employeesRepository.GetAllEmployees();

                string res = JsonConvert.SerializeObject(employee);

                return Json(res, JsonRequestBehavior.AllowGet);


            }


            [HttpGet]
            [OverRidingAuthorizationAttribute]
            public JsonResult GetEmpById(int id)
            {

                Employees employee = _employeesRepository.GetAllEmployees().Single(e => e.Id==id);
                string res = JsonConvert.SerializeObject(employee);
                return Json(res, JsonRequestBehavior.AllowGet);

            }

            [HttpPost]
            [OverRidingAuthorizationAttribute]
            public JsonResult EditEmp(Employees emp)
            {
                _employeesRepository.Edit(emp);
                string res = JsonConvert.SerializeObject(emp);
                return Json(res);
            }

            [HttpGet]
           
            public JsonResult LoadDropDownList()
            {
                var dept = _departmentsRepository.GetAllDepartments();

                string res = JsonConvert.SerializeObject(dept, Formatting.Indented, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });

                return Json(res, JsonRequestBehavior.AllowGet);
            }



            [HttpPost]
            [OverRidingAuthorizationAttribute]
            public JsonResult DeleteEmp(int id)
            {
                Employees emp = _employeesRepository.GetAllEmployees().Single(e => e.Id == id);
               _employeesRepository.DeleteEmp(id);
               string res = JsonConvert.SerializeObject(emp);
               return Json(res);

            }

      


        ////---------------------------------------------------------------------departmentcode begins------------------------------------------------
         [OverRidingAuthorizationAttribute]
            public JsonResult CreateDepartment(Departments department)
            {
                string res = "";

                if (ModelState.IsValid) 
                { 
                    _departmentsRepository.CreateDepartment(department);
                    res = JsonConvert.SerializeObject(department);
                }
               
                return Json(res);
                
            }


            //public JsonResult GetAllDepartments()
            //{

            //    IEnumerable<Departments> departments = _departmentsRepository.GetAllDepartments();

            //    string res = JsonConvert.SerializeObject(departments);

            //    return Json(res, JsonRequestBehavior.AllowGet);


            //}

            [HttpGet]
            [OverRidingAuthorizationAttribute]
            public JsonResult GetAllEmployeesByDepartments()
            {

            var employees = from department in _departmentsRepository.GetAllDepartments()
                            join employee in _employeesRepository.GetAllEmployees()
                                on department.Id equals employee.DepartmentId
                                into employeeGroup
                            select new DepartmentTotal { Id = department.Id, Name = department.Name, Total = employeeGroup.Count() };

            string res = JsonConvert.SerializeObject(employees);

                return Json(res, JsonRequestBehavior.AllowGet);
            }

         [HttpPost]
         [OverRidingAuthorizationAttribute]
         public JsonResult DeleteDept(int id)
          {
            Departments dept = _departmentsRepository.GetAllDepartments().Single(e=>e.Id==id);
           _departmentsRepository.Delete(id);
            string res = JsonConvert.SerializeObject(dept);
            return Json(res);
          }

        [HttpPost]
        [OverRidingAuthorizationAttribute]
        public JsonResult EditDept(Departments dept)
        {
            _departmentsRepository.EditDept(dept);
            string res = JsonConvert.SerializeObject(dept);
            return Json(res);
        }
         [OverRidingAuthorizationAttribute]
        public JsonResult GetDeptById(int id)
            {

                Departments department = _departmentsRepository.GetAllDepartments().Single(dept => dept.Id==id);
                string res = JsonConvert.SerializeObject(department, Formatting.Indented,
                                                                                        new JsonSerializerSettings
                                                                                        {
                                                                                            PreserveReferencesHandling = PreserveReferencesHandling.Objects
                                                                                        });
                return Json(res, JsonRequestBehavior.AllowGet);
            }


    }
}