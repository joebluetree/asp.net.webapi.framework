﻿using System;
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
