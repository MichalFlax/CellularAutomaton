using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CellularAutomaton.Rules
{
    class GameOfLife : DefaultRule
    {
        /// <summary>
        /// Returns cell default state, which is value 0.
        /// </summary>
        public override int DefaultState
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Compute next state for all cell in automaton.
        /// </summary>
        /// <param name="cells">Cells in 2D automaton</param>
        public override void ComputeNextState(List<List<Cell>> cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException();
            }

            Parallel.For(0, cells.Count, i =>
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    int surrondings = 0;

                    // i-1, j-1
                    if (i > 0 && j > 0)
                        surrondings += cells[i - 1][j - 1].StateOld;
                    // i-1,j
                    if (i > 0)
                        surrondings += cells[i - 1][j].StateOld;
                    // i-1, j+1
                    if (i > 0 && j < cells[i].Count - 1)
                        surrondings += cells[i - 1][j + 1].StateOld;
                    // i, j+1
                    if (j < cells[i].Count - 1)
                        surrondings += cells[i][j + 1].StateOld;
                    // i+1, j+1
                    if (i < cells.Count - 1 && j < cells[i].Count - 1)
                        surrondings += cells[i + 1][j + 1].StateOld;
                    // i+1, j
                    if (i < cells.Count - 1)
                        surrondings += cells[i + 1][j].StateOld;
                    // i+1, j-1
                    if (i < cells.Count - 1 && j > 0)
                        surrondings += cells[i + 1][j - 1].StateOld;
                    // i, j-1
                    if (j > 0)
                        surrondings += cells[i][j - 1].StateOld;
                    
                    if (cells[i][j].StateOld == 0 && surrondings == 3)
                        cells[i][j].StateNew = 1;
                    if (cells[i][j].StateOld == 1 && surrondings != 2 && surrondings != 3)
                        cells[i][j].StateNew = 0;
                }
            });
        }

        /// <summary>
        /// Return black color if cell state is 1, else return white color.
        /// </summary>
        /// <param name="state">Cell state</param>
        /// <returns></returns>
        public override Brush GetStateColor(int state)
        {
            return state == 1 ? Brushes.Black : Brushes.White;
        }

        /// <summary>
        /// Randomize state of all cells in cellular automaton.
        /// </summary>
        /// <param name="cells">Cells in 2D automaton.</param>
        public override void RandomizeAutomaton(List<List<Cell>> cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException("Initialize automaton before randomize cells.");
            }

            var rand = new Random((int)DateTime.Now.Ticks);

            foreach (var row in cells)
            {
                foreach (var c in row)
                {
                    c.StateOld = rand.Next(0, 2);
                }
            }
        }

        public override string ToString()
        {
            return "Game of Life";
        }
    }
}
