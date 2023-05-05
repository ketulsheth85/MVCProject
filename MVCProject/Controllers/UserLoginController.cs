using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Demo.Controllers
{
    public class UserLoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(IFormCollection collection)
        {
            UserParameter user = new UserParameter();
            if (Convert.ToString(collection["RegDevice"]).Length > 0)
            {
                dynamic dynamic = new System.Dynamic.ExpandoObject();
                dynamic.UserName = collection["RegUserName"].ToString();
                dynamic.FirstName = collection["RegFirstName"].ToString();
                dynamic.LastName = collection["RegLastName"].ToString();
                dynamic.Address = collection["RegAddress"].ToString();
                dynamic.Password = collection["RegPassword"].ToString();
                dynamic.DeviceCode = collection["RegDevice"].ToString();

                var userData = (UserModel)DataAccess.RegisterUseWithDevice(dynamic);
                if (userData == null)
                {
                    TempData["Error"] = "Device Code is not matched with system. ";
                    return View();
                }
                else
                {
                    user.UserName = collection["RegUserName"].ToString();
                    user.Password = collection["RegPassword"].ToString();
                }
            }
            else
            {
                user.UserName = collection["userName"].ToString();
                user.Password = collection["userPassword"].ToString();

            }
            //user.UserName = collection["userName"].ToString();
            //user.Password = collection["userPassword"].ToString();
            user.op = 6;
            var usersData = DataAccess.GetList(user).FirstOrDefault();
            if (usersData != null)
            {
                HttpContext.Session.SetString("UserName", usersData.UserName);
                HttpContext.Session.SetInt32("UserId", usersData.UserID);
                HttpContext.Session.SetString("UserFullName", usersData.FirstName + usersData.LastName);
                HttpContext.Session.SetString("UserRole", usersData.RoleName);
                HttpContext.Session.SetInt32("UserRoleID", usersData.UserRole);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = "please insert correct User Name or Password ";
                return View();
            }
        }
    }
}
