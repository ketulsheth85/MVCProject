using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MVC_Demo.Models.Hubs;

namespace MVC_Demo.Controllers
{
    public class DeviceInfoController : Controller
    {
        DeviceInfoModelVM ModelVM = new DeviceInfoModelVM();
        // GET: DeviceInfoController
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
        public ActionResult Index(string stringData = "")
        {
            if (stringData != "")
            {
                ViewBag.isSaved = stringData;
            }
            int UserRoleID = Convert.ToInt32(HttpContext.Session.GetInt32("UserRoleID"));
            ViewBag.LoggedInUserRole = UserRoleID;
            ModelVM.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            ModelVM.Op = 1;
            var deviceInfoList = DeviceInfoDataAccess.GetDeviceInfoList(ModelVM);
            return View(deviceInfoList);
        }

        // GET: DeviceInfoController/Create
        public ActionResult CreateDeviceInfo(int id)
        {
            if (id != 0)
            {
                ModelVM.Op = 2;
                ModelVM.DeviceID = id;
                var dbDeviceInfo = DeviceInfoDataAccess.GetDeviceInfoData(ModelVM);
                ViewBag.UserId = dbDeviceInfo.UserID;
                ViewBag.DeviceInfoData = dbDeviceInfo;
            }
            DeviceModelVM device = new DeviceModelVM();
            device.Op = 1;
            ViewBag.LocationData = DeviceLocationAccess.GetDeviceLocationList(device).Where(p => p.InActiveDate != null).ToList();
            UserParameter UserList = new UserParameter();
            UserList.op = 5;
            UserList.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            UserList.InActiveDate = DateTime.Now;
            List<UserModel> userList = DataAccess.GetList(UserList);
            ViewBag.UserData = userList;
            return View();
        }

        // POST: DeviceInfoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDeviceInfo(int id,int UserRoleID, IFormCollection collection)
        {
            try
            {
                var role = Convert.ToInt32(HttpContext.Session.GetInt32("UserRoleID"));
                if (id == 0)
                { 
                    ModelVM.Op = 7;
                    ModelVM.DeviceName = collection["deviceName"].ToString();
                    ModelVM.DeviceInfo = collection["dviceAddress"].ToString();
                    ModelVM.MachineID = collection["MachineID"].ToString();
                    ModelVM.loginId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                    ModelVM.UserID = role == 1 ? Convert.ToInt32(collection["userlist"].ToString()) : Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                    var deviceData = DeviceInfoDataAccess.CRUDDeviceInfo(ModelVM);
                    return RedirectToAction(nameof(Index), new { stringData = deviceData.UpdateData });
                }
                else
                {
                    //ModelVM.Op = 2;
                     ModelVM.DeviceID = id;
                   // ModelVM = DeviceInfoDataAccess.GetDeviceInfoData(ModelVM);
                    ModelVM.Op = 4;
                    ModelVM.DeviceName = collection["deviceName"].ToString();
                    ModelVM.DeviceInfo = collection["dviceAddress"].ToString();
                    //ModelVM.LocationID = Convert.ToInt32(collection["ddlLocationId"].ToString());
                    ModelVM.MachineID = collection["MachineID"].ToString();
                    ModelVM.loginId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                    ModelVM.UserID = role == 1 ? Convert.ToInt32(collection["userlist"].ToString()) : Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                    var deviceModel = DeviceInfoDataAccess.CRUDDeviceInfo(ModelVM);
                    return RedirectToAction(nameof(Index), new { stringData = deviceModel.UpdateData });
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        // GET: DeviceInfoController/Delete/5
        public JsonResult Delete(int id, int ischecked)
        {
            ModelVM.DeviceID = id;
            ModelVM.IsCheckActive = ischecked;
            ModelVM.Op = 5;
            var userModel = DeviceInfoDataAccess.CRUDDeviceInfo(ModelVM);
            return Json("");
        }

        [HttpGet]
        public JsonResult GetRefreshData(string stringData)
        {
            if (stringData != "")
            {
                ViewBag.isSaved = stringData;
            }
            int UserRoleID = Convert.ToInt32(HttpContext.Session.GetInt32("UserRoleID"));
            ViewBag.LoggedInUserRole = UserRoleID;
            ModelVM.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            ModelVM.Op = 1;
            var deviceInfoList = DeviceInfoDataAccess.GetDeviceInfoList(ModelVM);
            return Json(deviceInfoList);
        }
        //[HttpGet]
        //public IEnumerable<DeviceInfoModel> GetRefreshDatas()
        //{
        //    string _connString =
        //    ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        //    var messages = new List<DeviceInfoModel>();
        //    using (var connection = new SqlConnection(_connString))
        //    {
        //        connection.Open();
        //        using (var command = new SqlCommand(@"SELECT [OnlineStatus] FROM [dbo].[DeviceInfoTbl]", connection))
        //        {
        //            command.Notification = null;
        //            var dependency = new SqlDependency(command);
        //            dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
        //            if (connection.State == ConnectionState.Closed)
        //                connection.Open();
        //            var reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                messages.Add(item: new DeviceInfoModel
        //                {
        //                    DeviceID = (int)reader["DeviceID"],
        //                    OnlineStatus=(bool)reader["OnlineStatus"]
        //                });
        //            }
        //        }
        //    }
        //    return messages;
        //}
        //private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        //{
        //    if (e.Type == SqlNotificationType.Change)
        //    {
        //        MessageHubs.SendMessages();
        //    }
        //}
    }
}
