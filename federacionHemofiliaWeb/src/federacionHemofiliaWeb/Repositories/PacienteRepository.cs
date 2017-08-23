using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Neo4jClient;
using SendGrid;

using federacionHemofiliaWeb.Interfaces;
using federacionHemofiliaWeb.Models;
using federacionHemofiliaWeb.Services;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

namespace federacionHemofiliaWeb.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private GraphClient neoClient;
        private SendGridClient emailSender;

        public PacienteRepository(IOptions<FireOps> options)
        {
            neoClient = new GraphClient(
                new Uri(options.Value.NeoUrl),
                options.Value.NeoUser,
                options.Value.NeoPss);

            emailSender = new SendGridClient(options.Value.SendGrid);
        }

        public async Task<bool> create(Paciente paciente, string id)
        {
            throw new NotImplementedException();
            /*
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
            */
        }

        public async Task<Dictionary<string,Paciente>> get()
        {
            throw new NotImplementedException();
            /*
            var response = await client.GetAsync("users/");
            var pacientes = response.ResultAs<Dictionary<string, Paciente>>();
            return pacientes;
            */
        }

        public async Task<Paciente> get(string id)
        {
            throw new NotImplementedException();
            /*
            var response = await client.GetAsync($"users/{id}");
            var user = response.ResultAs<Paciente>();
            return user;
            */
        }

        public async Task<Dictionary<DateTime, int>> getData(string id)
        {
            throw new NotImplementedException();
            /*
            var response = await client.GetAsync($"users/{id}/Aplicaciones/");
            var datos = response.ResultAs<Dictionary<DateTime, int>>();
            return datos;
            */
        }

        public async Task<bool> update(Paciente paciente, string id)
        {
            throw new NotImplementedException();
            /*
            var users = await get();
            users[id] = paciente;
            
            try { 
                var response = await client.UpdateAsync($"users/", users);
                if (response.StatusCode.ToString() == "200")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
            */
        }

        public async void sendEmail(string email, string password)
        {
            var newMessage = new SendGridMessage();
            newMessage.AddTo(email);
            newMessage.From = new EmailAddress("hello@federacion.com", "Hello");
            newMessage.Subject = "Nueva cuenta";
            newMessage.HtmlContent = $@"
                                <html>
                                    <body>
                                            <p>
                                                password: {password}
                                            </p>
                                    </body>
                                </html>";

            await emailSender.SendEmailAsync(newMessage);
        }
    }
}
