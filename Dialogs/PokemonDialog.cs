using CedulaBot.Resources;
using CedulaBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Activity = Microsoft.Bot.Schema.Activity;


namespace CedulaBot.Dialogs
{
    public class PokemonDialog : ComponentDialog
    {
        
        protected readonly IPokemon _pokemon;
        protected readonly string _pokemonValidator = "PokemonValidatorAsync";

        public PokemonDialog(ConversationState conversationState, IPokemon pokemon) : base(nameof(PokemonDialog))
        {
            _pokemon = pokemon;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                PokemonStepAsync,
            }));
            AddDialog(new TextPrompt(nameof(_pokemonValidator), PokemonValidatorAsync));
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }

        

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Por favor, ingrese el nombre del pokemon"),
                RetryPrompt = MessageFactory.Text("Por favor, ingrese un nombre de pokemon válido"),
            };
            return await stepContext.PromptAsync(nameof(_pokemonValidator), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> PokemonStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var pokemon = stepContext.Context.Activity.Text.ToLower();
            var resultado = await _pokemon.consultarPokemon(pokemon);
            var attachments = new List<Attachment>();
            var reply = MessageFactory.Text("No hay registro de este pokemon");
            var r = resultado.types.Count;
            if (resultado != null)
            {

                reply = (Activity)MessageFactory.Attachment(attachments);
                reply.Attachments.Add(Cards.CreatePokemonAttachment(resultado));
            }
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.EndDialogAsync();
        }

        private Task<bool> PokemonValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            try
            {
                List<string> invalidChars = new List<string>() { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                var text = promptContext.Context.Activity.Text;
                foreach (string s in invalidChars)
                {
                    if (text.Contains(s))
                    {
                        return Task.FromResult(false);
                    }
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

    }
}
