// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.12.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using CedulaBot.Resources;

namespace CedulaBot.Bots
{
    public class EchoBot<T> : ActivityHandler
        where T : Dialog
    {
        private readonly Dialog _dialog;
        private readonly BotState _conversationState;
        public EchoBot(ConversationState conversationState, T dialog)
        {
            _dialog = dialog;
            _conversationState = conversationState;
        }

        // Welcome msg
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            
            await turnContext.SendActivityAsync("Welcome to Botluis");
            var reply = MessageFactory.Text("Please select an option...");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Cedula", Type = ActionTypes.ImBack, Value = "cedula", Image = "https://www.nacion.com/resizer/eOunl2o-n6oRu0LYQNM7ELq5_wU=/1200x0/center/middle/filters:quality(100)/arc-anglerfish-arc2-prod-gruponacion.s3.amazonaws.com/public/OMNUMMIW6NEPFHDL2237TQWJ4U.jpg", ImageAltText = "C" },
                    new CardAction() { Title = "Pokemon", Type = ActionTypes.ImBack, Value = "pokemon", Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/98/International_Pok%C3%A9mon_logo.svg/1200px-International_Pok%C3%A9mon_logo.svg.png", ImageAltText = "P" },
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        // Always here
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        // First msg
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Run the Dialog with the new message Activity.
            await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }


    }
}
