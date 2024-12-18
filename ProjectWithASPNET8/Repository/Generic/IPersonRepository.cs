﻿using ProjectWithASPNET8.Model;

namespace ProjectWithASPNET8.Repository.Generic
{
    public interface IRepository
    {
        Person Create(Person person);
        Person FindById(long id);
        List<Person> FindAll();
        Person Update(Person person);
        void Delete(long id);
        bool Exists(long id);
    }
}
