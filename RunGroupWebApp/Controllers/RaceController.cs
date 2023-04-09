using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interface;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAll();
            return View(races);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                if (result.Error == null)
                {
                    var race = new Race
                    {
                        Title = raceVM.Title,
                        Description = raceVM.Description,
                        Image = result.Url.ToString(),
                        Address = new Address()
                        {
                            City = raceVM.Address.City,
                            Street = raceVM.Address.Street,
                            State = raceVM.Address.State,
                        }
                    };
                    _raceRepository.Add(race);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Image upload failed");
                }
            }

            return View(new CreateRaceViewModel());
        }
    }
}
