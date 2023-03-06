# Database Models

<h4>Models</h4>
<p>
Database Models
</p>

```
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
```

```
using System;
namespace Database.Models
{
    public class Page
    {
        public int pageSize { get; set; }
        public int pages { get; set; }
        public int currentPageNo { get; set; }
        public int rows { get; set; }
        public string action { get; set; }
    }
}

```