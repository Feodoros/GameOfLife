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
        private int resolution;
        private Graphics graphics;
        private bool[,] field;
        private int rows;
        private int cols;

        public Form1()
        {
            InitializeComponent();
        }

        // Началло игры
        private void StartGame()
        {
            if (timer1.Enabled)
                return;

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            resolution = (int)nudResolution.Value;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            field = new bool[cols, rows];

            // Первое поколение
            Random rand = new Random();
            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    field[x, y] = rand.Next((int)nudDensity.Value) == 0;
                    if (field[x, y])
                    {
                        //graphics.FillRectangle(Brushes.GreenYellow, x * resolution, y * resolution, resolution, resolution);
                    }
                }
            }

            timer1.Start();
        }

        private void NextGeneration()
        {
            graphics.Clear(Color.Black);

            bool[,] newField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int neighbours = CountNeighbours(x, y);

                    bool hasLife = field[x, y];

                    if (!hasLife && neighbours == 3)
                        newField[x, y] = true;
                    else
                    {
                        if (hasLife && (neighbours < 2 || neighbours > 3))
                            newField[x, y] = false;
                        else
                            newField[x, y] = field[x, y];
                    }

                    if (hasLife)
                    {
                        // Отрисовка живой клетки
                        graphics.FillRectangle(Brushes.GreenYellow, x * resolution, y * resolution, resolution, resolution);                       
                    }
                }
            }

            field = newField;
            pictureBox1.Refresh();
        }

        private int CountNeighbours(int x, int y)
        {

            return 0;
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

       
    }
}
