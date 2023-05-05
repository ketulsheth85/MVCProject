using BusinessLogic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Demo.Enums
{
    public class CommonFunction
    {
        public List<UserModel> GetUserSession(string StrSession)
        {
            List<UserModel> userModel = JsonConvert.DeserializeObject<List<UserModel>>(StrSession);
            return userModel;
        } 
    }
}
