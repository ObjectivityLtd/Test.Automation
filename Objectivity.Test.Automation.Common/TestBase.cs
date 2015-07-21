﻿/*
The MIT License (MIT)

Copyright (c) 2015 Objectivity Bespoke Software Specialists

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Objectivity.Test.Automation.Common
{
    using System;
    using System.Collections.Generic;

    using Helpers;

    using NUnit.Framework;

    using Objectivity.Test.Automation.Common.Types;

    /// <summary>
    /// Class contains method for all tests, should be used in project test base
    /// </summary>
    public class TestBase
    {
        private readonly List<ErrorDetail> verifyMessages = new List<ErrorDetail>();

        /// <summary>
        /// The browser manager
        /// </summary>
        protected BrowserManager BrowserManager
        {
            get
            {
                return new BrowserManager();
            }
        }

        /// <summary>
        /// Gets or sets the test title.
        /// </summary>
        /// <value>
        /// The test title.
        /// </value>
        protected string TestTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [test failed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [test failed]; otherwise, <c>false</c>.
        /// </value>
        protected bool IsTestFailed { get; set; }

        /// <summary>
        /// Run before the class.
        /// </summary>
        protected static void StartPerformanceMeasure()
        {
            PerformanceHelper.Instance = new PerformanceHelper();
        }

        /// <summary>
        /// Run after the class.
        /// </summary>
        protected static void StopPerfromanceMeasure()
        {
            PerformanceHelper.Instance.PrintAveragePercentiles90DurationMilliseconds();
        }

        /// <summary>
        /// Stops the browser.
        /// </summary>
        protected void StopBrowser()
        {
            BrowserManager.Stop();
        }

        /// <summary>
        /// Run before the test.
        /// </summary>
        protected void StartBrowser()
        {
            BrowserManager.Start(BaseConfiguration.TestBrowser);
        }

        /// <summary>
        /// Take screenshot if test failed and delete cached page objects.
        /// </summary>
        protected void FinalizeTest()
        {
            if (IsTestFailed)
            {
                var screenshot = BrowserManager.TakeScreenshot();
                BrowserManager.SaveScreenshot(new ErrorDetail(screenshot, DateTime.Now, null), TestTitle);
            }

            Pages.DeleteCachedPages();
        }

        /// <summary>
        /// Verify assert conditions
        /// </summary>
        /// <param name="myAssert">
        /// Assert condition
        /// </param>
        public void Verify(Action myAssert)
        {
            try
            {
                myAssert();
            }
            catch (AssertionException e)
            {
                var screenshot = BrowserManager.TakeScreenshot();
                this.verifyMessages.Add(new ErrorDetail(BrowserManager.TakeScreenshot(), DateTime.Now, e));
                BrowserManager.SaveScreenshot(new ErrorDetail(screenshot, DateTime.Now, null), TestTitle);
                Console.WriteLine("\n-----VERIFY FAILS-----\n" + e + "\n-------------------\n");
            }
        }

        /// <summary>
        /// Fails the test if verify failed.
        /// </summary>
        public void FailTestIfVerifyFailed()
        {
            if (!this.verifyMessages.Count.Equals(0) && !IsTestFailed)
            {
                Assert.Fail();
            }
        }
    }
}