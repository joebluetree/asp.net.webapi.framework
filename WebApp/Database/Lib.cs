using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public static class Lib
    {
        public static int getTotalPages(int Rows, int PageSize)
        {
            int Pages = (Rows / PageSize);
            if (Rows < PageSize)
                Pages = 1;
            else if ((Pages * PageSize) != Rows)
                Pages++;
            return Pages;
        }

        public static int getStartRow(int currentPageNo, int pageSize)
        {
            return  (currentPageNo - 1) * pageSize + 1;
        }

        public static int getEndRow(int currentPageNo, int pageSize)
        {
            return  currentPageNo * pageSize;
        }

        public static int FindPage(string Action, int CurrentPageNo, int Pages)
        {
            if (Action == "NEXT")
                CurrentPageNo++;
            if (Action == "PREV")
                CurrentPageNo--;
            if (Action == "FIRST")
                CurrentPageNo = 1;
            if (Action == "LAST")
                CurrentPageNo = Pages;
            if (CurrentPageNo < 1)
                CurrentPageNo = 1;
            if (CurrentPageNo > Pages)
                CurrentPageNo = Pages;
            return CurrentPageNo;
        }
    }
}
