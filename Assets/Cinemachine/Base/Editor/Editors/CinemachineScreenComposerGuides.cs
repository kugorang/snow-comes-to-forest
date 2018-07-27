#region

using Cinemachine.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    internal class CinemachineScreenComposerGuides
    {
        public delegate SerializedObject ObjectGetter();

        public delegate Rect RectGetter();

        public delegate void RectSetter(Rect r);

        public const float kGuideBarWidthPx = 3f;
        private readonly Rect[] mDragBars = new Rect[9];

        // Clients MUST implement all of these
        public RectGetter GetHardGuide;
        public RectGetter GetSoftGuide;
        private DragBar mDragging = DragBar.NONE;
        public RectSetter SetHardGuide;
        public RectSetter SetSoftGuide;
        public ObjectGetter Target;

        public void SetNewBounds(Rect oldHard, Rect oldSoft, Rect newHard, Rect newSoft)
        {
            if (oldSoft != newSoft || oldHard != newHard)
            {
                Undo.RecordObject(Target().targetObject, "Composer Bounds");
                if (oldSoft != newSoft)
                    SetSoftGuide(newSoft);
                if (oldHard != newHard)
                    SetHardGuide(newHard);
                Target().ApplyModifiedProperties();
            }
        }

        public void OnGUI_DrawGuides(bool isLive, Camera outputCamera, LensSettings lens, bool showHardGuides)
        {
            var cameraRect = outputCamera.pixelRect;
            var screenWidth = cameraRect.width;
            var screenHeight = cameraRect.height;
            cameraRect.yMax = Screen.height - cameraRect.yMin;
            cameraRect.yMin = cameraRect.yMax - screenHeight;

            // Rotate the guides along with the dutch
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Translate(cameraRect.min);
            GUIUtility.RotateAroundPivot(lens.Dutch, cameraRect.center);

            var hardBarsColour = CinemachineSettings.ComposerSettings.HardBoundsOverlayColour;
            var softBarsColour = CinemachineSettings.ComposerSettings.SoftBoundsOverlayColour;
            var overlayOpacity = CinemachineSettings.ComposerSettings.OverlayOpacity;
            if (!isLive)
            {
                softBarsColour = CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;
                hardBarsColour = Color.Lerp(softBarsColour, Color.black, 0.5f);
                overlayOpacity /= 2;
            }

            hardBarsColour.a *= overlayOpacity;
            softBarsColour.a *= overlayOpacity;

            var r = showHardGuides ? GetHardGuide() : new Rect(-2, -2, 4, 4);
            var hardEdgeLeft = r.xMin * screenWidth;
            var hardEdgeTop = r.yMin * screenHeight;
            var hardEdgeRight = r.xMax * screenWidth;
            var hardEdgeBottom = r.yMax * screenHeight;

            mDragBars[(int) DragBar.HardBarLineLeft] = new Rect(hardEdgeLeft - kGuideBarWidthPx / 2f, 0f,
                kGuideBarWidthPx, screenHeight);
            mDragBars[(int) DragBar.HardBarLineTop] =
                new Rect(0f, hardEdgeTop - kGuideBarWidthPx / 2f, screenWidth, kGuideBarWidthPx);
            mDragBars[(int) DragBar.HardBarLineRight] = new Rect(hardEdgeRight - kGuideBarWidthPx / 2f, 0f,
                kGuideBarWidthPx, screenHeight);
            mDragBars[(int) DragBar.HardBarLineBottom] = new Rect(0f, hardEdgeBottom - kGuideBarWidthPx / 2f,
                screenWidth, kGuideBarWidthPx);

            r = GetSoftGuide();
            var softEdgeLeft = r.xMin * screenWidth;
            var softEdgeTop = r.yMin * screenHeight;
            var softEdgeRight = r.xMax * screenWidth;
            var softEdgeBottom = r.yMax * screenHeight;

            mDragBars[(int) DragBar.SoftBarLineLeft] = new Rect(softEdgeLeft - kGuideBarWidthPx / 2f, 0f,
                kGuideBarWidthPx, screenHeight);
            mDragBars[(int) DragBar.SoftBarLineTop] =
                new Rect(0f, softEdgeTop - kGuideBarWidthPx / 2f, screenWidth, kGuideBarWidthPx);
            mDragBars[(int) DragBar.SoftBarLineRight] = new Rect(softEdgeRight - kGuideBarWidthPx / 2f, 0f,
                kGuideBarWidthPx, screenHeight);
            mDragBars[(int) DragBar.SoftBarLineBottom] = new Rect(0f, softEdgeBottom - kGuideBarWidthPx / 2f,
                screenWidth, kGuideBarWidthPx);

            mDragBars[(int) DragBar.Center] = new Rect(softEdgeLeft, softEdgeTop, softEdgeRight - softEdgeLeft,
                softEdgeBottom - softEdgeTop);

            // Handle dragging bars
            if (isLive)
                OnGuiHandleBarDragging(screenWidth, screenHeight);

            // Draw the masks
            GUI.color = hardBarsColour;
            var hardBarLeft = new Rect(0, hardEdgeTop, Mathf.Max(0, hardEdgeLeft), hardEdgeBottom - hardEdgeTop);
            var hardBarRight = new Rect(hardEdgeRight, hardEdgeTop,
                Mathf.Max(0, screenWidth - hardEdgeRight), hardEdgeBottom - hardEdgeTop);
            var hardBarTop = new Rect(Mathf.Min(0, hardEdgeLeft), 0,
                Mathf.Max(screenWidth, hardEdgeRight) - Mathf.Min(0, hardEdgeLeft), Mathf.Max(0, hardEdgeTop));
            var hardBarBottom = new Rect(Mathf.Min(0, hardEdgeLeft), hardEdgeBottom,
                Mathf.Max(screenWidth, hardEdgeRight) - Mathf.Min(0, hardEdgeLeft),
                Mathf.Max(0, screenHeight - hardEdgeBottom));
            GUI.DrawTexture(hardBarLeft, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(hardBarTop, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(hardBarRight, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(hardBarBottom, Texture2D.whiteTexture, ScaleMode.StretchToFill);

            GUI.color = softBarsColour;
            var softBarLeft = new Rect(hardEdgeLeft, softEdgeTop, softEdgeLeft - hardEdgeLeft,
                softEdgeBottom - softEdgeTop);
            var softBarTop = new Rect(hardEdgeLeft, hardEdgeTop, hardEdgeRight - hardEdgeLeft,
                softEdgeTop - hardEdgeTop);
            var softBarRight = new Rect(softEdgeRight, softEdgeTop, hardEdgeRight - softEdgeRight,
                softEdgeBottom - softEdgeTop);
            var softBarBottom = new Rect(hardEdgeLeft, softEdgeBottom, hardEdgeRight - hardEdgeLeft,
                hardEdgeBottom - softEdgeBottom);
            GUI.DrawTexture(softBarLeft, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(softBarTop, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(softBarRight, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(softBarBottom, Texture2D.whiteTexture, ScaleMode.StretchToFill);

            // Draw the drag bars
            GUI.DrawTexture(mDragBars[(int) DragBar.SoftBarLineLeft], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int) DragBar.SoftBarLineTop], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int) DragBar.SoftBarLineRight], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int) DragBar.SoftBarLineBottom], Texture2D.whiteTexture,
                ScaleMode.StretchToFill);

            GUI.color = hardBarsColour;
            GUI.DrawTexture(mDragBars[(int) DragBar.HardBarLineLeft], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int) DragBar.HardBarLineTop], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int) DragBar.HardBarLineRight], Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(mDragBars[(int) DragBar.HardBarLineBottom], Texture2D.whiteTexture,
                ScaleMode.StretchToFill);

            GUI.matrix = oldMatrix;
        }

        private void OnGuiHandleBarDragging(float screenWidth, float screenHeight)
        {
            if (Event.current.type == EventType.MouseUp)
                mDragging = DragBar.NONE;
            if (Event.current.type == EventType.MouseDown)
            {
                mDragging = DragBar.NONE;
                for (var i = DragBar.Center; i < DragBar.NONE && mDragging == DragBar.NONE; ++i)
                {
                    var slop = new Vector2(5f, 5f);
                    if (i == DragBar.Center)
                    {
                        if (mDragBars[(int) i].width > 3f * slop.x)
                            slop.x = -slop.x;
                        if (mDragBars[(int) i].height > 3f * slop.y)
                            slop.y = -slop.y;
                    }

                    var r = mDragBars[(int) i].Inflated(slop);
                    if (r.Contains(Event.current.mousePosition))
                        mDragging = i;
                }
            }

            if (mDragging != DragBar.NONE && Event.current.type == EventType.MouseDrag)
            {
                var d = new Vector2(
                    Event.current.delta.x / screenWidth,
                    Event.current.delta.y / screenHeight);

                // First snapshot some settings
                var newHard = GetHardGuide();
                var newSoft = GetSoftGuide();
                var changed = Vector2.zero;
                switch (mDragging)
                {
                    case DragBar.Center:
                        newSoft.position += d;
                        break;
                    case DragBar.SoftBarLineLeft:
                        newSoft = newSoft.Inflated(new Vector2(-d.x, 0));
                        break;
                    case DragBar.SoftBarLineRight:
                        newSoft = newSoft.Inflated(new Vector2(d.x, 0));
                        break;
                    case DragBar.SoftBarLineTop:
                        newSoft = newSoft.Inflated(new Vector2(0, -d.y));
                        break;
                    case DragBar.SoftBarLineBottom:
                        newSoft = newSoft.Inflated(new Vector2(0, d.y));
                        break;
                    case DragBar.HardBarLineLeft:
                        newHard = newHard.Inflated(new Vector2(-d.x, 0));
                        break;
                    case DragBar.HardBarLineRight:
                        newHard = newHard.Inflated(new Vector2(d.x, 0));
                        break;
                    case DragBar.HardBarLineBottom:
                        newHard = newHard.Inflated(new Vector2(0, d.y));
                        break;
                    case DragBar.HardBarLineTop:
                        newHard = newHard.Inflated(new Vector2(0, -d.y));
                        break;
                }

                // Apply the changes, enforcing the bounds
                SetNewBounds(GetHardGuide(), GetSoftGuide(), newHard, newSoft);
                InternalEditorUtility.RepaintAllViews();
            }
        }

        // For dragging the bars - order defines precedence
        private enum DragBar
        {
            Center,
            SoftBarLineLeft,
            SoftBarLineTop,
            SoftBarLineRight,
            SoftBarLineBottom,
            HardBarLineLeft,
            HardBarLineTop,
            HardBarLineRight,
            HardBarLineBottom,
            NONE
        }
    }
}