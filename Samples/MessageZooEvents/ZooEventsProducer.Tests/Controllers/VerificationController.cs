using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PactNet.PactVerification;
using ZooEventsProducer.Models;

namespace ZooEventsProducer.Tests.Controllers
{
    [Route("")]
    public class VerificationController : Controller
    {
        private readonly MessageInvoker _messageInvoker;

        public VerificationController()
        {
            var animalCreator = new AnimalCreator();
            _messageInvoker = new MessageInvoker(new Dictionary<string, Action>
                {
                    {
                        "there is a Pet animal",
                        InsertPetIntoDatabase
                    },
                    {
                        "there is a non Pet animal",
                        InsertNonPetAnimalIntoDatabase
                    }
                },
                new Dictionary<string, Func<dynamic>>
                {
                    { "a pet animal created event", () => animalCreator.CreateAPet("Jane", PetType.Cat) },
                    {"a non pet animal created event", () => animalCreator.CreateANonPet("Fred", "Sloth") }
                }
            );
        }

        // POST: /
        [HttpPost]
        public ActionResult<dynamic> Post([FromBody] MessagePactDescription description)
        {
            var message = _messageInvoker.Invoke(description);
            return new { contents = message };
        }

        private void InsertNonPetAnimalIntoDatabase()
        {
            //ImplementProviderState
        }

        private void InsertPetIntoDatabase()
        {
            //ImplementProviderState
        }
    }
}
