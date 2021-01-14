using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BinaryFog.NameParser;
using LogicAppFunctions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace LogicAppFunctions
{
    public static class ParseFullNameHttp
    {
        [FunctionName(Names.ParseFullNameHttp)]
        public static HttpResponseMessage ParseFullName(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] Person person,
            ILogger log)
        {
            log.LogInformation($"Started {Names.ParseFullNameHttp} function.");

            HttpResponseMessage response = new HttpResponseMessage();

            if (person == null || string.IsNullOrWhiteSpace(person.FullName))
            {
                log.LogError("Person object as input is null or doesn't contain full name.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            string mailLeftHand = person.Mail?.Split('@').Length == 2 ? person.Mail.Split('@')[0] : null;

            FullNameParser parser = new FullNameParser(person.FullName);
            parser.Parse();
            if (!string.IsNullOrWhiteSpace(parser.FirstName) && !string.IsNullOrWhiteSpace(parser.LastName))
            {
                person.FirstName = parser.FirstName.IsAllCaps() ? parser.FirstName.ToTitleCase() : parser.FirstName;
                person.LastName = parser.LastName.IsAllCaps() ? parser.LastName.ToTitleCase() : parser.LastName;
            }
            else if (!string.IsNullOrWhiteSpace(person.Mail) && mailLeftHand?.Split('.', StringSplitOptions.RemoveEmptyEntries).Length == 2)
            {
                var splitMailLeftHand = mailLeftHand.Split('.');
                person.FirstName = splitMailLeftHand[0].ToTitleCase();
                person.LastName = splitMailLeftHand[1].ToTitleCase();
            }

            if (!string.IsNullOrWhiteSpace(person.FirstName) && !string.IsNullOrWhiteSpace(person.LastName))
            {
                response.Content = new StringContent(JsonSerializer.Serialize(person), Encoding.UTF8, "application/json");
                response.StatusCode = HttpStatusCode.OK;
                log.LogInformation($"Finished {Names.ParseFullNameHttp} function successfully.");
            }
            else
            {
                person.FirstName = person.FullName.IsAllCaps() ? person.FullName.ToTitleCase() : person.FullName;
                response.Content = new StringContent(JsonSerializer.Serialize(person), Encoding.UTF8, "application/json");
                response.StatusCode = HttpStatusCode.OK;
                log.LogWarning(
                    $"Finished {Names.ParseFullNameHttp} function successfully, but... Could not parse person for first name and last name. Entered full name to first name property");

            }

            return response;
        }
    }
}
