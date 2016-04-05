﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using System.Net.Mail;

using FireSharp;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using Neo4jClient;
using SendGrid;

using federacionHemofiliaWeb.Interfaces;
using federacionHemofiliaWeb.Models;
using federacionHemofiliaWeb.Services;

namespace federacionHemofiliaWeb.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private IFirebaseClient client;
        private GraphClient neoClient;
        private Web emailSender;

        public PacienteRepository(IOptions<FireOps> options)
        {
            client = new FirebaseClient(new FirebaseConfig
            {
                AuthSecret = options.Value.Secret,
                BasePath = options.Value.Url
            });

            neoClient = new GraphClient(
                new Uri(options.Value.NeoUrl),
                options.Value.NeoUser,
                options.Value.NeoPss);

            emailSender = new Web(options.Value.SendGrid);
        }

        public async Task<bool> create(Paciente paciente, string id)
        {
            var pacientes = new Dictionary<string, Paciente>();
            pacientes.Add(id, paciente);
            var response = await client.UpdateAsync($"users/", pacientes);

            var newPaciente = new Models.Neo4j.Paciente
            {
                Id = id
            };

            neoClient.Connect();
            neoClient.Cypher
                     .Create("(user:Paciente {newPaciente})")
                     .WithParam("newPaciente", newPaciente)
                     .ExecuteWithoutResults();


            if(response.StatusCode.ToString() == "OK")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Dictionary<string,Paciente>> get()
        {
            var response = await client.GetAsync("users/");
            var pacientes = response.ResultAs<Dictionary<string, Paciente>>();
            return pacientes;
        }

        public async Task<Paciente> get(string id)
        {
            var response = await client.GetAsync($"users/{id}");
            var user = response.ResultAs<Paciente>();
            return user;
        }

        public async Task<Dictionary<DateTime, int>> getData(string id)
        {
            var response = await client.GetAsync($"users/{id}/Aplicaciones/");
            var datos = response.ResultAs<Dictionary<DateTime, int>>();
            return datos;
        }

        public async Task<bool> update(Paciente paciente, string id)
        {
            var response = await client.UpdateAsync($"users/{id}", paciente);
            if(response.StatusCode.ToString() == "200")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async void sendEmail(string email, string password)
        {
            SendGridMessage newMessage = new SendGridMessage();
            newMessage.AddTo(email);
            newMessage.From = new MailAddress("hello@federacion.com", "Hello");
            newMessage.Subject = "Nueva cuenta";
            newMessage.Html = $@"
                                <html>
                                    <body>
                                            <p>
                                                password: {password}
                                            </p>
                                    </body>
                                </html>";

            await emailSender.DeliverAsync(newMessage);
        }
    }
}
