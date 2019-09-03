﻿using System.Collections.Generic;

namespace SnapIt.Library.Entities
{
    public class LayoutArea
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public bool Merged { get; set; }
        public List<LayoutArea> Areas { get; set; }
    }
}