
using DogGo1.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using DogGo1.Repositories;

namespace DogGo1.Repositories
{
    public interface IWalkerRepository
    {
        List<Walker> GetAllWalkers();
        Walker GetWalkerById(int id);
    }
}