using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsCRUDAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightsCRUDAPI.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Flight> Flights { get; set; }
    }
}