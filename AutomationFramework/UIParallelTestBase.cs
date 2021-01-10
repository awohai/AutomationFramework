﻿using AutomationFramework.Entities;
using AutomationFramework.Enums;
using NUnit.Framework;
using OpenQA.Selenium;
using System.IO;

namespace AutomationFramework
{
    public class UIParallelTestBase
    {
        protected RunSettingManager _runSettingsSettings;
        protected LogManager _logManager;
        protected ToolsManager _toolsManager;

        public virtual void OneTimeSetUpParallelExec()
        {
            _runSettingsSettings = new RunSettingManager();

            Directory.CreateDirectory(_runSettingsSettings.TestsReportDirectory);
            Directory.CreateDirectory(_runSettingsSettings.TestsAssetDirectory);

            _logManager = LogManager.GetLogManager(_runSettingsSettings);
            _toolsManager = ToolsManager.GetToolsManager(_runSettingsSettings, _logManager);
        }

        ///<summary>
        ///Before Each UI Test for parallel execution
        ///</summary>
        public void UITestSetUpParallelExec(
            out WebDriverManager webDriverManager
            )
        {
            _logManager.CreateTestFoldersAndLog(TestContext.CurrentContext);

            webDriverManager = WebDriverManager.GetWebDriverManager(_runSettingsSettings, _logManager);

            _logManager._driver = webDriverManager._driver;
        }

        ///<summary>
        ///After Each UI Test for parallel execution
        ///</summary>
        public void UITestTearDownParallelExec(WebDriverManager webDriverManager)
        {
            webDriverManager.Quit(_runSettingsSettings.Browser);
        }
    }
}
