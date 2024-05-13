using ProjectWithASPNET8.Model;
using System.Collections.Generic;

namespace ProjectWithASPNET8.Services
{
    public interface IPersonService
    {
        Person Create(Person person);
        Person FindById(long id);
        List<Person> FindAll();
        Person Update(Person person);
        void Delete(long id);
    }
}
