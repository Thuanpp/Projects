using FTPServer.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer.Tests.Controllers
{
    [TestClass]
    public class FolderControllerTest
    {
        [TestMethod]
        public void Get()
        {
            FolderController controller = new FolderController();

            // Act
            IEnumerable<string> result = controller.Get();
            var a = "";
        }
    }
}
