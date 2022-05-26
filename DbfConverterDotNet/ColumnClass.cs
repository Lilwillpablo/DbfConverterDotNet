using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbfConverterDotNet
{
    public class Column
    {
        public Column(string ColumnName, string columnType, int ColumnSize)
        {
            this.ColumnName = ColumnName;
            this.ColumnType = columnType;
            this.ColumnSize = ColumnSize;
        }

        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int ColumnSize { get; set; }
    }
}

