﻿using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.History
{
    internal enum HistoryListColumn
    {
        Changeset,
        CreationDate,
        CreatedBy,
        Comment,
        Branch,
    }

    [Serializable]
    internal class HistoryListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static HistoryListHeaderState Default
        {
            get
            {
                var headerState = new HistoryListHeaderState(new Column[]
                {
                    new Column()
                    {
                        width = 100,
                        headerContent = new GUIContent(
                            GetColumnName(HistoryListColumn.Changeset)),
                        minWidth = 50,
                    },
                    new Column()
                    {
                        width = 250,
                        headerContent = new GUIContent(
                            GetColumnName(HistoryListColumn.CreationDate)),
                        minWidth = 100
                    },
                    new Column()
                    {
                        width = 250,
                        headerContent = new GUIContent(
                            GetColumnName(HistoryListColumn.CreatedBy)),
                        minWidth = 100
                    },
                    new Column()
                    {
                        width = 400,
                        headerContent = new GUIContent(
                            GetColumnName(HistoryListColumn.Comment)),
                        minWidth = 100
                    },
                    new Column()
                    {
                        width = 200,
                        headerContent = new GUIContent(
                            GetColumnName(HistoryListColumn.Branch)),
                        minWidth = 100
                    },
                });

                // NOTE(rafa): we cannot ensure that the order in the list is the same as in the enum
                // take extra care modifying columns list or the enum
                if (headerState.columns.Length != Enum.GetNames(typeof(HistoryListColumn)).Length)
                    throw new InvalidOperationException("header columns and Column enum must have the same size and order");

                return headerState;
            }
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.ChangesetColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.BranchColumn));
            return result;
        }

        internal static string GetColumnName(HistoryListColumn column)
        {
            switch (column)
            {
                case HistoryListColumn.Changeset:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.ChangesetColumn);
                case HistoryListColumn.CreationDate:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn);
                case HistoryListColumn.CreatedBy:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn);
                case HistoryListColumn.Comment:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn);
                case HistoryListColumn.Branch:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.BranchColumn);
                default:
                    return null;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (mHeaderTitles != null)
                TreeHeaderColumns.SetTitles(columns, mHeaderTitles);

            if (mColumsAllowedToggleVisibility != null)
                TreeHeaderColumns.SetVisibilities(columns, mColumsAllowedToggleVisibility);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        HistoryListHeaderState(Column[] columns)
            : base(columns)
        {
            if (mHeaderTitles == null)
                mHeaderTitles = TreeHeaderColumns.GetTitles(columns);

            if (mColumsAllowedToggleVisibility == null)
                mColumsAllowedToggleVisibility = TreeHeaderColumns.GetVisibilities(columns);
        }

        [SerializeField]
        string[] mHeaderTitles;

        [SerializeField]
        bool[] mColumsAllowedToggleVisibility;
    }
}
