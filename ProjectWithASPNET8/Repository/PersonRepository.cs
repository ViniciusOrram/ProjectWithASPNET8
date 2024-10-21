using ProjectWithASPNET8.Model;
using ProjectWithASPNET8.Model.Context;
using ProjectWithASPNET8.Repository.Generic;

namespace ProjectWithASPNET8.Repository
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(MySqlContext context) : base(context)
        {

        }

        public Person Disable(long id)
        {
            if (!_context.Persons.Any(p => p.Id.Equals(id))) return null;

            var user =_context.Persons.SingleOrDefault(p => p.Id == id);
            if(user != null)
            {
                user.Enabled = false;
                try
                {
                    _context.Entry(user).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                } 
                catch (Exception)
                {
                    throw;
                }
            }
            return user;
        }
    }
}
