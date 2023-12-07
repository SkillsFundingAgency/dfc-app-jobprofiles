using System.ComponentModel;

namespace DFC.App.JobProfile.Enums
{
    public enum HomeOption
    {
        [Description("None")]
        None,
        [Description("Chat online")]
        Webchat,
        [Description("By email")]
        SendAMessage,
        [Description("Ask us to call you")]
        Callback,
        [Description("By post")]
        Sendletter,
    }
}