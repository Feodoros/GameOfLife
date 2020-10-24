using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    /// <summary>
    /// Интерфесй, отвечающий за поведение клетки
    /// </summary>
    class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Condition Condition { get; set; } // Состояние клетки
        public List<Cell> Neighbours { get; set; }
        public int NeighboursCount => Neighbours.Count;

        public Cell(int x, int y, Condition condition)
        {
            X = x;
            Y = y;
            Condition = condition;
        }
    }
}
