using ProjectWithASPNET8.Data.VO;
using System.Collections.Generic;

namespace ProjectWithASPNET8.Business
{
    public interface IPersonBusiness
    {
        PersonVO Create(PersonVO person);
        PersonVO FindById(long id);
        List<PersonVO> FindAll();
        PersonVO Update(PersonVO person);
        void Delete(long id);
    }
}
