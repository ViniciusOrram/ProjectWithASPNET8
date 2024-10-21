using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Model;

namespace ProjectWithASPNET8.Repository
{
    public interface IPersonRepository : IRepository<Person>
    {
        Person Disable(long id);
    }
}
