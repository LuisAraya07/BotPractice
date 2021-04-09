using CedulaBot.Model;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.IO;
using AdaptiveCards.Templating;

namespace CedulaBot.Resources
{
    public class Cards
    {

        public static Attachment CreateAdaptiveCardAttachment(Result cedulaResponse)
        {
            // combine path for cross platform support
            var paths = new[] { ".", "Resources", "adaptiveCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardJson);
            var myData = new
            {
                result = cedulaResponse
            };
            string cardJson = template.Expand(myData);

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };
            return adaptiveCardAttachment;
        }

        public static Attachment CreatePokemonAttachment(PokemonResponse pokemon)
        {
            // combine path for cross platform support
            var paths = new[] { ".", "Resources", "pokemonCard.json" };
            var pokemonCardJson = File.ReadAllText(Path.Combine(paths));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(pokemonCardJson);
            pokemon.name = pokemon.name.ToUpper();
            pokemon.types[0].type.name = pokemon.types[0].type.name.ToUpper();
            var myData = new
            {
                pokemon = pokemon,
                secondType = pokemon.types.Count > 1 ? pokemon.types[1].type.name.ToUpper() : "",
                // esta feo esto :(
                hp = pokemon.stats[0].base_stat,
                attack = pokemon.stats[1].base_stat,
                defense = pokemon.stats[2].base_stat,
                spAttack = pokemon.stats[3].base_stat,
                spDefense = pokemon.stats[4].base_stat,
                speed = pokemon.stats[5].base_stat,

            };
            string cardJson = template.Expand(myData);
            

            var pokemonCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };
            return pokemonCardAttachment;
        }
    }
}
