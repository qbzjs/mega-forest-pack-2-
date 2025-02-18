﻿using System;
using System.IO;
using System.Linq;

using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Common;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.CM.ConfigureHelper;
using Codice.LogWrapper;
using PlasticGui;
using PlasticPipe.Certificates;
using Unity.PlasticSCM.Editor.Configuration;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal static class PlasticApp
    {
        internal static void Initialize()
        {
            if (mInitialized)
                return;

            ConfigureLogging();

            RegisterExceptionHandlers();

            InitLocalization();

            ThreadWaiter.Initialize(new UnityThreadWaiterBuilder());
            ServicePointConfigurator.ConfigureServicePoint();
            CertificateUi.RegisterHandler(new ChannelCertificateUiImpl());

            DisableFsWatcherIfNeeded();

            EditionManager.Get().DisableCapability(
                EnumEditionCapabilities.Extensions);

            ClientHandlers.Register();

            PlasticGuiConfig.SetConfigFile(
                PlasticGuiConfig.UNITY_GUI_CONFIG_FILE);

            mInitialized = true;
        }

        internal static void Dispose()
        {
            UnRegisterExceptionHandlers();
        }

        static void InitLocalization()
        {
            string language = null;
            try
            {
                language = ClientConfig.Get().GetLanguage();
            }
            catch
            {
                language = string.Empty;
            }

            Localization.Init(language);
            PlasticLocalization.SetLanguage(language);
        }

        static void ConfigureLogging()
        {
            try
            {
                string log4netpath = ToolConfig.GetUnityPlasticLogConfigFile();

                if (!File.Exists(log4netpath))
                    WriteLogConfiguration.For(log4netpath);

                XmlConfigurator.Configure(new FileInfo(log4netpath));
            }
            catch
            {
                //it failed configuring the logging info; nothing to do.
            }
        }

        static void RegisterExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            Application.logMessageReceivedThreaded += HandleLog;
        }

        static void UnRegisterExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;

            Application.logMessageReceivedThreaded -= HandleLog;
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            if (IsExitGUIException(ex) ||
                !IsPlasticStackTrace(ex.StackTrace))
                return;

            GUIActionRunner.RunGUIAction(delegate {
                ExceptionsHandler.HandleException("HandleUnhandledException", ex);
            });
        }

        static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Exception)
                return;

            if (!IsPlasticStackTrace(stackTrace))
                return;

            GUIActionRunner.RunGUIAction(delegate {
                mLog.ErrorFormat("[HandleLog] Unexpected error: {0}", logString);
                mLog.DebugFormat("Stack trace: {0}", stackTrace);

                string message = logString;
                if (ExceptionsHandler.DumpStackTrace())
                    message += Environment.NewLine + stackTrace;

                GuiMessage.ShowError(message);
            });
        }

        static void DisableFsWatcherIfNeeded()
        {
            if (!DisableFsWatcher.MustDisableFsWatcher())
                return;

            WorkspaceWatcherFsNodeReadersCache.Get().DisableWatcher();
            Debug.LogWarning(
                "Plastic SCM: Detected an unsupported Runtime Version. " +
                "The Plastic SCM Filesystem Watcher is not compatible with this runtime, " +
                "and has been disabled.This can affect to the Pending Changes view performance, " +
                "and also causes that the auto - refresh feature for views is disabled. We strongly " +
                "recommend upgrading to the.NET 4.x equivalent Runtime Version for a better experience. " +
                "In order to change the Runtime Version go Edit > Project Settings > Player > Other Settings > Configuration > Scripting Runtime Version.");
        }

        static bool IsPlasticStackTrace(string stackTrace)
        {
            if (stackTrace == null)
                return false;

            string[] namespaces = new[] {
                "Codice.",
                "GluonGui.",
                "PlasticGui."
            };

            return namespaces.Any(stackTrace.Contains);
        }

        static bool IsExitGUIException(Exception ex)
        {
            return ex is ExitGUIException;
        }

        static bool mInitialized;

        static readonly ILog mLog = LogManager.GetLogger("PlasticApp");
    }
}
