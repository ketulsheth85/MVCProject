using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Demo.Controllers
{
    public class GameController : Controller
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

        // GET: GameController
        public ActionResult Index(string stringData)
        {
            if (stringData != "")
            {
                ViewBag.isSaved = stringData;
            }
            game.Op = 1;
            var gameList = GameDataAccess.GetGameList(game);
            return View(gameList);
        }


        // GET: GameController/CreateGame
        public ActionResult CreateGame(int id)
        {
            if (id != 0)
            {
                game.Op = 2;
                game.GameID = id;
                game.InActiveDate = DateTime.Now;
                var dbGame = GameDataAccess.GetGameData(game);
                ViewBag.GameData = dbGame;
                return View();
            }
            else
            {
                return View();
            }
        }

        // POST: GameController/CreateGame
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGame(int id, IFormCollection collection, IFormFile file, IFormFile banner)
        {
            try
            {
                if (Convert.ToInt32(collection["gameID"]) == 0)
                {
                    game.Op = 3;
                    game.GameName = collection["gameName"].ToString();
                    game.GameDesc = collection["gameDesc"].ToString();
                    game.ExeName = collection["gameExeName"].ToString();
                    

                    if (file != null)
                    {
                        string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Games" + file.FileName);
                        game.GamePath = @"Games/" + file.FileName;
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            file.CopyToAsync(stream);
                        }
                    }
                    if (banner != null)
                    {
                        game.GameBanner = banner.FileName;
                        string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "GameBanner" , banner.FileName);
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            banner.CopyToAsync(stream);
                        }
                    }

                    var UserData = GameDataAccess.CRUDGame(game);
                    return RedirectToAction(nameof(Index),new { stringData = UserData.UpdateData} );
                }
                else
                {

                    game.Op = 2;
                    game.GameID = id;
                    game = GameDataAccess.GetGameData(game);
                    game.Op = 4;
                    game.GameName = collection["gameName"].ToString();
                    game.GameDesc = collection["gameDesc"].ToString();
                    game.ExeName = collection["gameExeName"].ToString();

                    if (file != null)
                    {
                        string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Games" , file.FileName);
                        game.GamePath = @"Games/" + file.FileName;
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            file.CopyToAsync(stream);
                        }
                    }
                    if (banner != null)
                    {
                        game.GameBanner = banner.FileName;
                        string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "GameBanner" , banner.FileName);
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            banner.CopyToAsync(stream);
                        }
                    }

                    var userModel = GameDataAccess.CRUDGame(game);
                    return RedirectToAction(nameof(Index), new { stringData = userModel.UpdateData });
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: GameController/Delete/5
        public JsonResult Delete(int id, int ischecked)
        {
            game.GameID = id;
            game.isChecked = ischecked;
            game.Op = 5;
            var userModel = GameDataAccess.CRUDGame(game);
            return Json("");
        }

        [HttpPost]
        public async Task<IActionResult> UploadGame(IFormFile file)
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), file.Name);

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok();
        }

    }
}
