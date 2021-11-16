using System;
using System.Collections.Generic;
using EW2;
using EW2.Tutorial.General;
using TigerForge;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zitga.UIFramework
{
    /// <summary>
    /// This is the centralized access point for all things UI.
    /// All your calls should be directed at this.
    /// </summary>
    public class UIFrame : Singleton<UIFrame>
    {
        [Tooltip("Set this to false if you want to manually initialize this UI Frame.")] [SerializeField]
        private bool initializeOnAwake = true;

        private PanelUILayer _panelLayer;
        private WindowUILayer _windowLayer;

        private Canvas _mainCanvas;
        private Camera _uiCamera;
        private GraphicRaycaster _graphicRaycaster;
        private CanvasScaler _mainCanvasScaler;
        private EventSystem _eventSystem;
        private bool _canKeyBack = true;

        /// <summary>
        /// The main canvas of this UI
        /// </summary>
        public Canvas MainCanvas
        {
            get
            {
                if (this._mainCanvas == null)
                {
                    this._mainCanvas = GetComponent<Canvas>();
                }

                return this._mainCanvas;
            }
        }

        /// <summary>
        /// The main canvas scaler of this UI
        /// </summary>
        public CanvasScaler MainCanvasScaler
        {
            get
            {
                if (this._mainCanvasScaler == null)
                {
                    this._mainCanvasScaler = GetComponent<CanvasScaler>();
                }

                return this._mainCanvasScaler;
            }
        }

        /// <summary>
        /// The Camera being used by the Main UI Canvas
        /// </summary>
        public Camera UICamera
        {
            get
            {
                if (this._uiCamera == null)
                    this._uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

                return this._uiCamera;
            }
        }


        private void Awake()
        {
            if (initializeOnAwake)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Initializes this UI Frame. Initialization consists of initializing both the Panel and Window layers.
        /// Although literally all the cases I've had to this day were covered by the "Window and Panel" approach,
        /// I made it virtual in case you ever need additional layers or other special initialization.
        /// </summary>
        protected virtual void Initialize()
        {
            if (this._panelLayer == null)
            {
                this._panelLayer = gameObject.GetComponentInChildren<PanelUILayer>(true);
                if (this._panelLayer == null)
                {
                    Debug.LogError("[UI Frame] UI Frame lacks Panel Layer!");
                }
                else
                {
                    this._panelLayer.Initialize();
                }
            }

            if (this._windowLayer == null)
            {
                this._windowLayer = gameObject.GetComponentInChildren<WindowUILayer>(true);
                if (this._panelLayer == null)
                {
                    Debug.LogError("[UI Frame] UI Frame lacks Window Layer!");
                }
                else
                {
                    this._windowLayer.Initialize();
                    this._windowLayer.RequestScreenBlock += OnRequestScreenBlock;
                    this._windowLayer.RequestScreenUnblock += OnRequestScreenUnblock;
                }
            }

            this._graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();

            this._eventSystem = GetComponentInChildren<EventSystem>();

            FixCanvasMatch();
        }

        private void FixCanvasMatch()
        {
            var referenceResolution = MainCanvasScaler.referenceResolution;

            var screenRatio = Screen.width * referenceResolution.y < Screen.height * referenceResolution.x;

            MainCanvasScaler.matchWidthOrHeight = screenRatio ? 0 : 1;
        }

        public void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            this._graphicRaycaster.Raycast(eventData, resultAppendList);
        }

        public bool IsHome()
        {
            if (this._windowLayer.CurrentWindow == null) return false;

            return this._windowLayer.CurrentWindow.GetType() == typeof(HomeWindowController);
        }

        /// <summary>
        /// Shows a panel by its id, passing no Properties.
        /// </summary>
        /// <param name="screenId">Panel Id</param>
        public void ShowPanel(string screenId)
        {
            if (!this._panelLayer.IsScreenRegistered(screenId))
            {
                AutoRegisterScreen(screenId);
            }

            this._panelLayer.ShowScreenById(screenId);
        }

        /// <summary>
        /// Shows a panel by its id, passing parameters.
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        /// <param name="properties">Properties.</param>
        /// <typeparam name="T">The type of properties to be passed in.</typeparam>
        /// <seealso cref="IPanelProperties"/>
        public void ShowPanel<T>(string screenId, T properties) where T : IPanelProperties
        {
            if (!this._panelLayer.IsScreenRegistered(screenId))
            {
                AutoRegisterScreen(screenId);
            }

            this._panelLayer.ShowScreenById<T>(screenId, properties);
        }

        /// <summary>
        /// Hides the panel with the given id.
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        public void HidePanel(string screenId, bool animated = true)
        {
            this._panelLayer.HideScreenById(screenId, animated);
        }

        /// <summary>
        /// Opens the Window with the given Id, with no Properties.
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        public void OpenWindow(string screenId)
        {
            if (!this._windowLayer.IsScreenRegistered(screenId))
            {
                AutoRegisterScreen(screenId);
            }

            this._windowLayer.ShowScreenById(screenId);
        }

        /// <summary>
        /// Closes the Window with the given Id.
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        public void CloseWindow(string screenId, bool animated = true)
        {
            Debug.Log("Close window: " + screenId);
            this._windowLayer.HideScreenById(screenId, animated);
        }

        /// <summary>
        /// Closes the currently open window, if any is open
        /// </summary>
        public void CloseCurrentWindow()
        {
            if (this._windowLayer.CurrentWindow != null)
            {
                CloseWindow(this._windowLayer.CurrentWindow.ScreenId);
            }
        }

        public void ClickBgWindow()
        {
            if (this._windowLayer.CurrentWindow != null && this._windowLayer.CurrentWindow.IsBgClose)
            {
                CloseWindow(this._windowLayer.CurrentWindow.ScreenId);
            }
        }

        /// <summary>
        /// Close current panel
        /// </summary>
        public void CloseCurrentPanel(bool animated)
        {
            if (this._panelLayer.CurrentPanel != null)
            {
                HidePanel(this._panelLayer.CurrentPanel.ScreenId, animated);
            }
        }

        /// <summary>
        /// Opens the Window with the given id, passing in Properties.
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        /// <param name="properties">Properties.</param>
        /// <typeparam name="T">The type of properties to be passed in.</typeparam>
        /// <seealso cref="IWindowProperties"/>
        public void OpenWindow<T>(string screenId, T properties) where T : IWindowProperties
        {
            if (!this._windowLayer.IsScreenRegistered(screenId))
            {
                AutoRegisterScreen(screenId);
            }

            this._windowLayer.ShowScreenById(screenId, properties);
        }

        /// <summary>
        /// Searches for the given id among the Layers, opens the Screen if it finds it
        /// </summary>
        /// <param name="screenId">The Screen id.</param>
        public void ShowScreen(string screenId)
        {
            Type type;
            if (IsScreenRegistered(screenId, out type))
            {
                if (type == typeof(IWindowController))
                {
                    OpenWindow(screenId);
                }
                else if (type == typeof(IPanelController))
                {
                    ShowPanel(screenId);
                }
            }
            else
            {
                Debug.LogError(string.Format("Tried to open Screen id {0} but it's not registered as Window or Panel!",
                    screenId));
            }
        }

        private void AutoRegisterScreen(string screenId)
        {
            var screenInstance = Instantiate(Resources.Load<GameObject>(screenId));
            var screenController = screenInstance.GetComponent<IUIScreenController>();

            if (screenController != null)
            {
                RegisterScreen(screenId, screenController, screenInstance.transform);
                screenInstance.SetActive(false);
            }
            else
            {
                Debug.LogError("[UIConfig] Screen doesn't contain a ScreenController! Skipping " + screenId);
            }
        }

        /// <summary>
        /// Registers a screen. If transform is passed, the layer will
        /// reparent it to itself. Screens can only be shown after they're registered.
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <param name="screenTransform">Screen transform. If not null, will be reparented to proper layer</param>
        public void RegisterScreen(string screenId, IUIScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window != null)
            {
                this._windowLayer.RegisterScreen(screenId, window);
                if (screenTransform != null)
                {
                    this._windowLayer.ReparentScreen(controller, screenTransform);
                }

                return;
            }

            IPanelController panel = controller as IPanelController;
            if (panel != null)
            {
                this._panelLayer.RegisterScreen(screenId, panel);
                if (screenTransform != null)
                {
                    this._panelLayer.ReparentScreen(controller, screenTransform);
                }
            }
        }

        /// <summary>
        /// Registers the panel. Panels can only be shown after they're registered.
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TPanel">The Controller type.</typeparam>
        public void RegisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IPanelController
        {
            this._panelLayer.RegisterScreen(screenId, controller);
        }

        /// <summary>
        /// Unregisters the panel.
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TPanel">The Controller type.</typeparam>
        public void UnregisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IPanelController
        {
            this._panelLayer.UnregisterScreen(screenId, controller);
        }

        /// <summary>
        /// Registers the Window. Windows can only be opened after they're registered.
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TWindow">The Controller type.</typeparam>
        public void RegisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            this._windowLayer.RegisterScreen(screenId, controller);
        }

        /// <summary>
        /// Unregisters the Window.
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TWindow">The Controller type.</typeparam>
        public void UnregisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            this._windowLayer.UnregisterScreen(screenId, controller);
        }

        /// <summary>
        /// Checks if a given Panel is open.
        /// </summary>
        /// <param name="panelId">Panel identifier.</param>
        public bool IsPanelOpen(string panelId)
        {
            return this._panelLayer.IsPanelVisible(panelId);
        }

        /// <summary>
        /// Hide all screens
        /// </summary>
        /// <param name="animate">Defines if screens should the screens animate out or not.</param>
        public void HideAll(bool animate = true)
        {
            CloseAllWindows(animate);
            HideAllPanels(animate);
        }

        /// <summary>
        /// Hide all screens on the Panel Layer
        /// </summary>
        /// <param name="animate">Defines if screens should the screens animate out or not.</param>
        public void HideAllPanels(bool animate = true)
        {
            this._panelLayer.HideAll(animate);
        }

        public void HideAllPriorityLayer()
        {
            this._windowLayer.HideAllPriorityLayer();
        }

        /// <summary>
        /// Hide all screens in the Window Layer
        /// </summary>
        /// <param name="animate">Defines if screens should the screens animate out or not.</param>
        public void CloseAllWindows(bool animate = true)
        {
            this._windowLayer.HideAll(animate);
        }

        public void CloseAllPopup(bool animate = true)
        {
            this._windowLayer.HideAllPopup(animate);
        }

        /// <summary>
        /// Checks if a given screen id is registered to either the Window or Panel layers
        /// </summary>
        /// <param name="screenId">The Id to check.</param>
        public bool IsScreenRegistered(string screenId)
        {
            if (this._windowLayer.IsScreenRegistered(screenId))
            {
                return true;
            }

            if (this._panelLayer.IsScreenRegistered(screenId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a given screen id is registered to either the Window or Panel layers,
        /// also returning the screen type
        /// </summary>
        /// <param name="screenId">The Id to check.</param>
        /// <param name="type">The type of the screen.</param>
        public bool IsScreenRegistered(string screenId, out Type type)
        {
            if (this._windowLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IWindowController);
                return true;
            }

            if (this._panelLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IPanelController);
                return true;
            }

            type = null;
            return false;
        }

        private void OnRequestScreenBlock()
        {
            if (this._graphicRaycaster != null)
            {
                this._graphicRaycaster.enabled = false;
            }
        }

        private void OnRequestScreenUnblock()
        {
            if (this._graphicRaycaster != null)
            {
                this._graphicRaycaster.enabled = true;
            }
        }

        public void SwitchCamera(bool isWorldCamera)
        {
            if (isWorldCamera)
            {
                MainCanvas.worldCamera = Camera.main;

                UICamera.gameObject.SetActive(false);
            }
            else
            {
                UICamera.gameObject.SetActive(true);

                MainCanvas.worldCamera = UICamera;
            }
        }

        public void SetEventSystem(bool isEnable)
        {
            this._eventSystem.enabled = isEnable;
        }

        public string GetCurrentScreenId()
        {
            if (this._windowLayer.CurrentWindow != null) return this._windowLayer.CurrentWindow.ScreenId;
            return null;
        }
        
        private void OnApplicationQuit()
        {
            EventManager.EmitEvent(GamePlayEvent.OnApplicationQuit);
        }

        public void EnableCanKeyBack(bool isActive)
        {
            this._canKeyBack = isActive;
        }

        public IUIScreenController FindWindow(string screenId)
        {
            return this._windowLayer.FindWindow(screenId);
        }
#if UNITY_EDITOR || UNITY_ANDROID
        private void Update()
        {
            if (Application.isPlaying && Input.GetKeyDown(KeyCode.Escape))
            {
                // Debug.LogWarning("Back");
                var currWindow = this._windowLayer.CurrentWindow;

                if (!this._canKeyBack) return;
                
                if (currWindow != null)
                {
                    if (currWindow.ScreenId == ScreenIds.loading || currWindow.CanNotKeyBack) return;

                    if (currWindow.ScreenId == ScreenIds.home || currWindow.ScreenId == ScreenIds.tap_to_start)
                    {
                        Instance.OpenWindow(ScreenIds.popup_quit_game, new QuitGameWindowProperty());
                    }
                    else if (currWindow.ScreenId == ScreenIds.game_play ||
                             currWindow.ScreenId == ScreenIds.game_play_hero_defense)
                    {
                        if (UserData.Instance.AccountData.CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_0))
                        {
                            GamePlayUIManager.Instance.CloseCurrentUI(true);

                            GamePlayUIManager.Instance.CloseAllUI();

                            GamePlayController.Instance.IsPause = true;

                            Time.timeScale = 0;

                            Instance.OpenWindow(ScreenIds.game_pause);
                        }
                        else
                        {
                            Instance.OpenWindow(ScreenIds.popup_quit_game, new QuitGameWindowProperty(true));
                        }
                    }
                    else
                    {
                        Instance.CloseWindow(currWindow.ScreenId);
                    }
                }
                else
                {
                    var isTutorial =
                        !UserData.Instance.AccountData.CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_0);

                    Instance.OpenWindow(ScreenIds.popup_quit_game, new QuitGameWindowProperty(isTutorial));
                }
            }
        }
#endif
    }
}