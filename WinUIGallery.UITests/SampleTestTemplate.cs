//******************************************************************************
//
// Copyright (c) 2024 Microsoft Corporation. All rights reserved.
//
// This code is licensed under the MIT License (MIT).
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//******************************************************************************

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace WinUIGallery.UITests;

[TestClass]
public class SampleTestTemplate : TestBase
{

    private static WindowsElement element1 = null;
    private static WindowsElement element2 = null;

    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("MyControlPage");
        Thread.Sleep(1000);
        element1 = Session.FindElementByAccessibilityId("Element Locator");
        Assert.IsNotNull(element1);
        element2 = Session.FindElementByAccessibilityId("Element Locator");
        Assert.IsNotNull(element2);
    }
}
