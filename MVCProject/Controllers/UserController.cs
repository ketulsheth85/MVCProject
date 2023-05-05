using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MVC_Demo.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BusinessLogic.Models;
using BusinessLogic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC_Demo.Controllers
{
    public class UserController : Controller
    {
        // GET: UserController

        UserParameter UserList = new UserParameter();


        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context != null)
            {
                var Data = HttpContext != null ? Convert.ToInt32(HttpContext.Session.GetInt32("UserId")) : 0;
                if (Data == 0)
                {
                    context.Result = new RedirectResult("/UserLogin/Login");
                }
            }
        }
        public ActionResult Index(string stringData)
        {
            if (stringData != "")
            {
                ViewBag.isSaved = stringData;
            }
            var UserLists = new List<UserModel>();
            string UserRole = HttpContext.Session.GetString("UserRole");
            if (!string.IsNullOrWhiteSpace(Convert.ToString(UserRole)))
            {
                if (Convert.ToString(UserRole).ToLower() == "user")
                {
                    UserList.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                    UserList.op = 4;
                    ViewBag.LoggedInUserRole = 3;
                }
                else
                {
                    UserList.op = 5;
                    ViewBag.LoggedInUserRole = 1;
                }
                //UserList.InActiveDate = DateTime.Now;
                UserList.InActiveDate = null;
                UserLists = DataAccess.GetList(UserList);
            }
            return View(UserLists);
        }

        // GET: UserController/UserCreate
        public ActionResult UserCreate(int id)
        {
            string UserRole = HttpContext.Session.GetString("UserRole");
            if (Convert.ToString(UserRole).ToLower() == "user")
            {
                ViewBag.LoggedInUserRole = 3;
            }
            else
            {
                ViewBag.LoggedInUserRole = 1;
            }
            if (id != 0)
            {
                UserList.op = 4;
                UserList.UserID = id;
                UserList.InActiveDate = DateTime.Now;
                var dbUser = DataAccess.GetUserData(UserList);
                ViewBag.UserData = dbUser;

                ViewBag.LoggedInUserRole = Convert.ToInt32(HttpContext.Session.GetInt32("UserRoleID"));
                return View();
            }
            else
            {
                return View();
            }
        }

        // POST: UserController/UserCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserCreate(int id, IFormCollection collection)
        {
            try
            {
                if (Convert.ToInt32(collection["userID"]) == 0)
                {

                    UserList.op = 1;
                    UserList.UserName = collection["userName"].ToString();
                    UserList.FirstName = collection["firstName"].ToString();
                    UserList.LastName = collection["lastName"].ToString();
                    UserList.Address = collection["address"].ToString();
                    UserList.UserRole = Convert.ToInt32(collection["role"].ToString());
                    UserList.InActiveDate = DateTime.Now;
                    var UserData = DataAccess.CRUDUsers(UserList);
                    return RedirectToAction(nameof(Index), new { stringData = UserData.UpdateData });
                }
                else
                {

                    UserList.op = 4;
                    UserList.UserID = id;
                    UserList = DataAccess.GetUserData(UserList);
                    UserList.op = 2;
                    UserList.UserName = collection["userName"].ToString();
                    UserList.FirstName = collection["firstName"].ToString();
                    UserList.LastName = collection["lastName"].ToString();
                    UserList.Address = collection["address"].ToString();
                    UserList.UserRole = Convert.ToInt32(collection["role"].ToString());
                    UserList.InActiveDate = DateTime.Now;
                    var userModel = DataAccess.CRUDUsers(UserList);
                    return RedirectToAction(nameof(Index), new { stringData = userModel.UpdateData });
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        // GET: UserController/Delete/5
        public JsonResult Delete(int id, int ischecked)
        {
            UserList.UserID = id;
            UserList.isChecked = ischecked;
            UserList.op = 3;
            var userModel = DataAccess.CRUDUsers(UserList);
            return Json("");
        }
    }
}
