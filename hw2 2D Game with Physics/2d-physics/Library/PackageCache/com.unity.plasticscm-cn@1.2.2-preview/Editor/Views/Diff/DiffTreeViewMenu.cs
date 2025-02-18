﻿using UnityEditor;
using UnityEngine;

using PlasticGui;
using PlasticGui.WorkspaceWindow.Diff;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal class DiffTreeViewMenu
    {
        internal interface IMetaMenuOperations
        {
            bool SelectionHasMeta();
            void DiffMeta();
            void HistoryMeta();
        }

        internal DiffTreeViewMenu(
            IDiffTreeViewMenuOperations operations,
            IMetaMenuOperations metaMenuOperations)
        {
            mOperations = operations;
            mMetaMenuOperations = metaMenuOperations;

            BuildComponents();
        }

        internal void Popup()
        {
            GenericMenu menu = new GenericMenu();

            UpdateMenuItems(menu);

            menu.ShowAsContext();
        }

        internal bool ProcessKeyActionIfNeeded(Event e)
        {
            DiffTreeViewMenuOperations operationToExecute = GetMenuOperation(e);

            if (operationToExecute == DiffTreeViewMenuOperations.None)
                return false;

            SelectedDiffsGroupInfo info =
                mOperations.GetSelectedDiffsGroupInfo();

            DiffTreeViewMenuOperations operations =
                DiffTreeViewMenuUpdater.GetAvailableMenuOperations(info);

            if (!operations.HasFlag(operationToExecute))
                return false;

            ProcessMenuOperation(operationToExecute, mOperations);
            return true;
        }

        void DiffMenuItem_Click()
        {
            mOperations.Diff();
        }

        void DiffMetaMenuItem_Click()
        {
            mMetaMenuOperations.DiffMeta();
        }

        void HistoryMenuItem_Click()
        {
            mOperations.History();
        }

        void HistoryMetaMenuItem_Click()
        {
            mMetaMenuOperations.HistoryMeta();
        }

        void RevertMenuItem_Click()
        {
            mOperations.RevertChanges();
        }

        void UndeleteMenuItem_Click()
        {
            mOperations.Undelete();
        }

        void UndeleteToSpecifiedPathMenuItem_Click()
        {
            mOperations.UndeleteToSpecifiedPaths();
        }

        void UpdateMenuItems(GenericMenu menu)
        {
            SelectedDiffsGroupInfo groupInfo =
                mOperations.GetSelectedDiffsGroupInfo();

            DiffTreeViewMenuOperations operations =
                DiffTreeViewMenuUpdater.GetAvailableMenuOperations(groupInfo);

            if (operations == DiffTreeViewMenuOperations.None)
            {
                menu.AddDisabledItem(GetNoActionMenuItemContent());
                return;
            }

            bool isMultipleSelection = groupInfo.SelectedItemsCount > 1;
            bool selectionHasMeta = mMetaMenuOperations.SelectionHasMeta();

            if (operations.HasFlag(DiffTreeViewMenuOperations.Diff))
                menu.AddItem(mDiffMenuItemContent, false, DiffMenuItem_Click);
            else
                menu.AddDisabledItem(mDiffMenuItemContent, false);

            if (mMetaMenuOperations.SelectionHasMeta())
            {
                if (operations.HasFlag(DiffTreeViewMenuOperations.Diff))
                    menu.AddItem(mDiffMetaMenuItemContent, false, DiffMetaMenuItem_Click);
                else
                    menu.AddDisabledItem(mDiffMetaMenuItemContent);
            }

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(DiffTreeViewMenuOperations.History))
                menu.AddItem(mViewHistoryMenuItemContent, false, HistoryMenuItem_Click);
            else
                menu.AddDisabledItem(mViewHistoryMenuItemContent, false);

            if (mMetaMenuOperations.SelectionHasMeta())
            {
                if (operations.HasFlag(DiffTreeViewMenuOperations.History))
                    menu.AddItem(mViewHistoryMetaMenuItemContent, false, HistoryMetaMenuItem_Click);
                else
                    menu.AddDisabledItem(mViewHistoryMetaMenuItemContent, false);
            }

            if (operations.HasFlag(DiffTreeViewMenuOperations.RevertChanges))
            {
                menu.AddSeparator(string.Empty);

                mRevertMenuItemContent.text = GetRevertMenuItemText(
                    isMultipleSelection,
                    selectionHasMeta);

                menu.AddItem(mRevertMenuItemContent, false, RevertMenuItem_Click);
            }

            if (operations.HasFlag(DiffTreeViewMenuOperations.Undelete) ||
                operations.HasFlag(DiffTreeViewMenuOperations.UndeleteToSpecifiedPaths))
            {
                menu.AddSeparator(string.Empty);
            }

            if (operations.HasFlag(DiffTreeViewMenuOperations.Undelete))
            {
                mUndeleteMenuItemContent.text = GetUndeleteMenuItemText(
                    isMultipleSelection,
                    selectionHasMeta);

                menu.AddItem(mUndeleteMenuItemContent, false, UndeleteMenuItem_Click);
            }

            if (operations.HasFlag(DiffTreeViewMenuOperations.UndeleteToSpecifiedPaths))
            {
                mUndeleteToSpecifiedPathMenuItemContent.text = GetUndeleteToSpecifiedPathMenuItemText(
                    isMultipleSelection,
                    selectionHasMeta);

                menu.AddItem(mUndeleteToSpecifiedPathMenuItemContent, false, UndeleteToSpecifiedPathMenuItem_Click);
            }
        }

        GUIContent GetNoActionMenuItemContent()
        {
            if (mNoActionMenuItemContent == null)
            {
                mNoActionMenuItemContent = new GUIContent(
                    PlasticLocalization.GetString(PlasticLocalization.
                        Name.NoActionMenuItem));
            }

            return mNoActionMenuItemContent;
        }

        static string GetRevertMenuItemText(
            bool isMultipleSelection,
            bool selectionHasMeta)
        {
            if (selectionHasMeta && !isMultipleSelection)
                return "Undo this change +meta";

            return isMultipleSelection ?
                "Undo selected changes" : "Undo this change";
        }

        static string GetUndeleteMenuItemText(
            bool isMultipleSelection,
            bool selectionHasMeta)
        {
            if (selectionHasMeta && !isMultipleSelection)
                return "Undelete revision +meta";

            return isMultipleSelection ?
                "Undelete selected revisions" : "Undelete revision";
        }

        static string GetUndeleteToSpecifiedPathMenuItemText(
            bool isMultipleSelection,
            bool selectionHasMeta)
        {
            if (selectionHasMeta && !isMultipleSelection)
                return "Undelete revision +meta to specified path...";

            return isMultipleSelection ?
                "Undelete selected revisions to specified paths..." :
                "Undelete revision to specified path...";
        }

        static void ProcessMenuOperation(
            DiffTreeViewMenuOperations operationToExecute,
            IDiffTreeViewMenuOperations operations)
        {
            if (operationToExecute == DiffTreeViewMenuOperations.Diff)
            {
                operations.Diff();
                return;
            }

            if (operationToExecute == DiffTreeViewMenuOperations.History)
            {
                operations.History();
                return;
            }
        }

        static DiffTreeViewMenuOperations GetMenuOperation(Event e)
        {
            if (Keyboard.IsControlOrCommandKeyPressed(e) && Keyboard.IsKeyPressed(e, KeyCode.D))
                return DiffTreeViewMenuOperations.Diff;

            if (Keyboard.IsControlOrCommandKeyPressed(e) && Keyboard.IsKeyPressed(e, KeyCode.H))
                return DiffTreeViewMenuOperations.History;

            return DiffTreeViewMenuOperations.None;
        }

        void BuildComponents()
        {
            mDiffMenuItemContent = new GUIContent("Diff %d");
            mDiffMetaMenuItemContent = new GUIContent("Diff .meta");
            mViewHistoryMenuItemContent = new GUIContent("View file history %h");
            mViewHistoryMetaMenuItemContent = new GUIContent("View .meta file history");
            mRevertMenuItemContent = new GUIContent();
            mUndeleteMenuItemContent = new GUIContent();
            mUndeleteToSpecifiedPathMenuItemContent = new GUIContent();
        }

        GUIContent mNoActionMenuItemContent;

        GUIContent mDiffMenuItemContent;
        GUIContent mDiffMetaMenuItemContent;
        GUIContent mViewHistoryMenuItemContent;
        GUIContent mViewHistoryMetaMenuItemContent;
        GUIContent mRevertMenuItemContent;
        GUIContent mUndeleteMenuItemContent;
        GUIContent mUndeleteToSpecifiedPathMenuItemContent;

        readonly IDiffTreeViewMenuOperations mOperations;
        readonly IMetaMenuOperations mMetaMenuOperations;
    }
}
