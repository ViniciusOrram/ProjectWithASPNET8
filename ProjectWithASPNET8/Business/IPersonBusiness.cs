using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Hypermidia.Utils;
using ProjectWithASPNET8.Model;
using System.Collections.Generic;
using System.Globalization;

namespace ProjectWithASPNET8.Business
{
    public interface IPersonBusiness
    {
        PersonVO Create(PersonVO person);
        PersonVO FindById(long id);
        List<PersonVO> FindByName(string firstName, string lastName);
        List<PersonVO> FindAll();
        PagedSearchVO<PersonVO> FindWithPagedSearch(string name, string sortDirection, int pageSize, int page);
        PersonVO Update(PersonVO person);
        PersonVO Disable(long id);
        void Delete(long id);
    }
}
