using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Demo.Controllers
{
    public class DeviceController : Controller
    {
        DeviceModelVM device = new DeviceModelVM();
        DeviceInfoModelVM ModelVM = new DeviceInfoModelVM();

        // GET: DeviceController
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
            
            if(stringData != "")
            {
                ViewBag.isSaved = stringData;
            }
            device.Op = 1;
            device.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            var deviceList = DeviceLocationAccess.GetDeviceLocationList(device);
            return View(deviceList);
        }

        [HttpGet]
        // GET: DeviceController/CreateDevice
        public ActionResult CreateDevice(int id)
        {
            if (id != 0)
            {
                device.Op = 2;
                device.LocationID = id;
                var dbDevice = DeviceLocationAccess.GetDeviceLocationData(device);
                ViewBag.UserId = dbDevice.UserID;
                ViewBag.DeviceData = dbDevice;
                //return View();
            }
            else
            {
                DeviceModelVM DeviceModel = new DeviceModelVM();
                ViewBag.DeviceData = DeviceModel;
                //return View();
            }
            UserParameter UserList = new UserParameter();
            int UserRoleID = Convert.ToInt32(HttpContext.Session.GetInt32("UserRoleID"));
            ViewBag.LoggedInUserRole = UserRoleID;
            if(id == 0)
            {
                ModelVM.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            }
            else
            {
                ModelVM.UserID = ViewBag.UserId;
            }
            
            ModelVM.Op = 1;
            List<DeviceInfoModel> deviceInfoList = DeviceInfoDataAccess.GetDeviceInfoList(ModelVM);
            ViewBag.DeviceList = deviceInfoList;
            UserList.op = 5;
            UserList.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            UserList.InActiveDate = DateTime.Now;
            List<UserModel> userList = DataAccess.GetList(UserList);
            ViewBag.UserData = userList;
            return View();

        }


        // POST: DeviceController/CreateDevice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDevice(int id, IFormCollection collection)
        {
            try
            {
                if (Convert.ToInt32(collection["deviceID"]) == 0)
                {
                    device.Op = 3;
                    //device.UserID = Convert.ToInt32(collection["userlist"].ToString());
                    device.LocationName = collection["deviceName"].ToString();
                    device.LocationAddress = collection["dviceAddress"].ToString();
                    device.City = collection["deviceCity"].ToString();
                    device.State = collection["deviceState"].ToString();
                    device.Country = collection["deviceCountry"].ToString();
                    device.ZipCode = collection["deviceZipCode"].ToString();
                    device.DeviceID = collection["devicelist"].ToString();
                    var deviceData = DeviceLocationAccess.CRUDDeviceLocation(device);
                    return RedirectToAction(nameof(Index),new {stringData = deviceData.UpdateData });
                }
                else
                {
                    device.Op = 2;
                    device.LocationID = id;
                    device = DeviceLocationAccess.GetDeviceLocationData(device);
                    device.Op = 4;
                    device.LocationName = collection["deviceName"].ToString();
                    device.LocationAddress = collection["dviceAddress"].ToString();
                    device.City = collection["deviceCity"].ToString();
                    device.State = collection["deviceState"].ToString();
                    device.Country = collection["deviceCountry"].ToString();
                    device.ZipCode = collection["deviceZipCode"].ToString();
                    device.DeviceID = collection["devicelist"].ToString();
                    var deviceModel = DeviceLocationAccess.CRUDDeviceLocation(device);
                    return RedirectToAction(nameof(Index), new { stringData = deviceModel.UpdateData });
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        // GET: DeviceController/Delete/5
        public JsonResult Delete(int id, int ischecked)
        {
            device.LocationID = id;
            device.IsCheckActive = ischecked;
            device.Op = 5;
            var userModel = DeviceLocationAccess.CRUDDeviceLocation(device);
            return Json("");
        }


        [HttpGet]
        public JsonResult GetDevices(string UserId)
        {
            dynamic dynamic = new System.Dynamic.ExpandoObject();
            dynamic.Op = 1;
            dynamic.UserId = UserId;
            return Json(DeviceInfoDataAccess.GetDeviceInfoLists(dynamic));
        }

        [HttpPost]
        public ActionResult Submit(IFormCollection collection)
        {
            ModelVM.UserID = Convert.ToInt32(collection["userlist"].ToString());
            ModelVM.Op = 1;
            List<DeviceInfoModel> deviceInfoList = DeviceInfoDataAccess.GetDeviceInfoList(ModelVM);
            TempData["DeviceList"] = JsonConvert.SerializeObject(deviceInfoList);
            TempData.Keep("DeviceList");
            return RedirectToAction("CreateDevice", "Device", new { id = 0, deviceId = ModelVM.UserID });
        }
    }
}
