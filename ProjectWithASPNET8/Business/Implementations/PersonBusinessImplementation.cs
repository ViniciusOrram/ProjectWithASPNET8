using ProjectWithASPNET8.Data.Converter.Implementations;
using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Hypermidia.Utils;
using ProjectWithASPNET8.Model;
using ProjectWithASPNET8.Repository;


namespace ProjectWithASPNET8.Business.Implementations
{
    public class PersonBusinessImplementation : IPersonBusiness
    {
        private readonly IPersonRepository _repository;
        private readonly PersonConverter _converter;

        public PersonBusinessImplementation(IPersonRepository repository)
        {
            _repository = repository;
            _converter = new PersonConverter();
        }

        public List<PersonVO> FindAll()
        {
            return _converter.Parse(_repository.FindAll());
        }

        public PagedSearchVO<PersonVO> FindWithPagedSearch(string name, string sortDirection, int pageSize, int page)
        {
           
            var sort =  (!string.IsNullOrEmpty(sortDirection) && !sortDirection.Equals("desc")) ? "asc" : "desc";
            var size = (pageSize < 1) ? 10 : pageSize;
            var offset = page > 0 ? (page - 1) * size : 0;

            //Implementando a geração dinamica da query
            //Condição para quando as querys não gerarem nada
            string query = @"select * from person p where 1 = 1";

            //Quando for diferente de null sera concatenado a query no WHERE
            if (!string.IsNullOrWhiteSpace(name)) query = query + $"and p.first_name like '%{name}%' ";
            query += $"order by p.first_name {sort} limit {size} offset {offset}";

            //A mesma condição de cima será aplicada para a CountQuery
            string countQuery = @"select count(*) from person p where 1 = 1";
            if (!string.IsNullOrWhiteSpace(name)) countQuery = countQuery + $"and p.first_name like '%{name}%'";

            var persons = _repository.FindWithPagedSearch(query);

            int totalResults = _repository.GetCount(countQuery);

            return new PagedSearchVO<PersonVO>
            {
                CurrentPage = page,
                List = _converter.Parse(persons),
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults
            };
        }

        public PersonVO FindById(long id)
        {
            return _converter.Parse(_repository.FindById(id));
        }

        public List<PersonVO> FindByName(string firstName, string lastName)
        {
            return _converter.Parse(_repository.FindByName(firstName, lastName));
        }

        public PersonVO Create(PersonVO person)
        {
            var personEntity = _converter.Parse(person);
            personEntity = _repository.Create(personEntity);

            return _converter.Parse(personEntity);
        }

        public PersonVO Update(PersonVO person)
        {
            var personEntity = _converter.Parse(person);
            personEntity = _repository.Update(personEntity);

            return _converter.Parse(personEntity);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public PersonVO Disable(long id)
        {
            var personEntity = _repository.Disable(id);

            return _converter.Parse(personEntity);
        }
    }
}
