using Notes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UIImageTests
{
    public class UIImageTestInfo : IDisposable
    {
        public Window Window { get; private set; }

        public String FullTestName { get; private set; }

        public String OutputFilePath { get; private set; }

        public String ReferenceFilePath { get; private set; }

        public String DiffFilePath { get; private set; }

        public String FailingOutputFilePath { get; private set; }

        public UIImageTestInfo(String testSuiteName, string testName, int windowWidth, int windowHeight)
        {
            FullTestName = $"{testName}_{windowWidth}x{windowHeight}";
            Window = new Window(0, 0, windowWidth, windowHeight, "Notes");

            OutputFilePath = TestSupport.GetOutputFileForTest(testSuiteName, FullTestName);
            ReferenceFilePath = TestSupport.GetReferenceFileForTest(testSuiteName, FullTestName);
            DiffFilePath = TestSupport.GetDiffFileForTest(testSuiteName, FullTestName);
            FailingOutputFilePath = TestSupport.GetFailingOutputsFileForTest(testSuiteName, FullTestName);

            EnsureTestDirectoriesExist();
        }

        public void Dispose()
        {
            Window.Dispose();
        }
        private void EnsureTestDirectoriesExist()
        {
            if (!Directory.Exists(Path.GetDirectoryName(OutputFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(OutputFilePath));
            if (!Directory.Exists(Path.GetDirectoryName(ReferenceFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(ReferenceFilePath));
            if (!Directory.Exists(Path.GetDirectoryName(DiffFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(DiffFilePath));
            if (!Directory.Exists(Path.GetDirectoryName(FailingOutputFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(FailingOutputFilePath));
        }
    }
}
