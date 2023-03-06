using System;
namespace Database
{
    public class DBRecord
    {
        private string SQL1 = "";
        private string SQL2 = "";
        public string WHERE = "";
        private string Table_Name = "";
        private string Mode = "";
        private string PK_COLUMN = "";
        private string PK_VALUE = "";

        public void CreateRow(string _Table_Name, string _Mode, string PKey_Col, string pKey_Data, bool isIdentity = false)
        {
            Init();
            Table_Name = _Table_Name;
            Mode = _Mode;
            SetPrimaryKey(PKey_Col, pKey_Data, isIdentity);
        }
        private void Init()
        {
            SQL1 = "";
            SQL2 = "";
            WHERE = "";
            Table_Name = "";
            Mode = "";
            PK_COLUMN = "";
            PK_VALUE = "";
        }
        public string UpdateRow()
        {
            string str = "";
            if (Mode == "ADD")
            {
                str = "INSERT INTO {TNAME} ({COLUMNS}) VALUES ({VALUES})";
                str = str.Replace("{TNAME}", Table_Name);
                str = str.Replace("{COLUMNS}", SQL1);
                str = str.Replace("{VALUES}", SQL2);
            }
            if (Mode == "EDIT")
            {
                str = "UPDATE {TNAME} SET {VALUES} WHERE {WHERE} ";
                str = str.Replace("{TNAME}", Table_Name);
                str = str.Replace("{VALUES}", SQL1);
                str = str.Replace("{WHERE}", WHERE);
            }
            return str;
        }
        public void SetPrimaryKey(string FldName, string Data, Boolean IsIdetity)
        {
            PK_COLUMN = FldName;
            PK_VALUE = Data;
            if (Mode == "ADD")
            {
                if (!IsIdetity)
                    InsertData("STRING", FldName, Data);
            }
            else
            {
                if (WHERE != "")
                    WHERE += " AND ";
                WHERE += PK_COLUMN += "='" + PK_VALUE + "'";
            }
        }

        public void InsertString(string FldName, string Data, string CharacterCase = "")
        {
            if (Data != null)
            {
                Data = Data.Replace("'", "''");
                if (CharacterCase.Trim().ToUpper() == "U")
                    Data = Data.ToUpper();
                else if (CharacterCase.Trim().ToUpper() == "L")
                    Data = Data.ToLower();
            }
            InsertData("STRING", FldName, Data);
        }

        public void InsertNumeric(string FldName, string Data)
        {
            if (Data.Trim() == "")
                Data = "0";
            InsertData("NUMERIC", FldName, Data);
        }

        public void InsertFunction(string FldName, string Data)
        {
            InsertData("FUNCTION", FldName, Data);
        }
        public void InsertDate(string FldName, Object Data)
        {
            string sData = "";
            DateTime Dt;
            if (Data == null || Data.ToString() == "")
                sData = "NULL";
            else
            {
                //Dt = (DateTime)Data;
                Dt = DateTime.Parse(Data.ToString());
                sData = "'{DATE}'";
                sData = sData.Replace("{DATE}", Dt.ToString("yyyy-MM-dd"));
            }
            InsertData("DATE", FldName, sData);
        }

        public void InsertDateAndTime(string FldName, Object Data)
        {
            string sData = "";
            DateTime Dt;
            if (Data == null)
                sData = "NULL";
            else
            {
                //Dt = (DateTime)Data;
                Dt = DateTime.Parse(Data.ToString());
                sData = "'{DATE}'";
                sData = sData.Replace("{DATE}", Dt.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            InsertData("DATE", FldName, sData);
        }
        private void InsertData(string sType, string FldName, string Data)
        {
            if (Mode == "ADD")
            {
                if (SQL1 != "")
                    SQL1 += ",";
                SQL1 += FldName;
                if (SQL2 != "")
                    SQL2 += ",";
                if (sType == "STRING")
                    SQL2 += "'" + Data + "'";
                if (sType == "NUMERIC")
                    SQL2 += Data;
                if (sType == "FUNCTION")
                    SQL2 += "(" + Data + ")";
                if (sType == "DATE")
                    SQL2 += Data;
            }
            else
            {
                if (SQL1 != "")
                    SQL1 += ",";
                if (sType == "STRING")
                    SQL1 += FldName + "=" + "'" + Data + "'";
                if (sType == "NUMERIC")
                    SQL1 += FldName + "=" + Data;
                if (sType == "FUNCTION")
                    SQL1 += FldName + "=" + "(" + Data + ")";
                if (sType == "DATE")
                    SQL1 += FldName + "=" + Data;
            }
        }

        public void AddGeneralColumns(string Mode, string user_code)
        {
            if (Mode == "ADD")
            {
                InsertString("rec_locked", "N");
                InsertString("rec_created_by", user_code);
                InsertDateAndTime("rec_created_date", System.DateTime.Now);
            }
            else
            {
                InsertString("rec_edited_by", user_code);
                InsertDateAndTime("rec_edited_date", System.DateTime.Now);
            }
        }
    }
}
