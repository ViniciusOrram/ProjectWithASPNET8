using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectWithASPNET8.Business;
using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Model;

namespace ProjectWithASPNET8.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ILoginBusiness _loginBusiness;

        public AuthController(ILoginBusiness loginBusiness)
        {
            _loginBusiness = loginBusiness;
        }

        [HttpPost]
        [Route("signin")]
        public IActionResult Signin([FromBody] UserVO user)
        {
            if(user == null)
            {
                return BadRequest("Invalid client request");
            }

            var token = _loginBusiness.ValidateCredencials(user);

            if(token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] TokenVO tokenVo)
        {
            if (tokenVo is null)
            {
                return BadRequest("Invalid client request");
            }

            var token = _loginBusiness.ValidateCredencials(tokenVo);

            if (token == null)
            {
                return BadRequest("Invalid client request");
            }

            return Ok(token);
        }

        [HttpGet]
        [Route("revoke")]
        [Authorize("Bearer")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var result = _loginBusiness.RevokeToken(username);

            if (!result)
            {
                return BadRequest("Invalid client request");
            }

            return NoContent();
        }
    }
}
