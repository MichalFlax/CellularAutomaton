using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomaton
{
    /// <summary>
    /// Object of this class represents one cell in cellular automata.
    /// </summary>
    [Serializable]
    public class Cell
    {
        public int StateOld { get; set; }
        public int StateNew { get; set; }

        public Cell(int startingState)
        {
            StateNew = startingState;
            StateOld = StateNew;
        }

        public override string ToString()
        {
            return $"{StateOld}";
        }
    }
}
