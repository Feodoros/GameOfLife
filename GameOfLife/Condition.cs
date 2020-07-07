using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    /// <summary>
    /// Возможные состояния клетки
    /// </summary>
    public enum Condition
    {
        NotAlived = 0,
        Alived = 1, 
        Stable = 2,
        Infectious = 3
    }
}
