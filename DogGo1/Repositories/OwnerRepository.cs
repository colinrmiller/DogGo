using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DogGo1.Models;

namespace DogGo1.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Owner.Id as oId, Owner.Name as Name, Phone, Email, Address, n.Name as nName, n.Id as nId
                        FROM Owner
                        JOIN Neighborhood n ON n.Id = Owner.NeighborhoodId
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Owner> owners = new List<Owner>();
                        while (reader.Read())
                        {
                            Owner owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("oId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Neighborhood = new Neighborhood()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("nId")),
                                    Name = reader.GetString(reader.GetOrdinal("nName")),
                                }
                            };

                            owners.Add(owner);
                        }

                        return owners;
                    }
                }
            }
        }

        public Owner GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Owner.Id as oId, Owner.Name as Name, Phone, Email, Address, n.Name as nName, n.Id as nId, d.Name as dogName, d.Breed as dogBreed
                        FROM Owner
                        JOIN Neighborhood as n ON n.Id = Owner.NeighborhoodId
                        LEFT JOIN Dog as d ON d.OwnerId = Owner.Id
                        Where Owner.Id = @id;
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Owner owner = null;
                        while(reader.Read())
                        {
                            if (owner == null)
                                owner = new Owner
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("oId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    Neighborhood = new Neighborhood()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("nId")),
                                        Name = reader.GetString(reader.GetOrdinal("nName")),
                                    },
                                   Dogs = new List<Dog> { }
                            };

                            //if (reader.IsDBNull(reader.GetOrdinal("dogName")))
                            //{
                                owner.Dogs.Add(new Dog()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("dogName")),
                                    Breed = reader.GetString(reader.GetOrdinal("dogBreed"))
                                });
                            //}
                        }
                        return owner;
                       
                    }
                }
            }
        }
    }
}
