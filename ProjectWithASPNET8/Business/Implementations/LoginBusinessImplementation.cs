using ProjectWithASPNET8.Configurations;
using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Repository;
using ProjectWithASPNET8.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectWithASPNET8.Business.Implementations
{
    public class LoginBusinessImplementation : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfiguration _configuration;

        private IUserRepository _repository;
        private readonly ITokenService _tokenService;

        public LoginBusinessImplementation(TokenConfiguration configuration, IUserRepository repository, ITokenService tokenService)
        {
            _configuration = configuration;
            _repository = repository;
            _tokenService = tokenService;
        }

        public TokenVO ValidateCredencials(UserVO userCredentials)
        {
            //Receber as credenciais e validar no banco, se tiver correto, ele retornará um "user"
            var user = _repository.ValidadeCredentials(userCredentials);

            //Se for igual a null, retorna null
            if (user == null) return null;

            //Se não, ele gera as Claims, o AccessToken e o RefreshToken
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            //Setando o AccessToken e o Refresh no user que ele recuperou do banco
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpiry);

            //Atualizando as informações do usuario
            _repository.RefreshUserInfo(user);

            //Trazendo quando foi gerado o token
            DateTime createDate = DateTime.Now;

            //Setando quando irá expirar (now + qtd minutos no appsetings)
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            //Setando as informações do Token
            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public TokenVO ValidateCredencials(TokenVO token)
        {
            var accessToken = token.AccessToken;
            var refreshToken = token.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identity.Name;

            var user = _repository.ValidateCredentials(username);

            if (user == null || 
                user.RefreshToken != refreshToken || 
                user.RefreshTokenExpiryTime <= DateTime.Now) return null;

            accessToken = _tokenService.GenerateAccessToken(principal.Claims);

            refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            _repository.RefreshUserInfo(user);

            //Trazendo quando foi gerado o token
            DateTime createDate = DateTime.Now;

            //Setando quando irá expirar (now + qtd minutos no appsetings)
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );
        }

        public bool RevokeToken(string userName)
        {
            return _repository.RevokeToken(userName);
        }
    }
}
