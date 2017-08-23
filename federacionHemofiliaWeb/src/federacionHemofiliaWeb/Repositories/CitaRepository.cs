using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

using Neo4jClient;
using SendGrid;
using Microsoft.Extensions.Options;

using federacionHemofiliaWeb.Interfaces;
using federacionHemofiliaWeb.Services;
using federacionHemofiliaWeb.Models.Neo4j;

namespace federacionHemofiliaWeb.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private GraphClient neoClient;
        private SendGridClient emailSender;

        public CitaRepository(IOptions<FireOps> options)
        {
            neoClient = new GraphClient(
                new Uri(options.Value.NeoUrl),
                options.Value.NeoUser,
                options.Value.NeoPss);

            emailSender = new SendGridClient(options.Value.SendGrid);
        }

        public Task<bool> Create(string IdDoctor, string IdPaciente, DateTime fecha)
        {
            var cita = new Cita
            {
                cita = fecha.Date.Day + "/" + fecha.Date.Month + "/" + fecha.Date.Year
            };
            neoClient.Connect();
            neoClient.Cypher
                .Match("(user1:Doctor)", "(user2:Paciente)")
                .Where((Doctor user1) => user1.Id == IdDoctor)
                .AndWhere((Models.Neo4j.Paciente user2) => user2.Id == IdPaciente)
                .CreateUnique("user1-[:Cita {fecha}]->user2")
                .WithParam("fecha", cita)
                .ExecuteWithoutResults();

            return Task.FromResult(true);
        }

        public Task<bool> Delete(string IdDoctor, string IdPaciente)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> Get(string IdDoctor, DateTime fecha)
        {
            var pacientesARecibir = new List<string>();
            var cita = new Cita
            {
                cita = fecha.Date.Day + "/" + fecha.Date.Month + "/" + fecha.Date.Year
            };
            neoClient.Connect();
            var pacientesPorDia = neoClient.Cypher
                                           .OptionalMatch("(doctor:Doctor)-[:Cita]-(paciente:Paciente)")
                                           .Where((Doctor doctor) => doctor.Id == IdDoctor)
                                           .Return((paciente) => new
                                           {
                                               Paciente = paciente.CollectAs<Models.Neo4j.Paciente>()
                                           })
                                           .Results;

            foreach (var paciente in pacientesPorDia)
            {
                var pac = paciente.Paciente;
                var citas = pac.ToList();
                foreach(var pacien in citas)
                {
                    pacientesARecibir.Add(pacien.Id);
                }
            }

            return Task.FromResult(pacientesARecibir);
        }
    }

    class Cita
    {
        public string cita { get; set; }
    }
}
