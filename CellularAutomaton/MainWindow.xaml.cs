using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CellularAutomaton.Rules;
using Microsoft.Win32;

namespace CellularAutomaton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CellularAutomaton2D automaton;

        List<List<Rectangle>> cellsCanvas;

        bool runningMode = false;

        int cellArrayWidth, cellArrayHeight;

        int size;

        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            size = 10;

            cellArrayWidth = (int) canvasCells.Width / size;
            cellArrayHeight = (int) canvasCells.Height / size;

            var rules = new GameOfLife();

            this.Title = "Cellural Automaton - " + rules.ToString();

            automaton = new CellularAutomaton2D(cellArrayWidth, cellArrayHeight, rules);

            CreateCanvasCellArray(cellArrayWidth, cellArrayHeight, size);

            UpdateCanvasCells();           
        }

        /// <summary>
        /// Create array rectangles on canvas. One cell is automaton is represented by one rectangle.
        /// </summary>
        /// <param name="width">Cell array width</param>
        /// <param name="height">Cell array height</param>
        /// <param name="size">Size of one cell</param>
        private void CreateCanvasCellArray(int width, int height, int size)
        {
            cellsCanvas = new List<List<Rectangle>>();

            for (int i = 0; i < height; i++)
            {
                cellsCanvas.Add(new List<Rectangle>());

                for (int j = 0; j < width; j++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Width = size,
                        Height = size,
                        Stroke = Brushes.LightSlateGray,
                        StrokeThickness = 0.5,
                    };

                    cellsCanvas[i].Add(rect);

                    Canvas.SetTop(rect, i * size);
                    Canvas.SetLeft(rect, j * size);

                    canvasCells.Children.Add(rect);
                }
            }
        }

        /// <summary>
        /// Update color of all rectangles in canvas array.
        /// </summary>
        private void UpdateCanvasCells()
        {
            for (int i = 0; i < cellArrayHeight; i++)
            {
                for (int j = 0; j < cellArrayWidth; j++)
                {
                    var newColor = automaton.GetCellColor(i, j);

                    if (cellsCanvas[i][j].Fill != newColor)
                    {
                        cellsCanvas[i][j].Fill = newColor;
                    }
                }
            }

            textBlockGeneration.Text = $"Generation: {automaton.Generation}";
        }

        /// <summary>
        /// Compute next state of cells in automaton.
        /// </summary>
        private void ComputeNextState()
        {
            automaton.ComputeNextState();

            UpdateCanvasCells();
        }

        /// <summary>
        /// EventHandler for button Run
        /// </summary>
        private void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            if (runningMode == false)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                
                dispatcherTimer.Start();

                runningMode = true;
            }
        }

        /// <summary>
        /// EventHandler for dispatcher timer.
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (runningMode == true)
            {
                ComputeNextState();
            }
        }

        /// <summary>
        /// EventHandler for item Exit in menu.
        /// </summary>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// EventHandler for save current state of automaton to file.
        /// </summary>
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            dialog.FileName = "CellularAutomaton";
            
            if (dialog.ShowDialog() == true)
            {
                automaton.Serialize(dialog.FileName);
            }
        }

        /// <summary>
        /// EventHandler for load state of automaton from file.
        /// </summary>
        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                automaton.Deserialize(dialog.FileName);
                UpdateCanvasCells();
            }
        }

        /// <summary>
        /// EventHandler for button Randomize - randomize of state of all cells.
        /// </summary>
        private void buttonRandomize_Click(object sender, RoutedEventArgs e)
        {
            if (automaton != null && runningMode == false)
            {
                automaton.Randomize();
                UpdateCanvasCells();
            }
        }

        /// <summary>
        /// EventHandler for button Reset - set state of all cells to default value.
        /// </summary>
        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            automaton.Reset();
            UpdateCanvasCells();

            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                runningMode = false;
            }
        }

        /// <summary>
        /// EventHandler for select cells.
        /// </summary>
        private void canvasCells_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(canvasCells);

            int x = (int) position.X / size;
            int y = (int) position.Y / size;

            automaton.SetCellState(y, x, 1);
            UpdateCanvasCells();
        }

        /// <summary>
        /// EventHandler for deselect cells.
        /// </summary>
        private void canvasCells_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(canvasCells);

            int x = (int) position.X / size;
            int y = (int) position.Y / size;

            automaton.SetCellState(y, x, 0);
            UpdateCanvasCells();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Button Run - run simulation.");
            builder.AppendLine("Button Stop - stop/pause simulation.");
            builder.AppendLine("Button Randomize - randomize state of all cells in automaton.");
            builder.AppendLine("Button Reset - reset all cells in automaton to default state.");

            builder.AppendLine();

            builder.AppendLine("Mouse left click - set selected cell to state 1.");
            builder.AppendLine("Mouse right click - set selected cell to state 0.");

            MessageBox.Show(builder.ToString(), "Help");
        }

        /// <summary>
        /// EventHandler for button Stop
        /// </summary>
        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            if (runningMode == true)
            {
                dispatcherTimer.Stop();
                runningMode = false;
            }
        }
    }
}
