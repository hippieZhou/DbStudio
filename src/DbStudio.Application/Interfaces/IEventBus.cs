using DbStudio.Application.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Application.Interfaces
{
    public interface IEventBus
    {
        Task<Response<T>> SendAsync<T>(
            IRequest<Response<T>> request,
            CancellationToken cancellationToken = default,
            bool throwEx = true);
    }
}