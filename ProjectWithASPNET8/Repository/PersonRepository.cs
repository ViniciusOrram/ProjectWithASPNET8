﻿using ProjectWithASPNET8.Model;
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

        public List<Person> FindByName(string firstName, string lastName)
        {
            //Os dois valores estão setados
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                return _context.Persons.Where(
                p => p.FirstName.Contains(firstName)
                && p.LastName.Contains(lastName)).ToList();
            }
            
            // FirstName não esta setado 
            else if (string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                return _context.Persons.Where(
                p => p.LastName.Contains(lastName)).ToList();
            }

            //O LastName não esta setado 
            else if (!string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
            {
                return _context.Persons.Where(
                p => p.FirstName.Contains(firstName)).ToList();
            }

            return null;
        }
    }
}
