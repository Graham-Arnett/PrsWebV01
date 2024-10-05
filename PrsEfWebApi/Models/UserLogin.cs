using PrsEfWebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;


namespace PrsEfWebApi.Models
{
    public class UserLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
