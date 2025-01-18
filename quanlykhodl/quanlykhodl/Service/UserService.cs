using Microsoft.AspNetCore.Authentication;
using quanlykhodl.ViewModel;
using System.Security.Claims;

namespace quanlykhodl.Service
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.SignOutAsync();
        }

        public string name()
        {
            string value = string.Empty;
            if (_httpContextAccessor != null)
            {
                value = _httpContextAccessor.HttpContext.User.FindFirstValue(Status.IDAUTHENTICATION);
            }

            return value;
        }
    }
}
