using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDVMedicaux.Dal
{
    public class Util
    {
        /// <summary>
        /// Permet d'obtenir la distance entre deux villes
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double distance = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            distance = Math.Acos(distance);
            distance = rad2deg(distance);
            distance = distance * 60 * 1.1515;
            distance = distance * 1.6093470878864446;

            return (distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
