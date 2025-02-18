﻿using UnityEditor;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui.WorkspaceWindow.QueryViews.Changesets;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    internal class ChangesetsViewMenu
    {
        internal interface IMenuOperations
        {
            void DiffBranch();
            ChangesetExtendedInfo GetSelectedChangeset();
        }

        internal ChangesetsViewMenu(
            IChangesetMenuOperations changesetMenuOperations,
            IMenuOperations menuOperations)
        {
            mChangesetMenuOperations = changesetMenuOperations;
            mMenuOperations = menuOperations;

            BuildComponents();
        }


        internal void Popup()
        {
            GenericMenu menu = new GenericMenu();

            UpdateMenuItems(menu);

            menu.ShowAsContext();
        }

        void DiffChangesetMenuItem_Click()
        {
            mChangesetMenuOperations.DiffChangeset();
        }

        void DiffSelectedChangesetsMenuItem_Click()
        {
            mChangesetMenuOperations.DiffSelectedChangesets();
        }
        void DiffBranchMenuItem_Click()
        {
            mMenuOperations.DiffBranch();
        }

        void UpdateMenuItems(GenericMenu menu)
        {
            ChangesetExtendedInfo singleSeletedChangeset = mMenuOperations.GetSelectedChangeset();

            ChangesetMenuOperations operations = ChangesetMenuUpdater.GetAvailableMenuOperations(
                mChangesetMenuOperations.GetSelectedChangesetsCount());

            AddDiffChangesetMenuItem(
                mDiffChangesetMenuItemContent,
                menu,
                singleSeletedChangeset,
                operations,
                DiffChangesetMenuItem_Click);

            AddDiffSelectedChangesetsMenuItem(
                mDiffSelectedChangesetsMenuItemContent,
                menu,
                operations,
                DiffSelectedChangesetsMenuItem_Click);

            if (IsOnMainBranch(singleSeletedChangeset))
                return;

            menu.AddSeparator(string.Empty);

            AddDiffBranchMenuItem(
               mDiffBranchMenuItemContent,
               menu,
               singleSeletedChangeset,
               operations,
               DiffBranchMenuItem_Click);
        }

        static void AddDiffChangesetMenuItem(
            GUIContent menuItemContent,
            GenericMenu menu,
            ChangesetExtendedInfo changeset,
            ChangesetMenuOperations operations,
            GenericMenu.MenuFunction menuFunction)
        {
            string changesetName =
                changeset != null ?
                changeset.ChangesetId.ToString() :
                string.Empty;

            menuItemContent.text = string.Format(
                "Diff changeset {0}", changesetName);

            if (operations.HasFlag(ChangesetMenuOperations.DiffChangeset))
            {
                menu.AddItem(
                    menuItemContent,
                    false,
                    menuFunction);
                return;
            }

            menu.AddDisabledItem(
                menuItemContent);
        }

        static void AddDiffSelectedChangesetsMenuItem(
            GUIContent menuItemContent,
            GenericMenu menu,
            ChangesetMenuOperations operations,
            GenericMenu.MenuFunction menuFunction)
        {
            if (operations.HasFlag(ChangesetMenuOperations.DiffSelectedChangesets))
            {
                menu.AddItem(
                    menuItemContent,
                    false,
                    menuFunction);

                return;
            }

            menu.AddDisabledItem(menuItemContent);
        }

        static void AddDiffBranchMenuItem(
            GUIContent menuItemContent,
            GenericMenu menu,
            ChangesetExtendedInfo changeset,
            ChangesetMenuOperations operations,
            GenericMenu.MenuFunction menuFunction)
        {
            string branchName = GetBranchName(changeset);

            menuItemContent.text = string.Format(
                "Diff branch {0}", branchName);

            if (operations.HasFlag(ChangesetMenuOperations.DiffChangeset))
            {
                menu.AddItem(
                    menuItemContent,
                    false,
                    menuFunction);
                return;
            }

            menu.AddDisabledItem(
                menuItemContent);
        }

        void BuildComponents()
        {
            mDiffChangesetMenuItemContent = new GUIContent();
            mDiffSelectedChangesetsMenuItemContent = new GUIContent("Diff selected changesets");
            mDiffBranchMenuItemContent = new GUIContent();
        }

        static string GetBranchName(ChangesetExtendedInfo changesetInfo)
        {
            if (changesetInfo == null)
                return string.Empty;

            string branchName = changesetInfo.BranchName;

            int lastIndex = changesetInfo.BranchName.LastIndexOf("/");

            if (lastIndex == -1)
                return branchName;

            return branchName.Substring(lastIndex + 1);
        }

        static bool IsOnMainBranch(ChangesetExtendedInfo singleSeletedChangeset)
        {
            if (singleSeletedChangeset == null)
                return false;

            return singleSeletedChangeset.BranchName == MAIN_BRANCH_NAME;
        }

        GUIContent mDiffChangesetMenuItemContent;
        GUIContent mDiffSelectedChangesetsMenuItemContent;
        GUIContent mDiffBranchMenuItemContent;

        readonly IChangesetMenuOperations mChangesetMenuOperations;
        readonly IMenuOperations mMenuOperations;

        const string MAIN_BRANCH_NAME = "/main";
    }
}
