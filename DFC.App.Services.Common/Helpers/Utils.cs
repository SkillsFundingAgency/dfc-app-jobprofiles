namespace DFC.App.Services.Common.Helpers
{
    public static class Utils
    {
        public static string LoggerMethodNamePrefix([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return $"[{memberName}()]: ";
        }
    }
}
