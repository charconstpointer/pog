using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pog.Auth.Commands.Login;

namespace Pog.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUser command)
            => Ok(await _mediator.Send(command));
    }
}