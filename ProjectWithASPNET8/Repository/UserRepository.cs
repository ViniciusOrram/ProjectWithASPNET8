using Microsoft.EntityFrameworkCore;
using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Model;
using ProjectWithASPNET8.Model.Context;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ProjectWithASPNET8.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlContext _context;

        public UserRepository(MySqlContext context)
        {
            _context = context;
        }

        public User? ValidadeCredentials(UserVO user)
        {
            var pass = ComputeHash(user.Password, SHA256.Create());
            return _context.Users.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == pass));
        }

        public User? ValidateCredentials(string userName)
        {
            return _context.Users.SingleOrDefault(u => (u.UserName == userName));
        }

        public bool RevokeToken(string userName)
        {
            //Recuperando um usuário e verificando se ele é null
            var user = _context.Users.SingleOrDefault(u => (u.UserName == userName));
            if(user is null)
            {
                return false;
            }
            //Recebe o user e anula ele, se ele nao for encontrado iniciando do 0 o processo
            user.RefreshToken = null;

            _context.SaveChanges();
            return true;
        }

        public User? RefreshUserInfo(User user)
        {
            //Se ele nao encontrar ninguem no banco banco com o mesmo Id que esta vindo como parametro...
            if (!_context.Users.Any(u => u.Id.Equals(user.Id))) return null;

            //Se ele encontrar alguem com o mesmo ID, ele armazena em result
            var result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));

            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        //Metodo responsavel por encriptar a senha 
        private string ComputeHash(string input, HashAlgorithm algotithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedBytes = algotithm.ComputeHash(inputBytes);

            var builder = new StringBuilder();
            
            foreach (var item in hashedBytes)
            {
                builder.Append(item.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
