using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using federacionHemofiliaWeb.Interfaces;
using federacionHemofiliaWeb.Models;
using federacionHemofiliaWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace federacionHemofiliaWeb.Controllers
{
    public class DoctorController : Controller
    {
        public IPacienteRepository PacienteRepository { get; set; }
        
        public ICitaRepository CitasRepository { get; set; }
        
        public IDoctorRepository DoctorRepository { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(
            UserManager<ApplicationUser> userManager,
            IPacienteRepository pacienteRepository,
            ICitaRepository citaRepository,
            IDoctorRepository doctorRepository)
        {
            PacienteRepository = pacienteRepository;
            CitasRepository = citaRepository;
            DoctorRepository = doctorRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Paciente()
        {
            if (User.Identity.IsAuthenticated)
            { 
                var id = await _userManager.FindByEmailAsync(User.Identity.Name);
                var date = DateTime.Now;
                var listaDePacientes = await CitasRepository.Get(id.Id, date);
                var pacient = new Dictionary<string, Paciente>();
                foreach (var paciente in listaDePacientes)
                {
                    pacient.Add(paciente, await PacienteRepository.get(paciente));
                }

                return View(new PacienteMV
                {
                    pacientes = pacient
                });
            }
            else
            {
                return RedirectToAction("Index", "Home", "redirected=True");
            }
        }
        
        [HttpGet]
        public async Task<Dictionary<DateTime, int>> pacienteGraph(string id)
        {
            return await PacienteRepository.getData(id);
        }
        
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Cita()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cita(CitaVM cita)
        {
            var paciente = await _userManager.FindByEmailAsync(cita.Email);
            var doctor = await _userManager.FindByNameAsync(User.Identity.Name);
            
            if(await CitasRepository.Create(doctor.Id, paciente.Id, cita.Fecha))
            {
                return RedirectToAction("Registro");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Invitar(Invitacion invitacion)
        {
            if (ModelState.IsValid)
            {
                var doctor = await _userManager.FindByEmailAsync(User.Identity.Name);
                var doctorName = await DoctorRepository.GetDoctor(doctor.Id);
                var doctorFullName = doctorName.FirstName + " " + doctorName.LastNames; 
                DoctorRepository.SendMail(doctorFullName, invitacion.Correo);
            }

            return RedirectToAction("InvitacionEnviada", "Doctor", invitacion.Correo);
        }

        [HttpGet]
        public IActionResult Invitar()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Confirmacion(string Id)
        {
            var user = await _userManager.FindByEmailAsync(Id);
            var name = await PacienteRepository.get(user.Id);

            var fullName = new PacienteUserName
            {
                FullUserName = name.Nombre + " " + name.Apellido
            };

            return View(fullName);
        }

        [HttpGet]
        public IActionResult CitaEnviada()
        {
            return View();
        }

        [HttpGet]
        public IActionResult InvitacionEnviada(string Id)
        {
            return View(Id);
        }
        
        [HttpGet]
        public IActionResult Inicio()
        {
            return View();
        }
    }
}
