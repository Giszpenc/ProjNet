using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Configuration;

namespace C4I.Applications.SCE.Configuration
{
    public class SceConfiguration
    {
        private static readonly SceConfiguration _instance = new SceConfiguration();

        public static SceConfiguration Instance { get { return _instance; } }

        private SceConfiguration()
        {
            SetDeafults();
            string wgsStr = ConfigurationManager.AppSettings["WGS"];
            if (!string.IsNullOrEmpty(wgsStr))
                WGS = int.Parse(wgsStr);

            string sridStr = ConfigurationManager.AppSettings["SRID"];
            if (!string.IsNullOrEmpty(sridStr))
                SRID = int.Parse(sridStr);

            string utmZone = ConfigurationManager.AppSettings["UTM_Zone"];
            if (!string.IsNullOrEmpty(utmZone))
                UTM_Zone = int.Parse(utmZone);
        }

        private void SetDeafults()  // Always default to D coordinates
        {
            WGS = 4326;
            SRID = 32640;
            UTM_Zone = 40;
        }

        public int WGS { get; private set; }
        public int SRID { get; private set; }
        public int UTM_Zone { get; private set; }
    }
}
