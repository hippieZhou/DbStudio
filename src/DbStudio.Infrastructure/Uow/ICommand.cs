using System.Data;

namespace DbStudio.Infrastructure.Uow
{
    public interface ICommand
    {
        /// <summary>
        /// 执行SQL
        /// </summary>
        string Sql { get; }

        /// <summary>
        /// SQL 参数
        /// </summary>
        object Param { get; }

        /// <summary>
        /// 超时时长（如果想无限等待，默认设置为 0 即可）
        /// </summary>
        int CommandTimeout { get; }

        /// <summary>
        /// 是否需要事务（默认不需要）
        /// </summary>
        bool RequiresTransaction { get; }
    }
}