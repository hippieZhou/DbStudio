namespace DbStudio.Domain.Entities
{
    public class DbExec
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DatabaseName { get; set; }

        public int SessionID { get; set; }

        public string CommandType { get; set; }

        /// <summary>
        /// 还原脚本
        /// </summary>
        public string StatementText { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// 还原完成百分比
        /// </summary>
        public int CompletePercent { get; set; }

        public decimal ElapsedTime { get; set; }

        public decimal EstimatedCompletionTime { get; set; }

        public string LastWait { get; set; }

        public string CurrentWait { get; set; }
    }
}
