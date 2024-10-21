using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Model;

namespace ProjectWithASPNET8.Repository
{
    public interface IUserRepository
    {
        User? ValidadeCredentials(UserVO user);
        User? ValidateCredentials(string username);
        User? RefreshUserInfo(User user);
        bool RevokeToken(string username);
    }
}
