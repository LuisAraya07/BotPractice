using CedulaBot.Model;
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
    public class CedulaDialog : ComponentDialog
    {

        protected readonly IStatePropertyAccessor<CedulaState> _cedulaState;
        protected readonly BotState _conversationState;
        protected readonly IGometa _gometa;
        protected readonly string _numberIntValidator = "NumberIntValidator";

        public CedulaDialog(ConversationState conversationState, IGometa gometa): base(nameof(CedulaDialog))
        {
            _gometa = gometa;
            _conversationState = conversationState;
            _cedulaState = _conversationState.CreateProperty<CedulaState>("CedulaState");
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                NumeroStepAsync,
            }));
            AddDialog(new NumberPrompt<int>(nameof(_numberIntValidator), NumberValidatorAsync));
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

        }


        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Por favor, ingrese el número de cedula"),
                RetryPrompt = MessageFactory.Text("Por favor, ingrese un número de cedula válido"),
            };
            return await stepContext.PromptAsync(nameof(_numberIntValidator), promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> NumeroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ced = await _cedulaState.GetAsync(stepContext.Context, () => new CedulaState());
            ced.numero = int.Parse(stepContext.Result.ToString());
            await _cedulaState.SetAsync(stepContext.Context, ced, cancellationToken);

            var resultado = await _gometa.consultarCedula(ced.numero);
            var attachments = new List<Attachment>();

            var reply = MessageFactory.Text("No hay registro de esta cedula");
            if (resultado != null && resultado.resultcount > 0)
            {

                reply = (Activity)MessageFactory.Attachment(attachments);
                reply.Attachments.Add(Cards.CreateAdaptiveCardAttachment(resultado.results.FirstOrDefault()));
                // msg = $"Su nombre es {resultado.results.FirstOrDefault().fullname}";
            }
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.EndDialogAsync();
        }










        // Validamos el número si es una cedula
        private static Task<bool> NumberValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            try
            {
                int i;
                bool isInteger = Int32.TryParse(promptContext.Recognized.Value.ToString(), out i);
                bool isLarge = promptContext.Recognized.Value.ToString().Length <= 9;
                bool isInRange = promptContext.Recognized.Value >= 100000000 && promptContext.Recognized.Value <= 799999999;
                return Task.FromResult(promptContext.Recognized.Succeeded && isLarge && isInteger && isInRange);
            }
            catch (Exception)
            {

                return Task.FromResult(false);
            }
            
        }
    }
}

