﻿//******************************************************************************
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace UITests.Tests
{
	[TestClass]
	public class PersonPicture : TestBase
	{
		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			OpenControlPage("PersonPicture");
		}

		[TestMethod]
		public void SwitchOptions()
		{
			GetElementByName("Profile Image").Click();
			GetElementByName("Display Name").Click();
			GetElementByName("Initials").Click();
		}
	}
}
