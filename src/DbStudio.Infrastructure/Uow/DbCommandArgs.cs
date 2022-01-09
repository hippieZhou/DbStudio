namespace DbStudio.Infrastructure.Uow
{
    public class DbCommandArgs
    {
        /// <summary>
        /// 执行SQL
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// SQL 参数
        /// </summary>
        public object Param { get; set; }

        /// <summary>
        /// 超时时长（如果想无限等待，默认设置为 0 即可）
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// 是否需要事务（默认不需要）
        /// </summary>
        public bool RequiresTransaction { get; set; }
    }
}