using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CellularAutomaton.Rules
{
    /// <summary>
    /// This abstract class define set of methods and properties, 
    /// which should implement class with cellular automaton rules.
    /// </summary>
    public abstract class DefaultRule
    {
        public abstract void ComputeNextState(List<List<Cell>> cells);
        
        public abstract void RandomizeAutomaton(List<List<Cell>> cells);

        public abstract int DefaultState { get; }

        public abstract Brush GetStateColor(int state);
    }
}
