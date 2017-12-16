using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO.Abstractions;
using System.Web;
using System.Web.Caching;
using Unity;

namespace Versioning.Tests
{
    [TestClass]
    public class VersioningTests
    {
        [TestMethod]
        public Versioning GetInstance()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.RegisterInstance(Mock.Of<HttpContextBase>());

            return Versioning.Instance;
        }

        [TestMethod]
        public void CreatesCorrectPathAndCheckums()
        {
            string filePath = "supyo.txt";

            var httpCache = new Cache();
            var mockedHttpCtx = new Mock<HttpContextBase>();
            mockedHttpCtx.SetupGet(x => x.Server).Returns(Mock.Of<HttpServerUtilityBase>());
            mockedHttpCtx.SetupGet(x => x.Cache).Returns(httpCache);

            var mockedFSW = new Mock<IFileWatcherService>();
            mockedFSW.Setup(x => x.CreateFileSystemWatcher(It.IsAny<string>()))
                .Returns(Mock.Of<FileSystemWatcherBase>());

            var mockedFS = new Mock<IFileSystem>();
            mockedFS.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);
            mockedFS.Setup(x => x.File.OpenRead)
            var container = UnityConfig.GetConfiguredContainer();
            container.RegisterInstance(mockedHttpCtx.Object);
            container.RegisterInstance(Mock.Of<IFileSystem>());
            container.RegisterInstance(mockedFSW.Object);

            var v = GetInstance();

            var result = new PrivateObject(
                v,
                new PrivateType(
                    typeof(Versioning)
                )
            ).Invoke(
                "PathNChecksum",
                new object[] { filePath }
            );

            Assert.AreEqual("supyo.txt?v=")
        }

    }
}
