using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model;
using System.Linq;
using MomNom_Backend.Model.Db;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using MySqlConnector;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedureTest
    {
            private readonly MomNomContext _context;

            public ProcedureTest(MomNomContext context)
            {
                _context = context;
            }
        
        [HttpPost]
        public async Task<ProcedureTestOutput> Post(string appendString)
        {

            using (var context = _context)
            {
                var appendStringParameter = new MySqlParameter("@appendString", "test");

                var result = context.procedureTestResults
                    .FromSqlRaw<ProcedureTestOutput>("CALL TestProcedure(@appendString)", appendStringParameter)
                    .ToList();
                return result.FirstOrDefault();
            }
        }
        
    }
}
