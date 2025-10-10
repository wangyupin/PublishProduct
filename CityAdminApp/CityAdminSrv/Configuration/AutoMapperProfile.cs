using AutoMapper;

namespace CityAdminSrv.Configuration {
    /// <summary>
    /// REF https://www.pragimtech.com/blog/blazor/using-automapper-in-asp.net-core/
    /// </summary>
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            //CreateMap<ViewModels.LoginRequest, SrvModels.UserLoginRequest>();
            //CreateMap<DBModels.UserBasicInfo, ViewModels.LoginResponse>();
            //CreateMap<DBModels.UserBasicInfo, ViewModels.LoginUserData>();
        }
    }
}
