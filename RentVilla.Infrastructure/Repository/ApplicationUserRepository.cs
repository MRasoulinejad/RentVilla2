using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RentVilla.Application.Common.Interfaces;
using RentVilla.Domain.Entities;
using RentVilla.Infrastructure.Data;

namespace RentVilla.Infrastructure.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser> , IApplicationUserRepository
    {
        private readonly ApplicationDBContext _db;
        public ApplicationUserRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }


    }
}
