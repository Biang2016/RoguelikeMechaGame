namespace GameCore
{
    public static class Utils
    {
        public delegate void DebugLogDelegate(string log);

        public delegate void NoticeCenterMsgDelegate(string noticeStr);

        public static DebugLogDelegate DebugLog;
        public static NoticeCenterMsgDelegate NoticeCenterMsg;
    }
}