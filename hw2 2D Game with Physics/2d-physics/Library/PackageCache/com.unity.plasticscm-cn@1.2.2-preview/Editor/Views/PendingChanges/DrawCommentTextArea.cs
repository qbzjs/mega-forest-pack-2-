﻿using System.Reflection;

using UnityEditor;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    static class DrawCommentTextArea
    {
        internal static void For(
            PlasticGUIClient plasticClient,
            float width,
            bool isOperationRunning)
        {
            using (new GuiEnabled(!isOperationRunning))
            {
                EditorGUILayout.BeginHorizontal();

                Rect textAreaRect = BuildTextAreaRect(
                    plasticClient.CommentText, width);

                EditorGUI.BeginChangeCheck();

                plasticClient.CommentText = DoTextArea(
                    plasticClient.CommentText ?? string.Empty,
                    plasticClient.ForceToShowComment,
                    textAreaRect);

                plasticClient.ForceToShowComment = false;

                if (EditorGUI.EndChangeCheck())
                    OnTextAreaChanged(plasticClient, plasticClient.CommentText);

                if (string.IsNullOrEmpty(plasticClient.CommentText))
                {
                    DoPlaceholderIfNeeded(PlasticLocalization.GetString(
                        PlasticLocalization.Name.CheckinComment), textAreaRect);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        static void OnTextAreaChanged(PlasticGUIClient plasticClient, string text)
        {
            plasticClient.UpdateIsCommentWarningNeeded(text);
        }

        static string DoTextArea(string text, bool forceToShowText, Rect textAreaRect)
        {
            // while the text area has the focus, the changes to 
            // the source string will not be picked up by the text editor. 
            // so, when we want to change the text programmatically
            // we have to remove the focus, set the text and then reset the focus.

            TextEditor textEditor = typeof(EditorGUI)
                .GetField("activeEditor", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null) as TextEditor;

            bool shouldBeFocusFixed = forceToShowText && textEditor != null;

            if (shouldBeFocusFixed)
                EditorGUIUtility.keyboardControl = 0;

            text = EditorGUI.TextArea(
                textAreaRect, text, EditorStyles.textArea);

            if (shouldBeFocusFixed)
                EditorGUIUtility.keyboardControl = textEditor.controlID;

            return text;
        }

        static void DoPlaceholderIfNeeded(string placeholder, Rect textAreaRect)
        {
            int textAreaControlId = GUIUtility.GetControlID(FocusType.Passive) - 1;

            if (EditorGUIUtility.keyboardControl == textAreaControlId)
                return;

            Rect hintRect = textAreaRect;
            hintRect.height = EditorStyles.textArea.lineHeight;

            GUI.Label(hintRect, placeholder,
                UnityStyles.PendingChangesTab.CommentPlaceHolder);
        }

        static Rect BuildTextAreaRect(string text, float width)
        {
            GUIStyle commentTextAreaStyle =
                UnityStyles.PendingChangesTab.CommentTextArea;

            float contentWidth = width -
                commentTextAreaStyle.margin.left -
                commentTextAreaStyle.margin.right;

            float requiredHeight = commentTextAreaStyle
                .CalcHeight(new GUIContent(text), contentWidth);

            Rect result = GUILayoutUtility.GetRect(
                contentWidth, Mathf.Max(requiredHeight, 42));
            result.x += commentTextAreaStyle.margin.left;
            result.width = contentWidth;
            result.height = Mathf.Max(result.height, 42);

            return result;
        }
    }
}
