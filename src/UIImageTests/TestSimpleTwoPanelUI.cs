using Notes;
using Notes.UserInterfaces;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static UIImageTests.TestSupport;
using Notes.MarkdigRenderers;

namespace UIImageTests
{
    [TestFixture]
    public class TestSimpleTwoPanelUI
    {
        private UIImageTestInfo testInfo;

        [TearDown]
        public void TearDown()
        {
            if (testInfo != null) testInfo.Dispose();
        }

        public static IEnumerable<object[]> TestCaseParameters()
        {
            yield return new object[] { "AST", 1280, 800 };
            yield return new object[] { "Plain text", 1280, 800 };
            yield return new object[] { "AST", 500, 500 };
            yield return new object[] { "Plain text", 500, 500 };
            yield return new object[] { "AST", 200, 200 };
            yield return new object[] { "Plain text", 200, 200 };
            yield return new object[] { "AST", 200, 1000 };
            yield return new object[] { "Plain text", 200, 1000 };
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void EmptyInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(EmptyInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void SingleLineInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(SingleLineInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = "This is a single line!";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void MultipleParagraphInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(MultipleParagraphInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;


            userInterface.Note.Text = @"This is a single line!

This is another line!

This is a third line!";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void LineBreaksInParagraphInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(LineBreaksInParagraphInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;


            userInterface.Note.Text = @"This is a line in a paragraph!
This is another line in the same paragraph!
This is a third line in the same paragraph!";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void ListInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(ListInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"List of things:
 - thing one
 - thing two
   - thing two a
   - thing two b
 - thing three
   - thing three a
     - thing three a one";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void BoldAndItalicInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(BoldAndItalicInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"This is *italic* and this is **bold** and this is ***bold italic***!";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void HeadingsInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(HeadingsInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"# Heading One
## Heading Two
### Heading Three
#### Heading Four
##### Heading Five
###### Heading Six

Alternate Heading One
=====================

Alternate Heading Two
---------------------";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void HorizontalRulesInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(HorizontalRulesInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"Horizontal rules:

---

***

___";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        // test case input text taken from example at https://markdown-it.github.io/
        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void BlockQuotesInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(BlockQuotesInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"Block quotes:

> Blockquotes can also be nested...
>> ...by using additional greater-than signs right next to each other...
> > > ...or with spaces between arrows.";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        // test case input text taken from example at https://markdown-it.github.io/
        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void CodeInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(CodeInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"
## Code

Inline `code`

Indented code

    // Some comments
    line 1 of code
    line 2 of code
    line 3 of code


Block code ""fences""

```
Sample text here...
```

Syntax highlighting

``` js
var foo = function(bar) {
                return bar++;
            };

            console.log(foo(5));
```";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        // test case input text taken from example at https://markdown-it.github.io/
        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void OrderedListsInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(OrderedListsInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"
Ordered

1. Lorem ipsum dolor sit amet
2. Consectetur adipiscing elit
3. Integer molestie lorem at massa


1. You can use sequential numbers...
1. ...or keep all the numbers as `1.`

Start numbering with offset:

57. foo
1. bar";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }

        // test case input text taken from example at https://markdown-it.github.io/
        [Test]
        [TestCaseSource(nameof(TestCaseParameters))]
        public void NestedQuotesAndListsInInputText(string renderType, int windowWidth, int windowHeight)
        {
            var testName = $"{nameof(NestedQuotesAndListsInInputText)}_{renderType.Replace(" ", "")}Renderer";
            testInfo = new UIImageTestInfo(nameof(TestSimpleTwoPanelUI), testName, windowWidth, windowHeight);

            var userInterface = new SimpleTwoPanelUI();

            userInterface.CurrentRenderType = renderType;

            userInterface.Note.Text = @"
> test:
> - stuff
>   stuff and things
>   and things and stuff
>   > nested quote in the list item
> - next list item
>   > quote in the list item
> > top level double quote";

            testInfo.Window.UserInterface = userInterface;
            DoTest(testInfo);
        }
    }
}
