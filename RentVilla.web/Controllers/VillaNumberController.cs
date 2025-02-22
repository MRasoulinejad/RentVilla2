using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RentVilla.Application.Common.Interfaces;
using RentVilla.Application.Services.Interface;
using RentVilla.Domain.Entities;
using RentVilla.Infrastructure.Data;
using RentVilla.web.ViewModels;

namespace RentVilla.web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
        }
        public IActionResult Index()
        {
            var villaNumbers = _villaNumberService.GetAllVillaNumbers();
            return View(villaNumbers);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
            };

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            bool roomNumberExists = _villaNumberService.CheckVillaNumberExists(obj.VillaNumber.Villa_Number);
            //ModelState.Remove("Villa");
            if (ModelState.IsValid && !roomNumberExists)
            {
                _villaNumberService.CreateVillaNumber(obj.VillaNumber);
                TempData["Success"] = "The villa Number has been created succesfully!";
                return RedirectToAction(nameof(Index));
            }
            if (roomNumberExists)
            {
                TempData["Error"] = "The villa number already exists!";
            }
            obj.VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(obj);
        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
            };

            if (villaNumberVM.VillaNumber is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            if (ModelState.IsValid)
            {
                _villaNumberService.UpdateVillaNumber(villaNumberVM.VillaNumber);
                TempData["Success"] = "The villa Number has been updated succesfully!";
                return RedirectToAction(nameof(Index));
            }

            villaNumberVM.VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(villaNumberVM);

        }
        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = _villaService.GetAllVillas().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
            };

            if (villaNumberVM.VillaNumber is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? objFromDB = _villaNumberService.GetVillaNumberById(villaNumberVM.VillaNumber.Villa_Number);
            if (objFromDB is not null)
            {
                _villaNumberService.DeleteVillaNumber(objFromDB.Villa_Number);
                TempData["Success"] = "The villa number has been deleted succesfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "The villa number could not be deleted!";
            return View();
        }
    }
}
