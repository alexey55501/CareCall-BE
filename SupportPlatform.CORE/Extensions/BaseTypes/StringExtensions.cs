using System.Linq;

namespace SupportPlatform.BLL.Infrastructure.Extensions.BaseTypes
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string ReplaceSpacesToLine(this string str)
        {
            return str.Trim().Replace(' ', '_');
        }

        public static string[] SplitStringBy(this string str, char[] characters = null)
        {
            if (characters == null) characters = new[] { ',', ';', ' ', '\n', '\t' };
            return str.Split(characters).Where(i => !i.Equals("")).ToArray();
        }
    }
}
