using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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

        private const float lineWidth = 0.1f;

        private int resolution;
        private Graphics graphics;
        private Cell[,] currentField;
        private Cell[,] newField;
        private int rows;
        private int cols;
        private int currentGeneration = 0;
        private bool isPlaying = false;

        public Form1()
        {
            InitializeComponent();

            resolution = (int)nudResolution.Value;

            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            pictureBox1.BackColor = Color.Black;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);

            currentGeneration = 0;
            currentField = new Cell[cols, rows];
        }

        // Первое рандомное поколение
        private void RandomFirstGeneration()
        {
            currentGeneration = 0;
            currentField = new Cell[cols, rows];

            // Первое поколение
            Random rand = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Condition condition;
                    int prob = rand.Next(0, 100);

                    condition = (Condition)Convert.ToInt32(rand.Next((int)nudDensity.Value) == 0);

                    // Вероятность стать зараженной клеткой 5%
                    if (condition == Condition.Alived && prob <= 5)
                    {
                        condition = Condition.Infectious;
                    }

                    currentField[x, y] = new Cell(x, y, condition);
                }
            }
        }

        // Логика генерации нового поколения
        private void NextGeneration()
        {
            int alivedCurrentCells = 1;
            int alivedNewCells = 0;

            newField = new Cell[cols, rows];

            for (int x = 1; x < cols-1; x++)
            {
                for (int y = 1; y < rows-1; y++)
                {

                    int neighboursCount = CountNeighbours(x, y);

                    bool hasLife = currentField[x, y]?.Condition == Condition.Alived;

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

                    /*if (currentField[x, y].Condition == Condition.Infectious)
                    {                        
                        alivedCurrentCells++;
                    }

                    if (hasLife)
                    {
                        alivedCurrentCells++;
                    }                   

                    if (currentField[x, y].Condition == Condition.NotAlived)
                    {
                        // Отрисовка живой клетки
                        graphics.FillRectangle(Brushes.Black, x * resolution + lineWidth,
                            y * resolution + lineWidth, resolution - lineWidth, resolution - lineWidth);
                        alivedCurrentCells++;
                    }*/
                }
            }

            if (alivedCurrentCells == 0)
            {
                labelProgress.Text = $"All cells are died :(";
                StopGame();
                pictureBox1.Refresh();
                btnClear.Visible = false;
                MessageBox.Show("Все клетки померли :(", "Игра окончена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                double progress = alivedNewCells * 100 / alivedCurrentCells;
                labelProgress.Text = $"Progress: {progress} %";
                DrawCells();
                currentField = newField;
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
                    try
                    {
                        bool hasLife = currentField[col, row]?.Condition == Condition.Alived;
                        if (hasLife && !isSelfChecking)
                            count++;
                    }
                    catch { continue; }                    
                }
            }

            return count;
        }

        private void StopGame()
        {
            currentGeneration = 0;
            currentField = new Cell[cols, rows];

            graphics.Clear(Color.Black);

            DrawLines();

            timer1.Enabled = false;
            nudDensity.Enabled = true;
            nudResolution.Enabled = true;
        }

        // Обработка Таймера
        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            /*if (!timer1.Enabled)
                return;*/

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                bool validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                {
                    Cell cell = new Cell(x, y, Condition.Alived);
                    currentField[x, y] = cell;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                bool validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                {
                    Cell cell = new Cell(x, y, Condition.NotAlived);
                    currentField[x, y] = cell;
                }
            }
            DrawCells();
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Conway's Game of Life";
        }

        private void checkBoxLines_CheckStateChanged(object sender, EventArgs e)
        {
            DrawLines();
        }

        private void DrawCells()
        {
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (currentField[x, y] == null)
                    {
                        continue;
                    }
                    try
                    {
                        // Отрисовка заболевшей клетки
                        if (currentField[x, y].Condition == Condition.Infectious)
                        {
                            graphics.FillRectangle(Brushes.Red, x * resolution + lineWidth,
                                y * resolution + lineWidth, resolution - lineWidth, resolution - lineWidth);
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); continue; }

                    try
                    {
                        // Отрисовка живой клетки
                        if (currentField[x, y].Condition == Condition.Alived)
                        {
                            graphics.FillRectangle(Brushes.GreenYellow, x * resolution + lineWidth,
                                y * resolution + lineWidth, resolution - lineWidth, resolution - lineWidth);
                        }

                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); continue; }

                    try
                    {
                        // Отрисовка живой клетки
                        if (currentField[x, y].Condition == Condition.NotAlived)
                        {
                            graphics.FillRectangle(Brushes.Black, x * resolution + lineWidth,
                                y * resolution + lineWidth, resolution - lineWidth, resolution - lineWidth);
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); continue; }
                }
            }
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                timer1.Start();
                btnPause.Text = "Pause";
                isPlaying = true;
                btnRndFstGen.Enabled = false;
                nudDensity.Enabled = false;
                nudResolution.Enabled = false;
                btnClear.Enabled = false;
            }
            else
            {
                timer1.Stop();
                btnPause.Text = "Play";
                isPlaying = false;
                btnRndFstGen.Enabled = true;
                nudDensity.Enabled = true;
                nudResolution.Enabled = true;
                btnClear.Enabled = true;
            }
        }

        private void btnRndFstGen_Click(object sender, EventArgs e)
        {
            RandomFirstGeneration();
            DrawCells();
        }

        private void nudResolution_ValueChanged(object sender, EventArgs e)
        {
            resolution = (int)nudResolution.Value;

            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            graphics.Clear(Color.Black);

            DrawLines();
        }

        private void DrawLines()
        {
            if (checkBoxLines.Checked)
            {
                nudResolution.Enabled = false;
                nudDensity.Enabled = false;

                // Отрисовка линий
                for (int i = 0; i < cols + 1; i++)
                {
                    graphics.DrawLine(new Pen(Color.FromArgb(25, Color.White), lineWidth), resolution * i, 0, resolution * i, pictureBox1.Height);
                }
                for (int i = 0; i < rows + 1; i++)
                {
                    graphics.DrawLine(new Pen(Color.FromArgb(25, Color.White), lineWidth), 0, resolution * i, pictureBox1.Width, resolution * i);
                }

                nudResolution.Enabled = true;
                nudDensity.Enabled = true;

                pictureBox1.Refresh();
            }
            else
            {
                nudResolution.Enabled = false;
                nudDensity.Enabled = false;

                // Убираем линии
                for (int i = 0; i < cols + 1; i++)
                {
                    graphics.DrawLine(new Pen(Color.Black, lineWidth), resolution * i, 0, resolution * i, pictureBox1.Height);

                }
                for (int i = 0; i < rows + 1; i++)
                {
                    graphics.DrawLine(new Pen(Color.Black, lineWidth), 0, resolution * i, pictureBox1.Width, resolution * i);
                }

                nudResolution.Enabled = true;
                nudDensity.Enabled = true;

                pictureBox1.Refresh();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            StopGame();
        }
    }
}
