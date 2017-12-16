using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.IO.Abstractions;
using System.Web;
using System.Web.Caching;
using Unity;

namespace GMO.Versioning.Tests
{
    [TestClass]
    public class VersioningTests
    {
        byte[] someData = new byte[8] { 139, 124, 66, 23, 87, 11, 62, 103 };

        [TestMethod]
        public void GetInstance()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.RegisterInstance(Mock.Of<HttpContextBase>());

            var v = Versioning.Instance;
        }

        [TestMethod]
        public void CreatesCorrectPathAndCheckums()
        {
            string filePath = "supyo.txt";
            var ms = new MemoryStream(someData);

            var mockedServerUtility = new Mock<HttpServerUtilityBase>();
            mockedServerUtility.Setup(x => x.MapPath(It.IsAny<string>())).Returns(filePath);

            var httpCache = new Cache();
            var mockedHttpCtx = new Mock<HttpContextBase>();

            mockedHttpCtx.SetupGet(x => x.Server).Returns(mockedServerUtility.Object);
            mockedHttpCtx.SetupGet(x => x.Cache).Returns(httpCache);

            var mockedFSW = new Mock<IFileWatcherService>();
            mockedFSW.Setup(x => x.CreateFileSystemWatcher(It.IsAny<string>()))
                .Returns(Mock.Of<FileSystemWatcherBase>());
            var mockedFS = new Mock<IFileSystem>();
            mockedFS.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);
            mockedFS.Setup(x => x.File.OpenRead(It.IsAny<string>())).Returns(ms);

            var container = UnityConfig.GetConfiguredContainer();
            container.RegisterInstance(mockedHttpCtx.Object);
            container.RegisterInstance(mockedFS.Object);
            container.RegisterInstance(mockedFSW.Object);

            var result = Versioning.AddChecksum(filePath);

            Assert.AreEqual(
                "supyo.txt?v=B6904CA20CF3967B8525DA9D19D7C5F90003524E3082FA0926EF3FE3148CC712",
                result
            );
        }
    }
}
