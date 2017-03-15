using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableStorage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TableOperations tblOps = new TableOperations();
            tblOps.TableOps();
        }
    }
}
