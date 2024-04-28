using System.Collections.Generic;
using System.Linq;

namespace SupportPlatform.BLL.Helpers
{
    public static class StringComparisonHelper
    {
        public static float Compare(string a, string b)
        {
            a = a.ToLower();
            b = b.ToLower();

            float dis = ComputeDistance(a, b);
            float maxLen = a.Length;
            if (maxLen < b.Length)
                maxLen = b.Length;
            if (maxLen == 0.0F)
                return 1.0F;
            else
                return 1.0F - dis / maxLen;
        }


        private static int ComputeDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] distance = new int[n + 1, m + 1]; // matrix
            int cost = 0;
            if (n == 0) return m;
            if (m == 0) return n;
            //init1
            for (int i = 0; i <= n; distance[i, 0] = i++) ;
            for (int j = 0; j <= m; distance[0, j] = j++) ;
            //find min distance
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    cost = (t.Substring(j - 1, 1) ==
                        s.Substring(i - 1, 1) ? 0 : 1);
                    distance[i, j] = new List<int>{distance[i - 1, j] + 1,
                    distance[i, j - 1] + 1,
                    distance[i - 1, j - 1] + cost }.Min();
                }
            }
            return distance[n, m];
        }
    }
}
