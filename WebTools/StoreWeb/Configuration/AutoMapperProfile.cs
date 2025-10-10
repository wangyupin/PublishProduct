using AutoMapper;

namespace StoreSrv.Configuration {
    /// <summary>
    /// REF https://www.pragimtech.com/blog/blazor/using-automapper-in-asp.net-core/
    /// </summary>
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<StoreWeb.ViewModels.User.LoginRequest, CityAdminDomain.Models.API.User.UserLoginRequest>();
            CreateMap<CityAdminDomain.Models.Common.UserBasicInfo, StoreWeb.ViewModels.User.LoginResponse>();
            CreateMap<CityAdminDomain.Models.Common.UserBasicInfo, StoreWeb.ViewModels.User.LoginUserData>();
        }
    }
} 
