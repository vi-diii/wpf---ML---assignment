using Microsoft.AspNetCore.Mvc;
using MovieModel;
using MovieDataAccess;


namespace WebRcommendationClient.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            List<User> users = new UserConnector().GetAllUsers();
            return View(users);
        }
    }
}
