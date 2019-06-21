using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class UtilityConvert
    {
        public static long ConvertMegaBytesToBytes(double megabytes)
        {
            var bytes = megabytes * (1024f * 1024f);
            return Convert.ToInt64(bytes);
        }

        public static string RandomString(int size, bool upperCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (upperCase)
                return builder.ToString().ToUpper();
            return builder.ToString();    
        }

        // Generate a random number between two numbers    
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }    
    }
}
