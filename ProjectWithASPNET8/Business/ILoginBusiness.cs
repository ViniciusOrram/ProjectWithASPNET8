using ProjectWithASPNET8.Data.VO;

namespace ProjectWithASPNET8.Business
{
    public interface ILoginBusiness
    {
        TokenVO ValidateCredencials(UserVO user);
        TokenVO ValidateCredencials(TokenVO token);
        bool RevokeToken(string userName);

    }
}
