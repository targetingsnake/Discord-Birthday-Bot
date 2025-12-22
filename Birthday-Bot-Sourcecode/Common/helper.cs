using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class helper
    {
        public static string intArrayToBorthdayString(int[] data)
        {
            string birthday = data[0].ToString("00") + "." + data[1].ToString("00");
            if (data[2] != -1)
            {
                birthday += "." + data[2].ToString("0000");
            }
            return birthday;
        }
    }

    public struct Birthday
    {
        public Birthday(ulong _userID, long _lastPosted, int _year)
        {
            userID = _userID;
            lastPosted = _lastPosted;
            year = _year;
        }
        public ulong userID;
        public long lastPosted;
        public int year;
    }
}
