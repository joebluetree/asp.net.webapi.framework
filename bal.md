# Business Access Layer


<h4>Controller</h4>
<p>
Controller Class which is used to define the web api end points	
</p>

```
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
        [Route("Login")]
        public IHttpActionResult Login([FromUri] string code, [FromUri] string password)
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
        [Route("Save")]
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


```




<h4>Business Service</h4>
<p>
Business Service
</p>

```
using System;
using System.Data;  
using System.Collections.Generic;
using Database;
using Database.Models;

namespace UserAdmin.Services
{
    public class UserService : BaseService
    {
        public IDictionary<string, object> List(User_SearchData searchData)
        {
            List<User> mList = new List<User>();
            User rec;

            Page pageData = searchData.PageData;
            int StartRow = 0;
            int EndRow = 0;

            try
            {
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                if (pageData.action == "SEARCH")
                {
                    sql = " select count(*) as tot from userm ";
                    DataTable Dt_Count = new DataTable();
                    Dt_Count = Connection.ExecuteQuery(sql);
                    pageData.rows = int.Parse(Dt_Count.Rows[0]["tot"].ToString());
                    pageData.pages = Lib.getTotalPages(pageData.rows, pageData.pageSize);
                    pageData.currentPageNo = 1;
                }
                else
                {
                    pageData.currentPageNo = Lib.FindPage(pageData.action, pageData.currentPageNo, pageData.pages);
                }

                StartRow = Lib.getStartRow(pageData.currentPageNo, pageData.pageSize);
                EndRow = Lib.getEndRow(pageData.currentPageNo, pageData.pageSize);

                sql = "";
                sql += " select * from ( ";
                sql += " select row_number() over(order by user_code) rownum, ";
                sql += " user_id, user_code, user_name,user_password, user_email, user_is_locked, user_is_admin  ";
                sql += " from userm ";
                sql += ") a ";
                sql += " where rownum between " + StartRow.ToString() + " and " + EndRow.ToString() ;
                sql += " order by user_code";


                DataTable Dt_Temp = new DataTable();
                Dt_Temp = Connection.ExecuteQuery(sql);
                foreach (DataRow Dr in Dt_Temp.Rows)
                {
                    rec = new User();
                    rec.user_id = int.Parse(Dr["user_id"].ToString());
                    rec.user_code = Dr["user_code"].ToString();
                    rec.user_name = Dr["user_name"].ToString();
                    rec.user_password = "";
                    rec.user_email = Dr["user_email"].ToString();
                    rec.user_is_admin = Dr["user_is_admin"].ToString() == "Y" ? true : false;
                    rec.user_is_locked = Dr["user_is_locked"].ToString() == "Y" ? true : false;
                    mList.Add(rec);
                }

                RetData.Add("pageData", pageData);
                RetData.Add("status", true);
                RetData.Add("records", mList);
                return RetData;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public IDictionary<string, object> GetRecord(Dictionary<string, object> SearchData)
        {
            string id = "";
            User rec = null;
            Dictionary<string, object> RetData = new Dictionary<string, object>();
            try
            {
                id = SearchData["id"].ToString();
                sql = " select user_id, user_code, user_name,user_password, user_email, user_is_locked, user_is_admin  ";
                sql += " from userm where user_id = '{ID}'";
                sql = sql.Replace("{ID}", id);

                DataTable Dt_Temp = new DataTable();
                Dt_Temp = Connection.ExecuteQuery(sql);
                foreach (DataRow Dr in Dt_Temp.Rows)
                {
                    rec = new User();
                    rec.user_id = int.Parse(Dr["user_id"].ToString());
                    rec.user_code = Dr["user_code"].ToString();
                    rec.user_name = Dr["user_name"].ToString();
                    rec.user_password = Dr["user_password"].ToString();
                    rec.user_email = Dr["user_email"].ToString();
                    rec.user_is_admin = Dr["user_is_admin"].ToString() == "Y" ? true : false;
                    rec.user_is_locked = Dr["user_is_locked"].ToString() == "Y" ? true : false;
                }
                RetData.Add("status", true);
                RetData.Add("record", rec);
                return RetData;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public User GetUserRecord(Dictionary<string, object> SearchData)
        {
            string id = "";
            User rec = null;
            try
            {
                id = SearchData["id"].ToString();
                sql = " select user_id, user_code, user_name,user_password, user_email, user_is_locked, user_is_admin  ";
                sql += " from userm where user_id = '{ID}'";
                sql = sql.Replace("{ID}", id);

                DataTable Dt_Temp = new DataTable();
                Dt_Temp = Connection.ExecuteQuery(sql);
                foreach (DataRow Dr in Dt_Temp.Rows)
                {
                    rec = new User();
                    rec.user_id = int.Parse(Dr["user_id"].ToString());
                    rec.user_code = Dr["user_code"].ToString();
                    rec.user_name = Dr["user_name"].ToString();
                    rec.user_password = Dr["user_password"].ToString();
                    rec.user_email = Dr["user_email"].ToString();
                    rec.user_is_admin = Dr["user_is_admin"].ToString() == "Y" ? true : false;
                    rec.user_is_locked = Dr["user_is_locked"].ToString() == "Y" ? true : false;
                }
                return rec;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public User ValdiateUser(string code, string pwd)
        {
            User rec = null;
            try
            {
                sql = " select user_id, user_code, user_name,user_password, user_email, user_is_locked, user_is_admin  ";
                sql += " from userm where user_code = '{CODE}' and user_password = '{PWD}'";
                sql = sql.Replace("{CODE}", code.ToUpper());
                sql = sql.Replace("{PWD}", pwd.ToUpper());

                DataTable Dt_Temp = new DataTable();
                Dt_Temp = Connection.ExecuteQuery(sql);
                foreach (DataRow Dr in Dt_Temp.Rows)
                {
                    rec = new User();
                    rec.user_id = int.Parse(Dr["user_id"].ToString());
                    rec.user_code = Dr["user_code"].ToString();
                    rec.user_name = Dr["user_name"].ToString();
                    rec.user_password = Dr["user_password"].ToString();
                    rec.user_email = Dr["user_email"].ToString();
                    rec.user_is_admin = Dr["user_is_admin"].ToString() == "Y" ? true : false;
                    rec.user_is_locked = Dr["user_is_locked"].ToString() == "Y" ? true : false;
                }
                return rec;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public IDictionary<string, object> Save(int id, string Mode, User Record)
        {
            string sql = "";
            Dictionary<string, object> RetData = new Dictionary<string, object>();

            try
            {
                DBRecord dbRecord = new DBRecord();
                dbRecord.CreateRow("user", Mode, "user_id", Record.user_id.ToString(), true);
                dbRecord.InsertString("user_code", Record.user_code);
                dbRecord.InsertString("user_name", Record.user_name);
                dbRecord.InsertString("user_password", Record.user_password);
                dbRecord.InsertString("user_email", Record.user_email);
                dbRecord.InsertString("user_is_locked", Record.user_is_locked ? "Y" : "N");
                dbRecord.InsertString("user_is_admin", Record.user_is_admin ? "Y" : "N");

                sql = dbRecord.UpdateRow();

                Connection.BeginTransaction();
                Connection.ExecuteNonQuery(sql);

                if (id == 0)
                {
                    sql = "SELECT CAST(scope_identity() AS int)";
                    Record.user_id = int.Parse(Connection.ExecuteScalar(sql).ToString());
                }

                Connection.CommitTransaction();

                RetData.Add("status", true);
                RetData.Add("record", Record);
                return RetData;
            }
            catch (Exception Ex)
            {
                Connection.RollbackTransaction();
                throw Ex;
            }
        }
    }
}
```