using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownLoadContactsImage
{
    class Program
    {
        static void Main(string[] args)
        {
            //CRMHelper obJCRMHElper = new CRMHelper();
            //obJCRMHElper.setSrvice();
            Contact objContact = new Contact();
            objContact.DownloadContactImages();

            Console.WriteLine("Weldone");
        }
    }
}
