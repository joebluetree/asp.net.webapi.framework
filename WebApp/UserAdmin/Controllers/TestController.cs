using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserAdmin.Controllers
{
    [Authorize(Roles = "test")]
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("GetMessage")]
        public IHttpActionResult GetHello()
        {
            try
            {
                return Ok("Hello");
            }
            catch (Exception Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }
    }
}
