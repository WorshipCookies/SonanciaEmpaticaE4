using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public interface ISpawnElement
    {
        int getTypeID();
        double tensionValue();
    }
}
