using CedulaBot.Model;
using CedulaBot.Resources;
using CedulaBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CedulaBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {

        public MainDialog(CedulaDialog cedulaDialog, PokemonDialog pokemonDialog) : base(nameof(MainDialog))
        {
            AddDialog(cedulaDialog);
            AddDialog(pokemonDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                MainStepAsync,
                LastStepAsync
            }));
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        

        private async Task<DialogTurnResult> MainStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //string msg = "Hola, gracias por contactarme, puedo ayudarte a consultar el numero de cedula";
            var value = stepContext.Context.Activity.Text;
            if (value != null)
            {
                if(value == "cedula")
                {
                    return await stepContext.BeginDialogAsync(nameof(CedulaDialog));
                }
                if (value == "pokemon")
                    return await stepContext.BeginDialogAsync(nameof(PokemonDialog));
            }
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> LastStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Welcome to Botluis");
            var reply = MessageFactory.Text("Please select an option...");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Cedula", Type = ActionTypes.ImBack, Value = "cedula", Image = "https://www.nacion.com/resizer/eOunl2o-n6oRu0LYQNM7ELq5_wU=/1200x0/center/middle/filters:quality(100)/arc-anglerfish-arc2-prod-gruponacion.s3.amazonaws.com/public/OMNUMMIW6NEPFHDL2237TQWJ4U.jpg", ImageAltText = "C" },
                    new CardAction() { Title = "Pokemon", Type = ActionTypes.ImBack, Value = "pokemon", Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/98/International_Pok%C3%A9mon_logo.svg/1200px-International_Pok%C3%A9mon_logo.svg.png", ImageAltText = "P" },
                },
            };
            await stepContext.Context.SendActivityAsync(reply);
            return await stepContext.NextAsync(reply);
        }


    }
}
