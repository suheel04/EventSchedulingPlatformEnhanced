namespace AccountService.Core.Helpers
{
    public static class MaskingHelper
    {
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            var atIndex = email.IndexOf("@");
            if (atIndex <= 2)
                return "****" + email.Substring(atIndex);

            return email.Substring(0, 2) + "****" + email.Substring(atIndex);
        }
    }

}
