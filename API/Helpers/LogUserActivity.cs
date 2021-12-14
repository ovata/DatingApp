using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resulContext = await next();

            if (!resulContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resulContext.HttpContext.User.GetUserId();
            var repo = resulContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            user.LasrActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}
