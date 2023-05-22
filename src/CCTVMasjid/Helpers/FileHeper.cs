using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CCTVMasjid.Helpers
{
    public class FileHeper
    {
        public static string GetAppPath()
        {
            var path = System.IO.Path.GetDirectoryName(
System.Reflection.Assembly.GetExecutingAssembly().Location);

            return path;
        }
    }
}
