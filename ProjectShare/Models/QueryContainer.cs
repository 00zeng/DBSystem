using System;
namespace ProjectShare.Models
{
    // 查询结果容器，可根据需要自由添加参数
    public class IdNamePair
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class IntIdNamePair
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class QContainer
    {
        public int idInt { get; set; }
        public DateTime? startTime1 { get; set; }
        public DateTime? startTime2 { get; set; }
        public DateTime? endTime1 { get; set; }
        public DateTime? endTime2 { get; set; }
    }
}
