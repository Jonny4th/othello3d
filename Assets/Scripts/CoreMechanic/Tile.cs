using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Tile
    {
        public Dictionary<int, Tile> Neighbors { get; private set; } = new();
        public Faction CurrentFaction {get; private set;} = Faction.None;
        public Ability Ability { get; private set; }

        public Tile()
        {
            Ability = new BasicOutFlank(this);
        }

        #region Links
        public void SetLinkedTiles(Dictionary<int, Tile> linkedTiles)
        {
            Neighbors = linkedTiles;
        }

        public void SetLinkedTile(int direction, Tile tile)
        {
            Neighbors[direction] = tile;
        }

        /// <summary>
        /// Return surrounding Tiles.
        /// </summary>
        /// <returns></returns>
        public Tile[] GetNeighbors()
        {
            return Neighbors.Values.ToArray();
        }

        /// <summary>
        /// Use recursion to return all Tiles off opposite faction between itself and a blank tile in one direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="accumulator">only for recursion</param>
        /// <returns>Array of outflankable Tiles</returns>
        public Tile[] GetPosibleOutflankInDirection(int direction, Faction? startFaction = null, List<Tile> accumulator = null)
        {
            if(accumulator == null && CurrentFaction == Faction.None) //if initial tile is blank, stop immediately.
            {
                return new Tile[0];
            }

            accumulator ??= new(); // true start.
            startFaction ??= CurrentFaction;

            if(Neighbors.TryGetValue(direction, out var next))
            {
                if(next.CurrentFaction == startFaction) return new Tile[0]; // next is same faction, meaning no possible outflanks.

                if(next.CurrentFaction == Faction.None) return accumulator.ToArray(); // end found.

                accumulator.Add(next); // next is oppos, add to outflankables.
            }
            else // no next means at rim, fail to find possible move.
            {
                return new Tile[0];
            }
            
            return next.GetPosibleOutflankInDirection(direction, startFaction, accumulator);
        }

        /// <summary>
        /// Use recursion to get all the Tiles in one direction. Conbine with the result from opposite direction to form a line.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="accumulator">only for recursion</param>
        /// <returns></returns>
        public Tile[] GetAllTilesInDirection(int direction, List<Tile> accumulator = null)
        {
            accumulator ??= new();

            if(Neighbors.TryGetValue(direction, out var next))
            {
                accumulator.Add(next);
            }
            else
            {
                return accumulator.ToArray();
            }

            return next.GetAllTilesInDirection(direction, accumulator);
        }
        #endregion

        public void SetFaction(Faction faction)
        {
            CurrentFaction = faction;
        }

        public void SetAbility(Ability ability)
        {
            Ability = ability;
        }
    }

    public enum AbilityTrigger
    {
        OnEntrySelf,
        OnEntryOppos,
        OnOutflanked,
        OnTurnEnd,
        OnTurnStart,
    }

    public struct AbilityContext
    {
    }

    public abstract class Ability
    {
        protected Tile m_Self;
        public abstract bool IsTriggered(AbilityTrigger trigger);
        public abstract BoardState Execute(AbilityContext context);

        public Ability(Tile self)
        {
            m_Self = self; 
        }
    }

    public class BasicOutFlank : Ability
    {
        public BasicOutFlank(Tile self) : base(self)
        {
        }

        public override BoardState Execute(AbilityContext context)
        {
            
            return new();
        }

        public override bool IsTriggered(AbilityTrigger trigger)
        {
            return trigger == AbilityTrigger.OnEntrySelf;
        }
    }
}
