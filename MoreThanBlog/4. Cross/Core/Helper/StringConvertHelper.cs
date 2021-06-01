using System.Globalization;
using Core.Errors;

namespace Core.Helper
{
    public static class StringConvertHelper
    {
        public static int ParseInt(this string value)
        {
            int result;
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            throw new MoreThanBlogException(nameof(ErrorCode.InvalidArgumentException), ErrorCode.InvalidArgumentException);
        }
    }
}