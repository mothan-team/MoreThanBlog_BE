using System.ComponentModel;

namespace Core.Constants.Enum
{
    public enum EmailTemplateType
    {
        [Description("Invite User")]
        InviteEmployee = 0,

        [Description("Forgot Password")]
        ForgotPass = 1,
    }
}