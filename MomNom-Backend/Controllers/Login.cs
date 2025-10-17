using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using MomNom_Backend;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly MomNomContext _context;

        public LoginController(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> LoginUser([FromBody] LoginRequest msuser)
        {
            try
            {
                var user = _context.MsUsers.Where((e)=> e.Email == msuser.Email).FirstOrDefault();

                if (user == null || BCrypt.Net.BCrypt.Verify(msuser.Password, user.PasswordHash) == false)
                {
                    throw new BadRequestException<LoginResponse>("Email and password is invalid or not exists");
                }

                var sessionId = await Auth.generateSessionId(_context, user.UserId);

                return new BaseResponse<LoginResponse>(new LoginResponse
                {
                    sessionId = sessionId
                });
            }
            catch (BaseException<LoginResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<LoginResponse>("Unexpected internal server error occured").toResponseOutput();
            }

        }

    }
}
