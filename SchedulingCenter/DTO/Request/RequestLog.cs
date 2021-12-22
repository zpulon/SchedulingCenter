namespace SchedulingCenter.DTO.Request
{
    public struct RequestLog
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string Ip { get; set; }
        public string Authorization { get; set; }
        public string ReqHeader { get; set; }
        public string Body { get; set; }
    }
}
