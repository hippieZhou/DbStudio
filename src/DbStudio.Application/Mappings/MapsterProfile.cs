using DbStudio.Application.DTOs;
using DbStudio.Domain.Entities;
using Mapster;

namespace DbStudio.Application.Mappings
{
    public class MapsterProfile: TypeAdapterConfig
    {
        public MapsterProfile()
        {
            ForType<UserDbConnection, UserDbConnectionDto>()
                .Map(dest => dest.DataSource, src => src.DataSource)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.Password, src => src.Password);
        }
    }
}