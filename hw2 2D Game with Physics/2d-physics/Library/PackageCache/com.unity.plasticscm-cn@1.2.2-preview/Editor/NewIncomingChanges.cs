﻿using System;

using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui.WorkspaceWindow;
using Unity.PlasticSCM.Editor.UI;
using UnityEngine;
using GluonNewIncomingChangesUpdater = PlasticGui.Gluon.WorkspaceWindow.NewIncomingChangesUpdater;
using GluonCheckIncomingChanges = PlasticGui.Gluon.WorkspaceWindow.CheckIncomingChanges;

namespace Unity.PlasticSCM.Editor
{
    internal static class NewIncomingChanges
    {
        internal static NewIncomingChangesUpdater BuildUpdaterForDeveloper(
            WorkspaceInfo wkInfo,
            CheckIncomingChanges.IAutoRefreshIncomingChangesView autoRefreshIncomingChangesView,
            CheckIncomingChanges.IUpdateIncomingChanges updateIncomingChanges)
        {
            if (!ClientConfig.Get().GetClientConfigData().IsIncomingChangesEnabled())
                return null;

            NewIncomingChangesUpdater updater = new NewIncomingChangesUpdater(
                new UnityPlasticTimerBuilder(), updateIncomingChanges);
            updater.SetAutoRefreshIncomingChangesView(
                autoRefreshIncomingChangesView);

            updater.SetWorkspace(wkInfo);
            updater.Start();
            return updater;
        }

        internal static AutoSetup.NewIncomingChangesUpdater BuildUpdaterFromMainForDeveloper(
            WorkspaceInfo wkInfo, CheckIncomingChanges.IUpdateIncomingChanges updateIncomingChanges)
        {
            if (!ClientConfig.Get().GetClientConfigData().IsIncomingChangesEnabled())
                return null;

            AutoSetup.NewIncomingChangesUpdater updater = new AutoSetup.NewIncomingChangesUpdater(
                new UnityPlasticTimerBuilder(), updateIncomingChanges);

            updater.SetWorkspace(wkInfo);
            updater.Start();
            return updater;
        }
        
        internal static GluonNewIncomingChangesUpdater BuildUpdaterForGluon(
            WorkspaceInfo wkInfo,
            GluonCheckIncomingChanges.IAutoRefreshIncomingChangesView autoRefreshIncomingChangesView,
            GluonCheckIncomingChanges.IUpdateIncomingChanges updateIncomingChanges,
            GluonCheckIncomingChanges.ICalculateIncomingChanges calculateIncomingChanges)
        {
            if (!ClientConfig.Get().GetClientConfigData().IsGluonIncomingChangesEnabled())
                return null;

            GluonNewIncomingChangesUpdater updater = new GluonNewIncomingChangesUpdater(
                wkInfo,
                new UnityPlasticTimerBuilder(),
                updateIncomingChanges,
                autoRefreshIncomingChangesView,
                calculateIncomingChanges);

            updater.Start();
            return updater;
        }

        internal static void LaunchUpdater(
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            GluonNewIncomingChangesUpdater gluonNewIncomingChangesUpdater,
            AutoSetup.NewIncomingChangesUpdater developerNewIncomingChangesFromMainUpdater)
        {
            if (developerNewIncomingChangesUpdater != null)
            {
                developerNewIncomingChangesUpdater.Start();
                developerNewIncomingChangesUpdater.Update();
            }

            if (gluonNewIncomingChangesUpdater != null)
            {
                gluonNewIncomingChangesUpdater.Start();
                gluonNewIncomingChangesUpdater.Update(DateTime.Now);
            }

            if (developerNewIncomingChangesFromMainUpdater != null)
            {
                developerNewIncomingChangesFromMainUpdater.Start();
                developerNewIncomingChangesFromMainUpdater.Update();
            }
        }

        internal static void StopUpdater(
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            GluonNewIncomingChangesUpdater gluonNewIncomingChangesUpdater,
            AutoSetup.NewIncomingChangesUpdater developerNewIncomingChangesFromMainUpdater)
        {
            if (developerNewIncomingChangesUpdater != null)
                developerNewIncomingChangesUpdater.Stop();

            if (gluonNewIncomingChangesUpdater != null)
                gluonNewIncomingChangesUpdater.Stop();

            if (developerNewIncomingChangesFromMainUpdater != null)
                developerNewIncomingChangesFromMainUpdater.Stop();
        }

        internal static void DisposeUpdater(
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            GluonNewIncomingChangesUpdater gluonNewIncomingChangesUpdater,
            AutoSetup.NewIncomingChangesUpdater developerNewIncomingChangesFromMainUpdater)
        {
            if (developerNewIncomingChangesUpdater != null)
                developerNewIncomingChangesUpdater.Dispose();

            if (gluonNewIncomingChangesUpdater != null)
                gluonNewIncomingChangesUpdater.Dispose();

            if (developerNewIncomingChangesFromMainUpdater != null)
                developerNewIncomingChangesFromMainUpdater.Dispose();
        }
    }
}
