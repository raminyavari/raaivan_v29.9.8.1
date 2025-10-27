using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public interface ITableType
    {
        string MSSQLName { get; }

        object[] to_array();
    }
}
