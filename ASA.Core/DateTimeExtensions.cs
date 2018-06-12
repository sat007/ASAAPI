using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
   public static class DateTimeExtensions
    {
        //public static CultureInfo GetCultureInfo(string info)
        //{
        //    CultureInfo ci = new CultureInfo(info);
        //    return ci.
        //}
        //public static DateTime ToDateTime(this string s,
        //          string format = "{0:yyyy-MM}", string cultureString = "tr-TR")
        //{
        //    try
        //    {
        //        var r = DateTime.ParseExact(
        //            s: s,
        //            format: format,
        //            provider: GetCultureInfo(cultureString));
        //        return r;
        //    }
        //    catch (FormatException)
        //    {
        //        throw;
        //    }
        //    catch (CultureNotFoundException)
        //    {
        //        throw; // Given Culture is not supported culture
        //    }
        //}

        public static DateTime ToDateTime(this string s
                    )
        {
            try
            {
                var r = DateTime.Parse(s);
                return r;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (CultureNotFoundException)
            {
                throw; // Given Culture is not supported culture
            }

        }

    }
}
