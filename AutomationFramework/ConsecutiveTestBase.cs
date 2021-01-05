﻿using AutomationFramework.Entities;
using NUnit.Framework;
using System.IO;

namespace AutomationFramework
{
    public class ConsecutiveTestBase
    {
        protected RunSettingManager _runSettingsSettings;
        protected LogManager _logManager;
        protected ToolsManager _toolsManager;
        protected WebDriverManager _webDriverManager;


        ///<summary>
        ///Once Before UI Tests 
        ///</summary>
        public virtual void OneTimeSetUp()
        {
            _runSettingsSettings = new RunSettingManager();

            Directory.CreateDirectory(_runSettingsSettings.TestsReportDirectory);
            Directory.CreateDirectory(_runSettingsSettings.TestsAssetDirectory);
        }

        ///<summary>
        ///Before Each UI Test 
        ///</summary>
        public virtual void SetUp()
        {
            _logManager = LogManager.GetLogManager(_runSettingsSettings);
            _logManager.CreateTestFoldersAndLog(TestContext.CurrentContext);

            _toolsManager = ToolsManager.GetToolsManager(_runSettingsSettings, _logManager);
            _webDriverManager = WebDriverManager.GetWebDriverManager(_runSettingsSettings, _logManager);

            _logManager._driver = _webDriverManager._driver;
        }

        ///<summary>
        ///After Each UI Test 
        ///</summary>
        public virtual void OneTearDown()
        {
            _webDriverManager.Quit(_runSettingsSettings.Browser);
        }
    }
}
