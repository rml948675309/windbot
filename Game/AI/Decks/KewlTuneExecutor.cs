using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    /// <summary>
    /// Kewl Tune Executor - OCG 2026.01 Meta
    /// Tournament-level AI for competitive Kewl Tune play
    /// </summary>
    [Deck("KewlTune", "AI_KewlTune")]
    public class KewlTuneExecutor : DefaultExecutor
    {
        // Card IDs
        private const int KewlTuneCue = 16387555;
        private const int KewlTuneRotary = 17209452;
        private const int KewlTuneReco = 89392810;
        private const int KewlTuneMix = 16509007;
        private const int KewlTuneClip = 43904702;
        private const int JJKewlTune = 14442329;
        private const int KewlTuneSynchro = 78058681;
        
        // Extra Deck
        private const int KewlTuneTrackMaker = 42781164;
        private const int KewlTuneRemix = 88170262;
        private const int KewlTuneRS = 15665977;
        private const int KewlTuneCrackle = 39576656;
        private const int KewlTuneLoudnessWar = 41069676;
        private const int KewlTuneBack2Back = 65961304;
        
        // Hand Traps
        private const int FidraulisHarmonia = 70088809;
        private const int Fuwalos = 42141493;
        private const int AshBlossom = 14558128;
        private const int GhostBelle = 73642296;
        private const int GhostOgre = 59438931;
        private const int EffectVeiler = 97268403;
        private const int MaxxC = 23434538;
        private const int Nibiru = 27204311;
        
        // Generic
        private const int SynchroOvertake = 99243014;
        private const int CalledByTheGrave = 24224830;
        
        // Generic Extra Deck
        private const int VisasAmritara = 821049;
        private const int Zalen = 4891376;
        private const int Packbit = 72444406;
        private const int Malong = 93125329;

        private bool CueUsedThisTurn = false;
        private bool RotaryUsedThisTurn = false;
        private bool JJActivatedThisTurn = false;
        private bool ComboStarted = false;

        public KewlTuneExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {
            // Normal Summon priorities
            AddExecutor(ExecutorType.Summon, KewlTuneCue, SummonCue);
            AddExecutor(ExecutorType.Summon, KewlTuneRotary, SummonRotary);
            AddExecutor(ExecutorType.Summon, KewlTuneMix, SummonMix);
            
            // Activate Field Spell
            AddExecutor(ExecutorType.Activate, JJKewlTune, ActivateJJ);
            
            // Monster effects
            AddExecutor(ExecutorType.Activate, KewlTuneCue, ActivateCue);
            AddExecutor(ExecutorType.Activate, KewlTuneRotary, ActivateRotary);
            AddExecutor(ExecutorType.Activate, KewlTuneReco, ActivateReco);
            AddExecutor(ExecutorType.Activate, KewlTuneMix, ActivateMix);
            AddExecutor(ExecutorType.Activate, KewlTuneClip, ActivateClip);
            
            // Spell activations
            AddExecutor(ExecutorType.Activate, KewlTuneSynchro, ActivateKewlTuneSynchro);
            AddExecutor(ExecutorType.Activate, SynchroOvertake, ActivateSynchroOvertake);
            AddExecutor(ExecutorType.Activate, CalledByTheGrave, DefaultCalledByTheGrave);
            
            // Hand Traps (opponent's turn)
            AddExecutor(ExecutorType.Activate, MaxxC, ActivateMaxxC);
            AddExecutor(ExecutorType.Activate, AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, GhostBelle, ActivateGhostBelle);
            AddExecutor(ExecutorType.Activate, EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, FidraulisHarmonia, ActivateFidraulis);
            AddExecutor(ExecutorType.Activate, Nibiru, DefaultNibiruThePrimalBeing);
            
            // Synchro Summons
            AddExecutor(ExecutorType.SpSummon, KewlTuneTrackMaker, SynchroSummonTrackMaker);
            AddExecutor(ExecutorType.SpSummon, KewlTuneRemix, SynchroSummonRemix);
            AddExecutor(ExecutorType.SpSummon, KewlTuneRS, SynchroSummonRS);
            AddExecutor(ExecutorType.SpSummon, KewlTuneCrackle, SynchroSummonCrackle);
            AddExecutor(ExecutorType.SpSummon, KewlTuneBack2Back, SynchroSummonBack2Back);
            
            // Extra Deck monsters effects
            AddExecutor(ExecutorType.Activate, KewlTuneTrackMaker, ActivateTrackMaker);
            AddExecutor(ExecutorType.Activate, KewlTuneRemix, ActivateRemix);
            AddExecutor(ExecutorType.Activate, KewlTuneRS, ActivateRS);
            AddExecutor(ExecutorType.Activate, KewlTuneCrackle, ActivateCrackle);
            AddExecutor(ExecutorType.Activate, KewlTuneBack2Back, ActivateBack2Back);
            AddExecutor(ExecutorType.Activate, Zalen, ActivateZalen);
        }

        public override void OnNewTurn()
        {
            CueUsedThisTurn = false;
            RotaryUsedThisTurn = false;
            JJActivatedThisTurn = false;
            ComboStarted = false;
        }

        // ========== SUMMON LOGIC ==========

        private bool SummonCue()
        {
            if (CueUsedThisTurn) return false;
            
            // Always summon Cue if we have it - best starter
            CueUsedThisTurn = true;
            ComboStarted = true;
            return true;
        }

        private bool SummonRotary()
        {
            if (RotaryUsedThisTurn) return false;
            
            // Summon Rotary if we don't have Cue
            if (!Bot.HasInHand(KewlTuneCue) && Bot.GetMonsterCount() == 0)
            {
                RotaryUsedThisTurn = true;
                return true;
            }
            return false;
        }

        private bool SummonMix()
        {
            // Summon Mix during combo extension
            if (ComboStarted && Bot.HasInHand(KewlTuneReco))
            {
                return true;
            }
            return false;
        }

        // ========== ACTIVATION LOGIC ==========

        private bool ActivateJJ()
        {
            if (JJActivatedThisTurn) return false;
            
            // Activate JJ "Kewl Tune" field spell
            if (!Bot.HasInSpellZone(JJKewlTune))
            {
                JJActivatedThisTurn = true;
                return true;
            }
            return false;
        }

        private bool ActivateCue()
        {
            // Cue effect: Special Summon Rotary from deck
            if (Card.Location == CardLocation.MonsterZone)
            {
                return true;
            }
            return false;
        }

        private bool ActivateRotary()
        {
            // Rotary effect: Look at opponent's hand, add JJ Kewl Tune
            if (Card.Location == CardLocation.Grave)
            {
                // Effect activated from GY after being used as Synchro material
                return true;
            }
            return false;
        }

        private bool ActivateReco()
        {
            // Reco effect: Add Kewl Tune card from deck or destroy S/T
            if (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave)
            {
                return true;
            }
            return false;
        }

        private bool ActivateMix()
        {
            // Mix effect: Add Reco or destroy monster
            if (Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave)
            {
                return true;
            }
            return false;
        }

        private bool ActivateClip()
        {
            // Clip effect: Banish from opponent's Extra Deck
            return true;
        }

        private bool ActivateKewlTuneSynchro()
        {
            // Activate Kewl Tune Synchro quick-play
            if (Duel.Phase >= DuelPhase.BattleStart || Duel.Player == 1)
            {
                return true;
            }
            return false;
        }

        private bool ActivateSynchroOvertake()
        {
            // Use Synchro Overtake to recover from disruption
            return Bot.HasInGraveyard(KewlTuneRotary) || Bot.HasInGraveyard(KewlTuneCue);
        }

        // ========== HAND TRAPS ==========

        private bool ActivateMaxxC()
        {
            // Only use Maxx C against combo decks (not Rituals)
            if (Duel.Player == 1 && Duel.Turn > 1)
            {
                // Activate on opponent's first Special Summon
                return Util.IsChainTarget(Card);
            }
            return false;
        }

        private bool ActivateGhostBelle()
        {
            // Ghost Belle on GY effects
            if (Duel.Player == 1)
            {
                return Util.IsChainTarget(Card);
            }
            return false;
        }

        private bool ActivateFidraulis()
        {
            // Fidraulis Harmonia - early disruption
            if (Duel.Player == 1 && Enemy.GetMonsterCount() <= 2)
            {
                // Only use early in opponent's combo
                return Bot.ExtraDeck.Count >= 5; // Need 5 Synchros to reveal
            }
            return false;
        }

        // ========== SYNCHRO SUMMONS ==========

        private bool SynchroSummonTrackMaker()
        {
            // Cue + Rotary = Track Maker
            if (Bot.HasInMonstersZone(KewlTuneCue) && Bot.HasInMonstersZone(KewlTuneRotary))
            {
                AI.SelectMaterials(new List<int> { KewlTuneCue, KewlTuneRotary });
                return true;
            }
            return false;
        }

        private bool SynchroSummonRemix()
        {
            // Mix + Reco = Remix
            if (Bot.HasInMonstersZone(KewlTuneMix))
            {
                return true;
            }
            return false;
        }

        private bool SynchroSummonRS()
        {
            // Reco + Mix = RS
            return true;
        }

        private bool SynchroSummonCrackle()
        {
            // Clip + Reco = Crackle
            return true;
        }

        private bool SynchroSummonBack2Back()
        {
            // RS + Crackle = Back 2 Back
            if (Bot.HasInMonstersZone(KewlTuneRS) && Bot.HasInMonstersZone(KewlTuneCrackle))
            {
                return true;
            }
            return false;
        }

        // ========== EXTRA DECK EFFECTS ==========

        private bool ActivateTrackMaker()
        {
            // Track Maker: Add Kewl Tune Synchro
            if (Card.Location == CardLocation.MonsterZone)
            {
                // Priority: Add Kewl Tune Synchro quick-play
                AI.SelectCard(KewlTuneSynchro);
                return true;
            }
            return false;
        }

        private bool ActivateRemix()
        {
            // Remix effect: Tribute to add Mix, SS Reco from GY
            if (Duel.Player == 1 && Card.Location == CardLocation.MonsterZone)
            {
                // Tribute Remix during opponent's turn
                AI.SelectCard(KewlTuneMix); // Add Mix from GY
                AI.SelectNextCard(KewlTuneReco); // SS Reco from GY
                return true;
            }
            return false;
        }

        private bool ActivateRS()
        {
            // RS: Negate face-up card
            if (Card.Location == CardLocation.MonsterZone && Duel.Player == 1)
            {
                // Negate opponent's key cards
                if (Util.GetLastChainCard() != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ActivateCrackle()
        {
            // Crackle: Banish from opponent's Extra Deck + SS itself from GY
            if (Card.Location == CardLocation.MonsterZone)
            {
                // Effect 1: Banish 2 cards from opponent's Extra Deck
                return true;
            }
            else if (Card.Location == CardLocation.Grave)
            {
                // Effect 2: Special Summon itself from GY
                return Bot.GetMonsterCount() < 5;
            }
            return false;
        }

        private bool ActivateBack2Back()
        {
            // Back 2 Back: Ultimate combo finisher
            if (Duel.Player == 1 && Card.Location == CardLocation.MonsterZone)
            {
                // When opponent activates effect: SS Mix from GY
                AI.SelectCard(KewlTuneMix);
                return true;
            }
            return false;
        }

        private bool ActivateZalen()
        {
            // Zalen the Shackled Dragon: Negate and destroy
            if (Duel.Player == 1 && Util.IsChainTarget(Card))
            {
                // Chain Zalen to negate and destroy opponent's card
                ClientCard target = Util.GetLastChainCard();
                if (target != null)
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }

        // ========== COMBO STATE TRACKING ==========

        public override bool OnSelectHand()
        {
            // Going first vs going second decision
            // Kewl Tune is a going-first deck
            return true;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            // Always summon in Attack Position for combos
            YGOSharp.OCGWrapper.NamedCard card = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (card != null && card.HasType(CardType.Monster))
            {
                return CardPosition.FaceUpAttack;
            }
            return base.OnSelectPosition(cardId, positions);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            // Smart card selection for effects
            
            // For Synchro materials: Optimize material choice
            if (hint == HintMsg.SynchroMaterial)
            {
                return SelectOptimalSynchroMaterials(cards, min, max);
            }
            
            // For adding cards: Priority order
            if (hint == HintMsg.AddToHand)
            {
                return SelectOptimalAdds(cards, max);
            }
            
            // For destroying: Target opponent's threats
            if (hint == HintMsg.Destroy)
            {
                return SelectOptimalDestroyTargets(cards, max);
            }
            
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        private IList<ClientCard> SelectOptimalSynchroMaterials(IList<ClientCard> cards, int min, int max)
        {
            // Intelligent Synchro material selection
            List<ClientCard> result = new List<ClientCard>();
            
            // Prioritize using cards that have already activated effects
            foreach (ClientCard card in cards)
            {
                if (card.Id == KewlTuneCue && CueUsedThisTurn)
                {
                    result.Add(card);
                    if (result.Count >= max) break;
                }
            }
            
            // Then add remaining materials needed
            foreach (ClientCard card in cards)
            {
                if (!result.Contains(card))
                {
                    result.Add(card);
                    if (result.Count >= max) break;
                }
            }
            
            return result;
        }

        private IList<ClientCard> SelectOptimalAdds(IList<ClientCard> cards, int max)
        {
            // Priority order for adding cards to hand
            List<ClientCard> result = new List<ClientCard>();
            
            int[] priority = {
                KewlTuneSynchro,  // Highest priority: Quick-play spell
                KewlTuneReco,     // Searcher/destroyer
                KewlTuneMix,      // Combo piece
                KewlTuneClip,     // Extra Deck hate
                JJKewlTune        // Field spell
            };
            
            foreach (int id in priority)
            {
                foreach (ClientCard card in cards)
                {
                    if (card.Id == id && !result.Contains(card))
                    {
                        result.Add(card);
                        if (result.Count >= max) return result;
                    }
                }
            }
            
            // Add remaining if needed
            foreach (ClientCard card in cards)
            {
                if (!result.Contains(card))
                {
                    result.Add(card);
                    if (result.Count >= max) break;
                }
            }
            
            return result;
        }

        private IList<ClientCard> SelectOptimalDestroyTargets(IList<ClientCard> cards, int max)
        {
            // Prioritize destroying biggest threats
            List<ClientCard> result = new List<ClientCard>();
            
            // Target opponent's monsters first
            foreach (ClientCard card in cards)
            {
                if (card.Controller == 1 && card.Location == CardLocation.MonsterZone)
                {
                    result.Add(card);
                    if (result.Count >= max) return result;
                }
            }
            
            // Then Spells/Traps
            foreach (ClientCard card in cards)
            {
                if (card.Controller == 1 && card.Location == CardLocation.SpellZone)
                {
                    result.Add(card);
                    if (result.Count >= max) return result;
                }
            }
            
            return result;
        }
    }
}
