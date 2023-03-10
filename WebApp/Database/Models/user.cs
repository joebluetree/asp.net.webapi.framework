using System;
namespace Database.Models
{
    public class User
    {
        public int user_id { get; set; }
        public string user_code { get; set; }
        public string user_name { get; set; }
        public string user_password { get; set; }
        public string user_email { get; set; }
        public Boolean user_is_locked { get; set; }
        public Boolean user_is_admin { get; set; }
    }
    public class User_SearchData
    {
        public Page PageData { get; set; }
    }

}
