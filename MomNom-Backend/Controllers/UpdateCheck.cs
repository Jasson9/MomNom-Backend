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
    public class UpdateCheck : ControllerBase
    {
        private readonly MomNomContext _context;

        public UpdateCheck(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<CheckVersionResponse>>> checkUpdate([FromBody] CheckVersionRequest req)
        {
            try
            {
                var currVersion = _context.MsVersions.Where((e)=> e.Version == req.version).FirstOrDefault();

                if (currVersion == null)
                {
                    throw new BadRequestException<CheckVersionResponse>("Version not found");
                }

                var latestVersion = _context.MsVersions.Where(e=>e.VersionChannel==currVersion.VersionChannel).OrderByDescending(e => e.CreatedDatetime).FirstOrDefault();
            
                if(latestVersion == null)
                {
                    throw new BadRequestException<CheckVersionResponse>("Latest version not found");
                }

                return new BaseResponse<CheckVersionResponse>(new CheckVersionResponse
                {
                    isUpToDate = latestVersion.Version == currVersion.Version,
                    versionString = latestVersion.Version,
                    downloadLink = latestVersion.DownloadLink,
                    changelogs = latestVersion.Changelogs,
                });
            }
            catch (BaseException<CheckVersionResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<CheckVersionResponse>("Unexpected internal server error occured").toResponseOutput();
            }

        }

    }
}
