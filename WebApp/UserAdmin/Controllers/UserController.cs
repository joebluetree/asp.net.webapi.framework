using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Database.Models;
using UserAdmin.Services;

namespace UserAdmin.Controllers
{
    [Authorize]
    [RoutePrefix("api/user")]
    //[Authorize(Roles = "Administrator")]
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("getMessage")]
        public IHttpActionResult getMessage([FromUri] string code, [FromUri] string password)
        {
            try
            {
                return Ok(code + " " + password);
            }
            catch (Exception Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }

        [HttpPost]
        [Route("List")]
        public IHttpActionResult List( [FromBody] User_SearchData rec)
        {
            try
            {
                using (UserService obj  = new UserService())
                    return Ok( obj.List(rec ));
            }
            catch (Exception Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }

        [HttpPost]
        [Route("GetUserRecord")]
        public IHttpActionResult GetUserRecord(Dictionary<string, object> SearchData)
        {
            try
            {
                using (UserService obj = new UserService())
                    return Ok(obj.GetUserRecord(SearchData));
            }
            catch (Exception Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }


        [HttpPost]
        [Route("GetRecord")]
        public IHttpActionResult GetRecord(Dictionary<string, object> SearchData)
        {
            try
            {
                using (UserService obj = new UserService())
                    return Ok(obj.GetRecord(SearchData));
            }
            catch (Exception Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }


        [HttpPost]
        [Route("RegisterUser")]
        public IHttpActionResult Save( [FromUri] int id, [FromUri] string mode , [FromBody]  User _user)
        {
            try
            {
                return Ok();
            }
            catch ( Exception Ex )
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }

    }
    
}
