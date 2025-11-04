using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Handler;
using MomNom_Backend.Model;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace MomNom_Backend.Controllers
{
    public class DetectionRequest
    {
        public string base64_img { get; set; }
    }

    public class DetectionResponse
    {
        public string status_code { get; set; }
        public string message { get; set; }
        public List<string> data { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FoodDetection : ControllerBase
    {
        private readonly MomNomContext _context;

        public FoodDetection(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<FoodDetectionResponse>>> detect([FromHeader] string authentication, [FromBody] FoodDetectionRequest req)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
                using HttpClient client = new HttpClient();
                string url = AppSettings.FoodDetectionEndpoint;

                var postData = new DetectionRequest{base64_img = req.imageBase64};
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(postData);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<DetectionResponse>(responseBody);

                if(res.status_code != "00")
                {
                    throw new InternalServerErrorException<FoodDetectionResponse>($"Food detection failed: {res.message}");
                }

                return new BaseResponse<FoodDetectionResponse>(new FoodDetectionResponse
                {
                    FoodNameList = res.data
                });
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<FoodDetectionResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<FoodDetectionResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<FoodDetectionResponse>("Unexpected internal server error occured").toResponseOutput();
            }
        }
    }
}
