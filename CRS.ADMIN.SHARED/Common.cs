namespace CRS.ADMIN.SHARED
{
    public class Common
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }
        public string IpAddress { get; set; }
        public string BrowserInfo { get; set; }
        public string ActionUser { get; set; }
        public string ActionUserId { get; set; }
        public string ActionDate { get; set; }
        public string ActionPlatform { get; set; } = "ADMIN";
        public string ActionIP { get; set; }
    }
    public class ApplicationConfigCommon
    {
        public string ConfigLabel { get; set; }
        public string ConfigValue { get; set; }
        public string ConfigValue1 { get; set; }
        public string ConfigValue2 { get; set; }
        public string ConfigValue3 { get; set; }
        public string ConfigValue4 { get; set; }
        public string ConfigValue5 { get; set; }
    }

    public class StaticDataCommon
    {
        public string StaticValue { get; set; }
        public string StaticLabel { get; set; }
        public string StaticLabelJapanese { get; set; }
        public string StaticLabelEnglish { get; set; }
    }
}
