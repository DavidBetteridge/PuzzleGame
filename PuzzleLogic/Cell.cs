using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle
{
    /// <summary>
    /// Represents a single cell within the board
    /// </summary>
    public class Cell
    {

        public bool HasTopWall { get; set; }
        public bool HasBottomWall { get; set; }
        public bool HasLeftWall { get; set; }
        public bool HasRightWall { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// What is currently in this cell?  (NULL if the cell is empty)
        /// </summary>
        public CellBase Contents { get; set; }

        public Cell(CellBase currentContent)
        {
            this.Contents = currentContent;
        }

        public Cell()
        {
        }
    }
}
