using System.Text;

namespace C4I.Applications.SCE.Interfaces.CoordinateSystems
{
    public static class CoordinateSystemWKTVerficationHelper
    {
        public static readonly WKTVariations WGS84WKTVariations;
        public static readonly WKTVariations UTM40WKTVariations;

        static CoordinateSystemWKTVerficationHelper()
        {
            string[] wgs84variations = { "wgs1984", "wgs84" };
            WGS84WKTVariations = new WKTVariations() { Variations = wgs84variations };

            string[] utmzone40variations = { "utm40", "utmzone40" };
            UTM40WKTVariations = new WKTVariations() { Variations = wgs84variations };
        }

        public static bool Verifiy(string WKT, WKTVariations datum, WKTVariations projection)
        {
            bool retval = false;
            try
            {

                StringBuilder sb = new StringBuilder(WKT);
                StringBuilder trimmedProj = new StringBuilder();

                for (int i = 0; i < WKT.Length; i++)
                {
                    if (char.IsLetterOrDigit(sb[i]))
                    {
                        trimmedProj.Append(sb[i]);
                    }
                }

                WKT = trimmedProj.ToString().ToLower();

                //You must have datum so null check.
                bool containsDatum = false;

                for (int i = 0; i < datum.Variations.Length && !containsDatum; i++)
                {
                    containsDatum = WKT.Contains(datum.Variations[i]);
                }
                retval = containsDatum;
                //You dont have to have a projection (utm) so null check.
                if (projection != null)
                {
                    bool containsProjection = false;

                    for (int i = 0; i < datum.Variations.Length && !containsProjection; i++)
                    {
                        containsProjection = WKT.Contains(datum.Variations[i]);
                    }
                    retval &= containsProjection;
                }

                return retval;
            }
            catch //(Exception ex)
            {
            }

            return retval;
        }
    }

    public class WKTVariations
    {
        public string[] Variations
        {
            get;
            internal set;
        }
    }


}
