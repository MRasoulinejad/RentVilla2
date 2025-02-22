using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentVilla.Application.Common.Interfaces;
using RentVilla.Application.Services.Interface;
using RentVilla.Domain.Entities;

namespace RentVilla.Application.Services.Implementation
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public VillaNumberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckVillaNumberExists(int villa_Number)
        {
            return _unitOfWork.VillaNumber.Any(x => x.Villa_Number == villa_Number);
        }

        public void CreateVillaNumber(VillaNumber villaNumber)
        {
            _unitOfWork.VillaNumber.Add(villaNumber);
            _unitOfWork.Save();
        }

        public bool DeleteVillaNumber(int id)
        {
            try
            {
                VillaNumber? objFromDB = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == id);
                if (objFromDB is not null)
                {
                    _unitOfWork.VillaNumber.Remove(objFromDB);
                    _unitOfWork.Save();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<VillaNumber> GetAllVillaNumbers()
        {
            return _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
        }

        public VillaNumber GetVillaNumberById(int id)
        {
            return _unitOfWork.VillaNumber.Get(u => u.Villa_Number == id, includeProperties: "Villa");
        }

        public IEnumerable<VillaNumber> GetVillaNumbersAvailabilityByDate(int nights, DateOnly checkInDate)
        {
            throw new NotImplementedException();
        }

        public bool IsVillaNumberAvailableByDate(int villaNumberId, int nights, DateOnly checkInDate)
        {
            throw new NotImplementedException();
        }

        public void UpdateVillaNumber(VillaNumber villaNumber)
        {
            _unitOfWork.VillaNumber.Update(villaNumber);
            _unitOfWork.Save();
        }
    }
}
