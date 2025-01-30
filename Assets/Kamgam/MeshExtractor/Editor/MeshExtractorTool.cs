﻿using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#if UNITY_2021_2_OR_NEWER
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;
#else
using PrefabStage = UnityEditor.Experimental.SceneManagement.PrefabStage;
#endif

namespace Kamgam.MeshExtractor
{
    [EditorTool("Mesh Extractor")]
    partial class MeshExtractorTool : EditorTool
    {
        public override GUIContent toolbarIcon
        {
            get
            {
                if(UtilsEditor.IsLightTheme())
                    return EditorGUIUtility.IconContent("PreMatCube@2x");
                else
                    return EditorGUIUtility.IconContent("d_PreMatCube@2x");
            }
        }

        

        public enum Mode { PickObjects, PaintSelection, ExtractMesh }

        public static MeshExtractorTool Instance;

        public GameObject[] _selectedObjects = new GameObject[] { };

        protected Mode _mode = Mode.PickObjects;

        protected Tool _toolBefore;

        // flags & temp
        protected bool _selectionChanged;
        protected bool _mouseIsDown;
        protected bool _mouseIsInSceneView;
        protected bool _mouseEnteredSceneView;
        protected bool _leftMouseIsDown;
        protected bool _leftMouseWasPressed;
        protected bool _leftMouseWasReleased;
        protected bool _shiftPressed;
        protected bool _altPressed;
        protected bool _controlPressed;
        protected bool _scrollWheelTurned;
        protected double _lastMouseDragTime;
        protected double _lastScrollWheelTime;
        protected int _toolActiveFrameCount;

        public override void OnActivated()
        {
            Instance = this;
            SetMode(Mode.PickObjects);
            if (Selection.gameObjects.Length > 0)
            {
                updateSelected();
                if (_selectedObjects.Length > 0)
                {
                    SetMode(Mode.PaintSelection);
                }
            }

            _toolActiveFrameCount = 0;

            Selection.selectionChanged -= onSelectionChanged;
            Selection.selectionChanged += onSelectionChanged;

            var sceneViewDrawer = SceneViewDrawer.Instance();
            sceneViewDrawer.OnRender -= onRenderMesh;
            sceneViewDrawer.OnRender += onRenderMesh;

            EditorApplication.hierarchyChanged -= onHierarchyChanged;
            EditorApplication.hierarchyChanged += onHierarchyChanged;

            EditorSceneManager.sceneOpened -= onSceneOpened;
            EditorSceneManager.sceneOpened += onSceneOpened;

            PrefabStage.prefabStageOpened -= onClearDueToPrefabStage;
            PrefabStage.prefabStageOpened += onClearDueToPrefabStage;

            PrefabStage.prefabStageClosing -= onClearDueToPrefabStage;
            PrefabStage.prefabStageClosing += onClearDueToPrefabStage;
        }

        void onClearDueToPrefabStage(PrefabStage obj)
        {
            clearSelection();
        }

        void onSceneOpened(Scene scene, OpenSceneMode mode)
        {
            clearSelection();
        }

        public override void OnWillBeDeactivated()
        {
            SetMode(Mode.PickObjects);
        }

        public bool IsInPaintSelectionMode()
        {
            return _mode == Mode.PaintSelection;
        }

        /// <summary>
        /// A method to activate the tool. Though remember, this is not always called.
        /// The tool is actually a ScriptableObject which is instantiated and deserialized by Unity.
        /// </summary>
        [MenuItem("Tools/Mesh Extractor/Start", priority = 1)]
        public static void Activate()
        {
#if UNITY_2020_2_OR_NEWER
            ToolManager.SetActiveTool(typeof(MeshExtractorTool));
#else
            EditorTools.SetActiveTool(typeof(MeshExtractorTool));
#endif

            SceneView.lastActiveSceneView.Focus();
        }

        public void OnToolChanged()
        {
            if (MeshExtractorToolActiveState.IsActive)
            {
                SceneView.lastActiveSceneView.Focus();

                initWindowSize();

                _mouseIsDown = false;
                _leftMouseIsDown = false;
            }
        }

        protected double _ignoreDisableOnHierarchyChangeUntilTime;

        protected void IgnoreDisableOnHierarchyForNSeconds(double ignoreDurationInSec = 0.5)
        {
            _ignoreDisableOnHierarchyChangeUntilTime = EditorApplication.timeSinceStartup + ignoreDurationInSec;
        }

        protected bool IgnoreDisableOnHierarchyChange()
        {
            return EditorApplication.timeSinceStartup < _ignoreDisableOnHierarchyChangeUntilTime;
        }

        private void onHierarchyChanged()
        {
            // For some reason the hierarch changes if a tool gets activated for the very first time. Thus we add this delay.
            if (_toolActiveFrameCount > 100 && MeshExtractorSettings.GetOrCreateSettings().DisableOnHierarchyChange && !IgnoreDisableOnHierarchyChange())
            {
                exitTool();
            }
        }

        // Will be called after all regular rendering is done
        public void onRenderMesh()
        {
            onSelectRenderMesh();
        }

        // Equivalent to Editor.OnSceneGUI.
        public override void OnToolGUI(EditorWindow window)
        {
            _toolActiveFrameCount++;

            if (!(window is SceneView sceneView))
            {
                return;
            }

            var settings = MeshExtractorSettings.GetOrCreateSettings();
            var current = Event.current;

            // Detect if the mouse returns into the scene view
            if (current.type == EventType.Repaint)
            {
                bool mouseIsInSceneView = IsMouseInSceneView();
                if (!_mouseIsInSceneView && mouseIsInSceneView)
                {
                    _mouseEnteredSceneView = true;
                }
                _mouseIsInSceneView = mouseIsInSceneView;
            }

            // handle key presses & draw handles
            int passiveControlID = GUIUtility.GetControlID(FocusType.Passive);

            // Key events
            bool keyEvent = false;
            bool useKey = false;
            bool snapCursor = false;
            _shiftPressed = current.shift;
            _controlPressed = current.control;
            if (current.type == EventType.KeyDown)
            {
                keyEvent = true;

                // undo redo
                if ((SceneViewIsActive() || IsMouseInSceneView())
                    && current.isKey && (current.control || current.command)
                    )
                {
                    if (current.keyCode == settings.UndoKey)
                    {
                        if (_undoStack != null && _undoStack.HasUndoActions())
                        {
                            _undoStack.Undo();
                        }
                        useKey = true;
                    }
                    else if (current.keyCode == settings.RedoKey)
                    {
                        if (_undoStack != null && _undoStack.HasRedoActions())
                        {
                            _undoStack.Redo();
                        }
                        useKey = true;
                    }
                }

                if (current.keyCode == KeyCode.Escape)
                {
                    useKey = true;
                    if (IsInPaintSelectionMode())
                    {
                        SetMode(Mode.PickObjects);
                    }
                    else
                    {
                        exitTool();
                    }
                }

                if (current.keyCode == KeyCode.LeftAlt)
                {
                    _altPressed = true;
                }

                if (current.keyCode == KeyCode.V && _mode == Mode.ExtractMesh && _mouseIsInSceneView)
                {
                    useKey = true;
                    snapCursor = true;
                }
            }

            if (_mode == Mode.PaintSelection && _saveLoadVisibility == 0
                && current.isKey && current.modifiers == EventModifiers.None && current.keyCode == MeshExtractorSettings.GetOrCreateSettings().TriggerSelectLinked
                && !Tools.viewToolActive)
            {
                keyEvent = true;
                useKey = true;
            }

            // Mouse events
            bool mouseEvent = false;
            if (current.type == EventType.MouseDown)
            {
                _mouseIsDown = true;
                if (current.button == 0)
                {
                    _leftMouseIsDown = true;
                    _leftMouseWasPressed = true;
                }
                mouseEvent = true;
            }
            else if (current.type == EventType.MouseUp)
            {
                _mouseIsDown = false;
                _leftMouseIsDown = false;
                _leftMouseWasReleased = true;
                mouseEvent = true;
            }
            else if (current.type == EventType.MouseDrag)
            {
                _mouseIsDown = true;
                mouseEvent = true;
                _lastMouseDragTime = EditorApplication.timeSinceStartup;
                if (current.button == 0)
                {
                    _leftMouseIsDown = true;
                }
            }
            else if (current.type == EventType.MouseMove)
            {
                // fixing mouse down
                bool timedOut = EditorApplication.timeSinceStartup - _lastMouseDragTime > 0.05f;
                if (_mouseIsDown && timedOut)
                {
                    _mouseIsDown = false;
                }
                if (_leftMouseIsDown && timedOut)
                {
                    _leftMouseIsDown = false;
                }
                mouseEvent = true;
            }

            bool scrollWheelEvent = false;
            if (current.type == EventType.ScrollWheel)
            {
                _scrollWheelTurned = true;
                _lastScrollWheelTime = EditorApplication.timeSinceStartup;
                scrollWheelEvent = true;
            }

            drawWindow(sceneView, passiveControlID + 1);

            switch (_mode)
            {
                case Mode.PickObjects:
                    break;

                case Mode.PaintSelection:
                    onSelectGUI(sceneView);
                    break;

                case Mode.ExtractMesh:
                    onPivotCursorGUI(sceneView, snapCursor);
                    break;

                default:
                    break;
            }

            // Restore selection after the mouse entered the scene view again.
            // And also do not auto select if the settings have been opened recently.
            if (_mouseEnteredSceneView && _selectedObjects.Length > 0
                && EditorApplication.timeSinceStartup - _lastSettingsOpenTimestamp > 2)
            {
                restoreSelected();
            }

            if (mouseEvent
                // Don't consume mouse events if a view tool is active.
                && !Tools.viewToolActive && !current.alt
                // Consume only left mouse button down or drag events.
                && current.button == 0
                && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
                )
            {
                if (IsInPaintSelectionMode() && IsMouseInSceneView()
                    // Do not consume if CTRL is pressed AND no triangles are hovered.
                    && !(current.control && _hoverTriangles.Count == 0)
                    )
                {
                    GUIUtility.hotControl = passiveControlID;
                    Event.current.Use();
                }
            }

            // Consume scroll wheel events.
            if (scrollWheelEvent)
            {
                if (selectConsumesScrollWheelEvent(current))
                {
                    // Do not set the hot control to inactive if teh scroll whell was used (otherwise it would prohibit key press detection afterwards).
                    Event.current.Use();
                }
            }
            else if (keyEvent && useKey)
            {
                Event.current.Use();
            }

            resetFlags();

            // Reset mode to object selection if no object is selected.
            if (_mode == Mode.PaintSelection && _selectedObjects.Length == 0)
            {
                SetMode(Mode.PickObjects);
            }

            if (_mode == Mode.ExtractMesh)
            {
                if (_selectedObjects.Length == 0)
                {
                    SetMode(Mode.PickObjects);
                }
                else if (_selectedTriangles.Count == 0)
                {
                    SetMode(Mode.PaintSelection);
                }
            }
        }

        void updateSelected()
        {
            List<GameObject> selectedObjectsInScene = getValidSelection();

            if (selectedObjectsInScene.Count > 0)
            {
                _selectedObjects = selectedObjectsInScene.ToArray();
                Selection.objects = _selectedObjects;

                // Show position warning (selection may be inaccurate if object is too far from origin)
                if (_selectedObjects.Length > 0 && _selectedObjects[0].transform.position.magnitude > 500 && MeshExtractorSettings.GetOrCreateSettings().ShowSelectedObjectPositionWarning)
                {
                    Logger.LogWarning("The selected object is pretty far from the world origin. " +
                        "Selection may be inaccurate. If possible please move the object closer to the origin position of 0/0/0. " +
                        "You can disable this warning in the settings.");
                }

                // Select bone export depending on whether first renderer has bones.
                if (_selectedObjects.Length > 0)
                {
                    var skinnedMesh = _selectedObjects[0].GetComponentInChildren<SkinnedMeshRenderer>();
                    if (skinnedMesh == null)
                    {
                        _extractBoneWeights = false;
                    }
                    else
                    {
                        if (skinnedMesh.bones != null && skinnedMesh.bones.Length > 0 && skinnedMesh.bones[0] != null)
                        {
                            _extractBoneWeights = true;
                        }
                        _cursorPosition = _selectedObjects[0].transform.position;
                    }
                }
            }

            resetToPickObjectModeIfNecessary();
        }

        void restoreSelected()
        {
            List<GameObject> selectedObjectsInScene = getValidSelection();

            if (selectedObjectsInScene.Count == 0)
            {
                var newSelectedObjects = new List<GameObject>(_selectedObjects);

                // Add the object from the selected triangles
                foreach (var tri in _selectedTriangles)
                {
                    if(tri.Component == null)
                    {
                        continue;
                    }

                    if (!selectedObjectsInScene.Contains(tri.Component.gameObject))
                    {
                        bool isChild = false;
                        foreach (var selectedObj in _selectedObjects)
                        {
                            if (tri.Component.transform.IsChildOf(selectedObj.transform))
                            {
                                isChild = true;
                                break;
                            }
                        }

                        if (!isChild && UtilsEditor.IsInScene(tri.Component.gameObject))
                        {
                            newSelectedObjects.Add(tri.Component.gameObject);
                        }
                    }
                }

                Selection.objects = newSelectedObjects.ToArray();
            }

            resetToPickObjectModeIfNecessary();
        }

        void resetToPickObjectModeIfNecessary()
        {
            if (_mode != Mode.PickObjects && _selectedObjects.Length == 0)
            {
                SetMode(Mode.PickObjects);
            }
        }

        List<GameObject> getValidSelection()
        {
            var selectedObjects = Selection.gameObjects;

            // Put all the objects which are in the scene view into a list (ignore the others).
            List<GameObject> selectedObjectsInScene = new List<GameObject>();
            if (selectedObjects.Length > 0)
            {
                for (int i = 0; i < selectedObjects.Length; i++)
                {
                    if (UtilsEditor.IsInScene(selectedObjects[i].gameObject))
                    {
                        // Add only if the object has a valid mesh on it.
                        var meshRenderer = selectedObjects[i].gameObject.GetComponentInChildren<MeshRenderer>();
                        var meshFilter = selectedObjects[i].gameObject.GetComponentInChildren<MeshFilter>();
                        var skinnedRenderer = selectedObjects[i].gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                        if (meshRenderer != null || meshFilter != null || skinnedRenderer != null)
                        {
                            selectedObjectsInScene.Add(selectedObjects[i]);
                        }
                    }
                }
            }

            return selectedObjectsInScene;
        }

        private void resetFlags()
        {
            _selectionChanged = false;
            _leftMouseWasPressed = false;
            _leftMouseWasReleased = false;
            _scrollWheelTurned = false;
            _altPressed = false;
            _mouseEnteredSceneView = false;
        }

        public void SetMode(Mode mode)
        {
            if (_mode == mode)
                return;

            switch (mode)
            {
                case Mode.PickObjects:
                    DefragSelectedObjects();
                    DefragTriangles();
                    break;

                case Mode.PaintSelection:

                    DefragSelectedObjects();
                    DefragTriangles();

                    // Refresh cache
                    if (_selectedTriangles.Count == 0)
                    {
                        TriangleCache.CacheTriangles(SceneView.lastActiveSceneView.camera, _selectedObjects.Length == 0 ? null : _selectedObjects);
                        TriangleCache.RebuildBakedMeshesCache(_selectedObjects);
                    }

                    warnAboutOldSelection();

                    if (didCameraChange(SceneView.lastActiveSceneView.camera))
                    {
                        TriangleCache.CacheTriangles(SceneView.lastActiveSceneView.camera, _selectedObjects);
                        // Make sure the cached world space is in sync.
                        updateSelectedTrianglesAfterCacheChange();
                    }
                    break;

                case Mode.ExtractMesh:
                    DefragTriangles();
                    if (_newMeshName == "Mesh" && _selectedObjects.Length > 0)
                    {
                        _newMeshName = _selectedObjects[0].name;
                    }
                    
                    // Auto center pivot if not edited and if bone export is disabled.
                    if (!_pivotModified)
                    {
                        if (_pivotBehaviour == PivotBehaviour.Center)
                        {
                            CenterPivot();
                        }
                        else
                        {
                            ResetPivotToOrigin();
                        }
                    }
                    break;

                default:
                    break;
            }

            _mode = mode;
        }

        private void warnAboutOldSelection()
        {
            // Check if there are selected triangles in object which are currently not selected.
            if (_mode == Mode.PickObjects && _selectedTriangles.Count > 0)
            {
                bool foundOldSelection = false;
                var trisToRemove = new List<SelectedTriangle>();
                foreach (var tri in _selectedTriangles)
                {
                    bool foundInSelection = false;
                    foreach (var obj in _selectedObjects)
                    {
                        if (obj.transform != null && tri.Transform != null && tri.Transform.IsChildOf(obj.transform))
                        {
                            foundInSelection = true;
                            break;
                        }
                    }
                    if (!foundInSelection)
                    {
                        foundOldSelection = true;
                        break;
                    }
                }
                if (foundOldSelection)
                {
                    if (MeshExtractorSettings.GetOrCreateSettings().ClearOldSelectionsAutomatically)
                    {
                        removeOldTris();
                    }
                    else if (MeshExtractorSettings.GetOrCreateSettings().WarnAboutOldSelections)
                    {
                        EditorApplication.delayCall += () =>
                        {
                            bool clear = EditorUtility.DisplayDialog(
                                "There already are selected triangles!",
                                "There are some selected triangles on another object. Would you like to clear those before you start a new selection?",
                                "Yes (clear old selection)", "No (keep selection)"
                            );

                            if (clear)
                            {
                                removeOldTris();
                            }
                        };
                    }
                }

            }
        }

        private void removeOldTris()
        {
            List<SelectedTriangle> trisToRemove = new List<SelectedTriangle>();
            foreach (var tri in _selectedTriangles)
            {
                bool foundInSelection = false;
                foreach (var obj in _selectedObjects)
                {
                    if (tri.Transform.IsChildOf(obj.transform))
                    {
                        foundInSelection = true;
                        break;
                    }
                }
                if (!foundInSelection)
                {
                    trisToRemove.Add(tri);
                }
            }
            foreach (var tri in trisToRemove)
            {
                _selectedTriangles.Remove(tri);
            }
        }

        public static Camera GetValidSceneViewCamera()
        {
            var cam = SceneView.lastActiveSceneView.camera;
            if (cam != null && cam.transform.position != Vector3.zero)
            {
                return cam;
            }

            return null;
        }


        public static bool SceneViewIsActive()
        {
            return EditorWindow.focusedWindow == SceneView.lastActiveSceneView;
        }

        public static bool IsMouseInSceneView()
        {
            return EditorWindow.mouseOverWindow != null && SceneView.sceneViews.Contains(EditorWindow.mouseOverWindow);
        }

        void exitTool()
        {
            Tools.current = Tool.Move;
            _selectedObjects = new GameObject[] { };

            Selection.selectionChanged -= onSelectionChanged;
            SceneViewDrawer.Instance().OnRender -= onRenderMesh;
            EditorApplication.hierarchyChanged -= onHierarchyChanged;
        }

        void onSelectionChanged()
        {
            _selectionChanged = true;

            updateSelected();

            // We do this to trigger a cache renewal after the selectin changed.
            resetCameraMemory();
        }
    }
}
