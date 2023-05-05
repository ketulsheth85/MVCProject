using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC_Demo.Controllers
{
    public class UserMappingController : Controller
    {
        GameMasterVM game = new GameMasterVM();
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
        public IActionResult Index()
        {
            game.Op = 1;
            ViewBag.GameData = GameDataAccess.GetGameList(game);
            return View();
        }

        [HttpGet]
        public JsonResult GetUsers(string Gameid)
        {
            //ViewBag.UserData = DataAccess.GetUsersbyGame(Gameid);
            dynamic dynamic = new System.Dynamic.ExpandoObject();
            dynamic.Gameid = Gameid;
            dynamic.UpdateUser = false;
            return Json(DataAccess.GetUsersbyGame(dynamic));
        }

        [HttpGet]
        public JsonResult UpdateUser(string Gameid, string UserId, bool IsChecked)
        {
            //ViewBag.UserData = DataAccess.GetUsersbyGame(Gameid);
            dynamic dynamic = new System.Dynamic.ExpandoObject();
            dynamic.Gameid = Gameid;
            dynamic.UserId = UserId;
            dynamic.IsChecked = IsChecked;
            dynamic.UpdateUser = true;
            return Json(DataAccess.GetUsersbyGame(dynamic));
        }


        [HttpGet]
        public JsonResult GetLocation(string query)
        {
            List<DeviceModel> locations;
            List<UserLocationDeviceGameMapping> records;
            DeviceModelVM deviceModelVM = new DeviceModelVM();
            deviceModelVM.Op = 1;
            locations = DeviceLocationAccess.GetDeviceLocationList(deviceModelVM).ToList();

            if (!string.IsNullOrWhiteSpace(query))
            {
                locations = locations.Where(q => q.LocationName.Contains(query)).ToList();
            }

            records = locations.Where(l => l.InActiveDate != null).OrderBy(l => l.LocationID)
                .Select(l => new UserLocationDeviceGameMapping
                {
                    id = l.LocationID,
                    text = l.LocationName,
                    children = GetChildren(l.LocationID)
                }).ToList();


            return this.Json(records);
        }

        //public JsonResult LazyGet(int? parentId)
        //{
        //    List<Location> locations;
        //    List<Models.DTO.Location> records;
        //    using (ApplicationDbContext context = new ApplicationDbContext())
        //    {
        //        locations = context.Locations.ToList();

        //        records = locations.Where(l => l.ParentID == parentId).OrderBy(l => l.OrderNumber)
        //            .Select(l => new Models.DTO.Location
        //            {
        //                id = l.ID,
        //                text = l.Name,
        //                @checked = l.Checked,
        //                population = l.Population,
        //                flagUrl = l.FlagUrl,
        //                hasChildren = locations.Any(l2 => l2.ParentID == l.ID)
        //            }).ToList();
        //    }

        //    return this.Json(records, JsonRequestBehavior.AllowGet);
        //}

        private List<UserLocationDeviceGameMapping> GetChildren(int? parentId)
        {
            DeviceInfoModelVM deviceModelVM = new DeviceInfoModelVM();
            deviceModelVM.Op = 1;
            var locations = DeviceInfoDataAccess.GetDeviceInfoList(deviceModelVM).ToList();

            return locations.Where(l => l.LocationID == parentId).OrderBy(l => l.DeviceID)
                .Select(l => new UserLocationDeviceGameMapping
                {
                    id = l.DeviceID,
                    text = l.DeviceName,
                    @checked = false,
                    children = GetGameChildren(l.DeviceID)
                }).ToList();
        }
        private List<UserLocationDeviceGameMapping> GetGameChildren(int? parentId)
        {
            DeviceGameMappingModelVM deviceModelVM = new DeviceGameMappingModelVM();
            deviceModelVM.Op = 1;
            var locations = DeviceGameMappingDataAccess.GetDeviceMappingList(deviceModelVM).ToList();

            return locations.Where(l => l.DeviceID == parentId).OrderBy(l => l.MappingID)
                .Select(l => new UserLocationDeviceGameMapping
                {
                    id = l.MappingID,
                    text = l.GameName,
                    @checked = false
                }).ToList();
        }

        //[HttpPost]
        //public JsonResult SaveCheckedNodes(List<int> checkedIds)
        //{
        //    if (checkedIds == null)
        //    {
        //        checkedIds = new List<int>();
        //    }
        //    using (ApplicationDbContext context = new ApplicationDbContext())
        //    {
        //        var locations = context.Locations.ToList();
        //        foreach (var location in locations)
        //        {
        //            location.Checked = checkedIds.Contains(location.ID);
        //        }
        //        context.SaveChanges();
        //    }

        //    return this.Json(true);
        //}

        //[HttpPost]
        //public JsonResult ChangeNodePosition(int id, int parentId, int orderNumber)
        //{
        //    using (ApplicationDbContext context = new ApplicationDbContext())
        //    {
        //        var location = context.Locations.First(l => l.ID == id);

        //        var newSiblingsBelow = context.Locations.Where(l => l.ParentID == parentId && l.OrderNumber >= orderNumber);
        //        foreach (var sibling in newSiblingsBelow)
        //        {
        //            sibling.OrderNumber = sibling.OrderNumber + 1;
        //        }

        //        var oldSiblingsBelow = context.Locations.Where(l => l.ParentID == location.ParentID && l.OrderNumber > location.OrderNumber);
        //        foreach (var sibling in oldSiblingsBelow)
        //        {
        //            sibling.OrderNumber = sibling.OrderNumber - 1;
        //        }


        //        location.ParentID = parentId;
        //        location.OrderNumber = orderNumber;

        //        context.SaveChanges();
        //    }

        //    return this.Json(true);
        //}

        //public JsonResult GetCountries(string query)
        //{
        //    List<Models.DTO.Location> records;
        //    using (ApplicationDbContext context = new ApplicationDbContext())
        //    {
        //        records = context.Locations.Where(l => l.Parent != null && l.Parent.ParentID == null)
        //            .Select(l => new Models.DTO.Location
        //            {
        //                id = l.ID,
        //                text = l.Name,
        //                @checked = l.Checked,
        //                population = l.Population,
        //                flagUrl = l.FlagUrl
        //            }).ToList();
        //    }

        //    return this.Json(records, JsonRequestBehavior.AllowGet);
        //}
    }
}
