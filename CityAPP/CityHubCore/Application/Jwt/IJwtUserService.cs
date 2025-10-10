using System.Threading.Tasks;

namespace CityHubCore.Application.Jwt {
    public interface IJwtUserService {
        Task<JwtUserInfo> GetById(string companyId, string userId);
    }
}