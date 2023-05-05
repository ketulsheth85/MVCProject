using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC_Demo.Enums;
using Newtonsoft.Json;

namespace MVC_Demo.Controllers
{
    public class GameMappingController : Controller
    {
        UserParameter UserList = new UserParameter();
        DeviceGameMappingModelVM ModelVM = new DeviceGameMappingModelVM();
        List<CommanEnum> commanEnum = new List<CommanEnum>();
        // GET: GameMappingController

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
            ModelVM.Op = 1;
            ModelVM.UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            var deviceGameMappingList = DeviceGameMappingDataAccess.GetDeviceMappingList(ModelVM);
            return View(deviceGameMappingList);
        }


        // GET: GameMappingController/Create
        public ActionResult CreateDeviceGameMapping(int id)
        {
            if (id != 0)
            {
                ModelVM.Op = 2;
                ModelVM.MappingID = id;
                var dbDeviceInfo = DeviceGameMappingDataAccess.GetDeviceMappingData(ModelVM);
                dbDeviceInfo.minute = dbDeviceInfo.GameTime.Minutes.ToString();
                dbDeviceInfo.Seconds = dbDeviceInfo.GameTime.Seconds.ToString();
                ViewBag.UserId = dbDeviceInfo.UserID;
                ViewBag.DeviceInfoData = dbDeviceInfo;
            }
            DeviceInfoModelVM device = new DeviceInfoModelVM();
            device.Op = 1;
            if (id == 0)
            {
                device.UserID = (int)HttpContext.Session.GetInt32("UserId");
            }
            else
            {
                device.UserID = ViewBag.UserId;
            }
            ViewBag.DeviceData = DeviceInfoDataAccess.GetDeviceInfoList(device).ToList();
            //GameMasterVM game = new GameMasterVM();
            dynamic dynamic = new System.Dynamic.ExpandoObject();
            dynamic.Op = 1;
            //dynamic.UserID = HttpContext.Session.GetInt32("UserId");
            if(id !=0)
            {
                dynamic.UserID= ViewBag.UserId;
                dynamic.Op = 7;
            }
            ViewBag.GameData = GameDataAccess.GetGameList(dynamic);
            UserList.op = 5;
            //UserList.UserID = id;
            UserList.InActiveDate = DateTime.Now;
            ViewBag.UserData = DataAccess.GetList(UserList);
            //ViewBag.GameData = GameDataAccess.GetGameList(game).Where(p => p.InActiveDate != null).ToList();
            for (int i = 0; i < 60; i++)
            {
                CommanEnum minutes = new CommanEnum();
                minutes.minute = i.ToString();
                minutes.minuteVlaue = i.ToString();
                commanEnum.Add(minutes);
            }
            ViewBag.MinuteData = commanEnum.ToList();
            ViewBag.SecondData = commanEnum.ToList();
            return View();
        }

        // POST: GameMappingController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]  

        //public ActionResult CreateDeviceGameMapping(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        var time = "00:" + collection["ddlMinuteId"].ToString() + ":" + collection["ddlSecondId"].ToString();
        //        var cTime = TimeSpan.Parse(time);
        //        if (id == 0)
        //        {                   
        //            ModelVM.Op = 3;
        //            ModelVM.DeviceID = Convert.ToInt32(collection["ddlDeviceId"].ToString());
        //            ModelVM.GameID = Convert.ToInt32(collection["ddlGameId"].ToString());
        //            ModelVM.GameTime = cTime;
        //            ModelVM.GameAmount = Convert.ToDecimal(collection["gameAmount"].ToString());
        //            var deviceData = DeviceGameMappingDataAccess.CRUDDeviceMapping(ModelVM);
        //            return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {                   
        //            ModelVM.Op = 2;
        //            ModelVM.MappingID = id;
        //            var Models = DeviceGameMappingDataAccess.GetDeviceMappingData(ModelVM);
        //            //ModelVM.MappingID = Models.GameID;
        //            //ModelVM.MappingID = id;
        //            ModelVM.Op = 4;
        //            ModelVM.DeviceID = Convert.ToInt32(collection["ddlDeviceId"]);
        //            ModelVM.GameID = Convert.ToInt32(collection["ddlGameId"].ToString());
        //            ModelVM.GameTime = cTime;
        //            ModelVM.GameAmount = Convert.ToDecimal(collection["gameAmount"].ToString());
        //            var deviceData = DeviceGameMappingDataAccess.CRUDDeviceMapping(ModelVM);
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        return View();
        //    }
        //}

        // GET: GameMappingController/Edit/

        // GET: GameMappingController/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDeviceGameMapping(int id, IFormCollection collection)
        {
            try
            {
                var time = "00:" + collection["ddlMinuteId"].ToString() + ":" + collection["ddlSecondId"].ToString();
                var cTime = TimeSpan.Parse(time);
                //if (Convert.ToInt32(collection["deviceID"]) == 0)
                if(id == 0)
                {
                    ModelVM.Op = 3;
                    ModelVM.DeviceID = Convert.ToInt32(collection["ddlDeviceId"].ToString());
                    ModelVM.GameID = Convert.ToInt32(collection["ddlGameId"].ToString());
                    ModelVM.GameTime = cTime;
                    ModelVM.GameAmount = Convert.ToDecimal(collection["gameAmount"].ToString());
                    var deviceData = DeviceGameMappingDataAccess.CRUDDeviceMapping(ModelVM);
                    return RedirectToAction(nameof(Index), new { stringData = deviceData.UpdateData });
                }
                else
                {
                    ModelVM.Op = 2;
                    ModelVM.MappingID = id;
                    //var Models = DeviceGameMappingDataAccess.GetDeviceMappingData(ModelVM);
                    //ModelVM.MappingID = Models.GameID;
                    ModelVM.Op = 4;
                    ModelVM.DeviceID = Convert.ToInt32(collection["ddlDeviceId"].ToString());
                    ModelVM.GameID = Convert.ToInt32(collection["ddlGameId"].ToString());
                    ModelVM.GameTime = cTime;
                    ModelVM.GameAmount = Convert.ToDecimal(collection["gameAmount"].ToString());
                    var deviceData = DeviceGameMappingDataAccess.CRUDDeviceMapping(ModelVM);
                    return RedirectToAction(nameof(Index), new { stringData = deviceData.UpdateData });
                }
            }
            catch (Exception ex)
            {
                
                return View();
            }
        }
        public JsonResult Delete(int id, int ischecked)
        {
            ModelVM.MappingID = id;
            ModelVM.IsCheckActive = ischecked;
            ModelVM.Op = 5;
            var userModel = DeviceGameMappingDataAccess.CRUDDeviceMapping(ModelVM);
            return Json("");
        }

        [HttpGet]
        public JsonResult GetDevices(string UserId)
        {
            DeviceGameMappingCls deviceGameMappingCls = new DeviceGameMappingCls();

            dynamic dynamic = new System.Dynamic.ExpandoObject();
            dynamic.Op = 1;
            dynamic.UserId = UserId;
            deviceGameMappingCls.deviceInfoModels = DeviceInfoDataAccess.GetDeviceInfoLists(dynamic);

            dynamic.Op = 7;
            dynamic.UserId = UserId;
            deviceGameMappingCls.gameMasters = GameDataAccess.GetGameLists(dynamic);
            return Json(JsonConvert.SerializeObject(deviceGameMappingCls));
        }

        //[HttpGet]
        //public JsonResult GetGames(string UserId)
        //{
        //    dynamic dynamic = new System.Dynamic.ExpandoObject();
        //    dynamic.Op = 7;
        //    dynamic.UserId = UserId;
        //    return Json(GameDataAccess.GetGameLists(dynamic));
    }
}

public class DeviceGameMappingCls
{
    public List<DeviceInfoModel> deviceInfoModels;
    public List<GameMaster> gameMasters;
}


