using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.web.ViewModels;

namespace WhiteLagoon.web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityService;
        private readonly IVillaService _villaService;
        public AmenityController(IAmenityService amenityService, IVillaService villaService)
        {
            _amenityService = amenityService;
            _villaService = villaService;
        }
        public IActionResult Index()
        {
            var amenities = _amenityService.GetAllAmenities();
            return View(amenities);
        }
        public IActionResult Create()
        {
            AmenityVM amenityVM = new AmenityVM
            {
                VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
            };

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {
            //ModelState.Remove("Villa");
            if (ModelState.IsValid)
            {
                _amenityService.CreateAmenity(obj.Amenity);
                TempData["Success"] = "The amenity has been created succesfully!";
                return RedirectToAction(nameof(Index));
            }
            
            obj.VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(obj);
        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM amenityVM = new AmenityVM
            {
                VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = _amenityService.GetAmenityById(amenityId)
            };

            if (amenityVM.Amenity is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM amenityVM)
        {
            if (ModelState.IsValid)
            {
                _amenityService.UpdateAmenity(amenityVM.Amenity);
                TempData["Success"] = "The amenity has been updated succesfully!";
                return RedirectToAction(nameof(Index));
            }

            amenityVM.VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(amenityVM);

        }
        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new AmenityVM
            {
                VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = _amenityService.GetAmenityById(amenityId)
            };

            if (amenityVM.Amenity is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            Amenity? objFromDB = _amenityService.GetAmenityById(amenityVM.Amenity.Id);
            if (objFromDB is not null)
            {
                _amenityService.DeleteAmenity(objFromDB.Id);
                TempData["Success"] = "The amenity has been deleted succesfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "The amenity could not be deleted!";
            return View();
        }
    }
}
