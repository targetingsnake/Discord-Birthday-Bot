using Common.Cfg;
using Database.Con;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class DatabaseConnector
    {
        public static baseConnector instanze {  
            get {
                if (_instanze == null)
                {
                    throw new NullReferenceException("Database Connection not initialized");
                }
                return _instanze;
            }    
        }

        private static baseConnector _instanze = null;

        public static void connect(config cfg)
        {
            _instanze = new baseConnector(cfg);
        }

    }
}
