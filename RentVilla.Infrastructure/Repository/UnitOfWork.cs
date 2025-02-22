using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentVilla.Application.Common.Interfaces;
using RentVilla.Infrastructure.Data;

namespace RentVilla.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _db;
        public IVillaRepository Villa {  get; private set; }
        public IBookingRepository Booking { get; private set; }
        public IApplicationUserRepository User { get; private set; }
        public IVillaNumberRepository VillaNumber { get; private set; }
        public IAmenityRepository Amenity { get; private set; }
        public UnitOfWork(ApplicationDBContext db)
        {
            _db = db;
            Villa = new VillaRepository(db);
            Booking = new BookingRepository(db);
            Amenity = new AmenityRepository(db);
            VillaNumber = new VillaNumberRepository(db);
            User = new ApplicationUserRepository(db);
        }

        public void Save()
        {

            _db.SaveChanges();
        }
    }
}
