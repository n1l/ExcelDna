﻿using System;
using System.IO;
using ExcelDna.AddIn.Tasks.Logging;

namespace ExcelDna.AddIn.Tasks.Utils
{
    internal sealed class ExcelDnaProject : IExcelDnaProject
    {
        private readonly IBuildLogger _log;
        private readonly IDevToolsEnvironment _dte;

        public ExcelDnaProject(IBuildLogger log, IDevToolsEnvironment dte)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _dte = dte ?? throw new ArgumentNullException(nameof(dte));
        }

        public bool TrySetDebuggerOptions(string projectName, string excelExePath, string excelAddInToDebug)
        {
            try
            {
                MessageFilter.Register();

                var project = _dte.GetProjectByName(projectName);
                if (project != null)
                {
                    _log.Debug($"Found project: {project.Name}");
                    var configuration = project
                        .ConfigurationManager
                        .ActiveConfiguration;

                    var startAction = configuration.Properties.Item("StartAction");
                    var startProgram = configuration.Properties.Item("StartProgram");
                    var startArguments = configuration.Properties.Item("StartArguments");

                    startAction.Value = 1; // Start external program
                    startProgram.Value = excelExePath;
                    startArguments.Value = $@"""{Path.GetFileName(excelAddInToDebug)}""";

                    project.Save(string.Empty);

                    return true;
                }

                return false;
            }
            finally
            {
                MessageFilter.Revoke();
            }
        }
    }
}
