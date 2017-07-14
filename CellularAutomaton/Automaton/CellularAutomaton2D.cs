using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using CellularAutomaton.Rules;

namespace CellularAutomaton
{
    /// <summary>
    /// Class for reprezenting 2D cellular automaton.
    /// </summary>
    public class CellularAutomaton2D
    {
        public int[] Size { get; private set; }

        public string Name { get; private set; }

        private DefaultRule rules;

        private List<List<Cell>> cells;

        public int Generation { get; private set; }

        /// <summary>
        /// Contructor for 2D cellular automaton.
        /// </summary>
        /// <param name="width">Width of array of cells</param>
        /// <param name="height">Height of array of cells</param>
        /// <param name="rules">Rules for cellular automaton</param>
        /// <param name="name">Name of instance of cellular automaton</param>
        public CellularAutomaton2D(int width, int height, DefaultRule rules, string name = "DefaultName")
        {
            Size = new int[2] { width, height };

            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("Width and height of cellular automat have to be positive number");
            }

            if (rules == null)
            {
                throw new ArgumentException("Parameter rules is null.");
            }

            this.rules = rules;

            Name = name;

            cells = new List<List<Cell>>();

            for (int i = 0; i < height; i++)
            {
                cells.Add(new List<Cell>());

                for (int j = 0; j < width; j++)
                {
                    cells[i].Add(new Cell(rules.DefaultState));
                }
            }

        }

        /// <summary>
        /// Return color, which represents state of selected cell.
        /// </summary>
        /// <param name="x">X coordination</param>
        /// <param name="y">Y coordination</param>
        /// <returns>Color</returns>
        public Brush GetCellColor(int x, int y)
        {
            if (x < 0 || x >= cells.Count || y < 0 || y >= cells[0].Count)
            {
                throw new ArgumentOutOfRangeException("Invalid coordinates");
            }

            return rules.GetStateColor(cells[x][y].StateOld);
        }

        /// <summary>
        /// Compute next state for all cells in automaton.
        /// </summary>
        public void ComputeNextState()
        {
            rules.ComputeNextState(cells);

            foreach (var row in cells)
            {
                foreach (var cell in row)
                {
                    cell.StateOld = cell.StateNew;
                }
            }

            Generation++;
        }

        /// <summary>
        /// All cells are set to default state.
        /// </summary>
        public void Reset()
        {
            foreach (var row in cells)
            {
                foreach (var c in row)
                {
                    c.StateOld = rules.DefaultState;
                }
            }

            Generation = 0;
        }

        /// <summary>
        /// Randomize state of all cells.
        /// </summary>
        public void Randomize()
        {
            rules.RandomizeAutomaton(cells);

            Generation = 0;
        }

        /// <summary>
        /// This indexer returns selected cell from automaton.
        /// </summary>
        /// <param name="x">X coordination</param>
        /// <param name="y">Y coordination</param>
        /// <returns>Selected cell</returns>
        public Cell this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= cells.Count || y < 0 || y >= cells[0].Count)
                {
                    throw new ArgumentOutOfRangeException("Invalid coordinates");
                }

                return cells[x][y];
            }
        }
        
        /// <summary>
        /// Set state of selected cell.
        /// </summary>
        /// <param name="x">X coordination</param>
        /// <param name="y">Y coordination</param>
        /// <param name="state">State of cells</param>
        public void SetCellState(int x, int y, int state)
        {
            cells[x][y].StateOld = cells[x][y].StateNew = state;
        }

        /// <summary>
        /// Serialize cells of automaton and save cells to selected file.
        /// </summary>
        /// <param name="path">Path to file</param>
        public void Serialize(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    var serializer = new BinaryFormatter();

                    serializer.Serialize(fs, cells);
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Load and deserialize cells of automaton from selected file.
        /// </summary>
        /// <param name="path">Path to file</param>
        public void Deserialize(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var serializer = new BinaryFormatter();

                    cells = serializer.Deserialize(fs) as List<List<Cell>>;
                }

                Generation = 0;
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
