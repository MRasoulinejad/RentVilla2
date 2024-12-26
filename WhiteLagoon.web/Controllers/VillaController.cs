using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        public VillaController(IVillaService villaService)
        {
            _villaService = villaService;
        }
        public IActionResult Index()
        {
            var villas = _villaService.GetAllVillas();
            return View(villas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if (obj.Name == obj.Description)
            {
                ModelState.AddModelError("name", "The description can not exactly match the name");
            }
            if (ModelState.IsValid)
            {
                _villaService.CreateVilla(obj);
                TempData["Success"] = "The villa has been created succesfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Update(int villaId)
        {
            Villa? obj = _villaService.GetVillaById(villaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Update(Villa obj)
        {

            if (ModelState.IsValid && obj.Id > 0)
            {
               _villaService.UpdateVilla(obj);
                TempData["Success"] = "The villa has been updated succesfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int villaId)
        {
            Villa? obj = _villaService.GetVillaById(villaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            bool deleted = _villaService.DeleteVilla(obj.Id);
            if (deleted)
            {
                TempData["Success"] = "The villa has been deleted succesfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "The villa could not be deleted!";
            }
            return View();
        }
    }
}
