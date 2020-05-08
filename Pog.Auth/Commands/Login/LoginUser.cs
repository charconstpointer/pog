using MediatR;

namespace Pog.Auth.Commands.Login
{
    public class LoginUser : IRequest<object>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Resource { get; set; }
    }
}