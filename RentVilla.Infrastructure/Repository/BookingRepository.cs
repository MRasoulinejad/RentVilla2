using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RentVilla.Application.Common.Interfaces;
using RentVilla.Application.Common.Utility;
using RentVilla.Domain.Entities;
using RentVilla.Infrastructure.Data;

namespace RentVilla.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDBContext _db;
        public BookingRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Booking entity)
        {
            _db.Bookings.Update(entity);
        }
    }
}
