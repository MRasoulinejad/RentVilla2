using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentVilla.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IVillaRepository Villa { get; }
        IVillaNumberRepository VillaNumber { get; }
        IApplicationUserRepository User { get; }
        IBookingRepository Booking { get; }
        IAmenityRepository Amenity { get; }
        void Save();
    }
    
}
