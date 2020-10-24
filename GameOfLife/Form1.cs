using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        // TODO: 
        // class Cell with properies int X, int Y, Condition condition

        // TODO:
        // Infection of cells:
        // Какая-то вероятность A заразить N клеток из всех Живых каждый ход
        // Какая-то вероятность B вылечить M клеток из всех Заболевших
        // Условия: Заболевшая клетка может умереть только от одиночества 
        //          либо если вокруг нее только живые клетки;
        //          Живая клетка рядом с заболевшей становится зараженной


        /// <summary>
        /// Новая логика -- заболевание
        /// Живая любая клетка может заболеть и стать красной с вероятностью 5%
        /// Если рядом с живой клеткой есть заболевшая, то шанс заболевания у текущей увеличивается на 5%
        /// Правила жизни для зараженной клетки те же что и для здорвой
        /// Любая больная клетка может стать здоровой с вероятностью 10% 
        /// </summary>

        private int resolution;
        private Graphics graphics;
        private Cell[,] currentField;
        private Cell[,] newField;
        private int rows;
        private int cols;
        private int currentGeneration = 0;
        
        public Form1()
        {
            InitializeComponent();
        }

        // Началло игры
        private void StartGame()
        {
            if (timer1.Enabled)
                return;

            currentGeneration = 0;

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            resolution = (int)nudResolution.Value;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            currentField = new Cell[cols, rows];
           
            // Первое поколение
            Random rand = new Random();
            for(int x = 0; x < cols; x++)
            {                
                for (int y = 0; y < rows; y++)
                {
                    Condition condition;
                    int prob = rand.Next(0, 100);
                    if (prob <= 5)
                    {
                        condition = Condition.Infectious;
                    }
                    else
                    {
                        condition = (Condition)Convert.ToInt32(rand.Next((int)nudDensity.Value) == 0);
                    }               
                    currentField[x, y] = new Cell(x, y, condition);
                }
            }

            timer1.Start();
        }

        // Логика генерации нового поколения
        private void NextGeneration()
        {
            int alivedCurrentCells = 0;
            int alivedNewCells = 0;

            graphics.Clear(Color.Black);

            newField = new Cell[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {       
                    
                    int neighboursCount = CountNeighbours(x, y);

                    bool hasLife = currentField[x, y].Condition == Condition.Alived;

                    if (!hasLife && neighboursCount == 3)
                    {                       
                        newField[x, y] = new Cell(x, y, Condition.Alived); ;
                        alivedNewCells++;
                    }                        
                    else
                    {
                        if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                            newField[x, y] = new Cell(x, y, Condition.NotAlived);                       
                        else
                        {
                            newField[x, y] = currentField[x, y];
                            if (hasLife)
                                alivedNewCells++;
                        }                            
                    }
                    
                    if (currentField[x, y].Condition == Condition.Infectious)
                    {
                        // Отрисовка заболевшей клетки
                        graphics.FillRectangle(Brushes.Red, x * resolution, y * resolution, resolution, resolution);
                        alivedCurrentCells++;
                    }

                    if (hasLife)
                    {
                        // Отрисовка живой клетки
                        graphics.FillRectangle(Brushes.GreenYellow, x * resolution, y * resolution, resolution, resolution);
                        alivedCurrentCells++;
                    }
                }
            }

            if (alivedCurrentCells == 0)
            {
                labelProgress.Text = $"All cells are died :(";
                StopGame();
                pictureBox1.Refresh();
                btnStop.Visible = false;
                MessageBox.Show("Все клетки померли :(", "Игра окончена", MessageBoxButtons.OK, MessageBoxIcon.Information);               
            }
            else
            {
                double progress = alivedNewCells * 100 / alivedCurrentCells;
                labelProgress.Text = $"Progress: {progress} %";

                currentField = newField;
                pictureBox1.Refresh();
                Text = $"Generation {++currentGeneration}";
            }
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;
                    
                    bool isSelfChecking = col == x && row == y;
                    bool hasLife = currentField[col, row].Condition == Condition.Alived;

                    if (hasLife && !isSelfChecking)
                        count++;
                }
            }

            return count;
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;

            timer1.Enabled = false;
            nudDensity.Enabled = true;
            nudResolution.Enabled = true;
        }

        // Обработка нажатия кнопки Start
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartGame();
            btnStop.Visible = true; 
        }

        // Обработка нажатия кнопки Stop
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        // Обработка Таймера
        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;
            
            if(e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                bool validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                    currentField[x, y].Condition = Condition.Alived;
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                bool validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                    currentField[x, y].Condition = Condition.NotAlived;
            }
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Conway's Game of Life";
        }
    }
}
