using System.Threading.Tasks;
using DbStudio.Application.Wrappers;
using MediatR;

namespace DbStudio.Application.Interfaces
{
    public interface IEventBus
    {
        Task<Response<T>> SendAsync<T>(IRequest<Response<T>> request);
    }
}