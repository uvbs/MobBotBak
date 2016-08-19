﻿#region using directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoGo.PokeMobBot.Logic.Common;
using PoGo.PokeMobBot.Logic.Event;
using PoGo.PokeMobBot.Logic.Logging;
using PoGo.PokeMobBot.Logic.State;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;

#endregion

namespace PoGo.PokeMobBot.Logic.Tasks
{
    public static class CatchLurePokemonsTask
    {
        public static async Task Execute(ISession session, FortData currentFortData, CancellationToken cancellationToken)
        {

            
            cancellationToken.ThrowIfCancellationRequested();

            // Refresh inventory so that the player stats are fresh
            await session.Inventory.RefreshCachedInventory();

            session.EventDispatcher.Send(new DebugEvent()
            {
                Message = session.Translation.GetTranslation(TranslationString.LookingForLurePokemon)
            });

            var fortId = currentFortData.Id;

            var pokemonId = currentFortData.LureInfo.ActivePokemonId;

            if (session.LogicSettings.UsePokemonToNotCatchFilter &&
                session.LogicSettings.PokemonsNotToCatch.Contains(pokemonId))
            {
                session.EventDispatcher.Send(new NoticeEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.PokemonSkipped, session.Translation.GetPokemonName(pokemonId))
                });
            }
            else
            {
                var encounterId = currentFortData.LureInfo.EncounterId;
                var encounter = await session.Client.Encounter.EncounterLurePokemon(encounterId, fortId);

                if (encounter.Result == DiskEncounterResponse.Types.Result.Success)
                {
                    //var pokemons = await session.MapCache.MapPokemons(session);
                    //var pokemon = pokemons.FirstOrDefault(i => i.PokemonId == encounter.PokemonData.PokemonId);
                    session.EventDispatcher.Send(new DebugEvent()
                    {
                        Message = "Found a Lure Pokemon."
                    });
                    
                    MapPokemon _pokemon = new MapPokemon
                    {
                        EncounterId = currentFortData.LureInfo.EncounterId,
                        ExpirationTimestampMs = currentFortData.LureInfo.LureExpiresTimestampMs,
                        Latitude = currentFortData.Latitude,
                        Longitude = currentFortData.Longitude,
                        PokemonId = currentFortData.LureInfo.ActivePokemonId,
                        SpawnPointId = currentFortData.LureInfo.FortId
                    };
                    PokemonCacheItem pokemon = new PokemonCacheItem(_pokemon);

                    await CatchPokemonTask.Execute(session, encounter, pokemon, currentFortData, encounterId);
                    currentFortData.LureInfo = null;
                    //await CatchPokemonTask.Execute(session, encounter, pokemon, currentFortData, encounterId);
                }
                else if (encounter.Result == DiskEncounterResponse.Types.Result.PokemonInventoryFull)
                {
                    if (session.LogicSettings.TransferDuplicatePokemon)
                    {
                        session.EventDispatcher.Send(new WarnEvent
                        {
                            Message = session.Translation.GetTranslation(TranslationString.InvFullTransferring)
                        });
                        await TransferDuplicatePokemonTask.Execute(session, cancellationToken);
                    }
                    else
                        session.EventDispatcher.Send(new WarnEvent
                        {
                            Message = session.Translation.GetTranslation(TranslationString.InvFullTransferManually)
                        });
                }
                else
                {
                    if (encounter.Result.ToString().Contains("NotAvailable")) return;
                    session.EventDispatcher.Send(new WarnEvent
                    {
                        Message =
                            session.Translation.GetTranslation(TranslationString.EncounterProblemLurePokemon,
                                encounter.Result)
                    });
                }

                // always wait the delay amount between catches, ideally to prevent you from making another call too early after a catch event
                await Task.Delay(session.LogicSettings.DelayBetweenPokemonCatch);
            }
        }
    }
}