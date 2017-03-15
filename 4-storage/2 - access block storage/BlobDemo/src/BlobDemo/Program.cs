using BlobDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlobDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            BlopOps blobOps = new BlopOps();
            blobOps.BasicBlobOps();
            blobOps.DownloadBlob();
        }
    }
}
