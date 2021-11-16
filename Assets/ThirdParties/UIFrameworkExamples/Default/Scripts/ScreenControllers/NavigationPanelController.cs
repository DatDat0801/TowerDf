using System;
using System.Collections.Generic;
using Zitga.UIFramework;
using Zitga.UIFramework.Examples;
using Zitga.Utils;
using UnityEngine;

[Serializable] 
public class NavigationPanelEntry
{
    [SerializeField] private Sprite sprite = null;
    [SerializeField] private string buttonText = "";
    [SerializeField] private string targetScreen = "";
    
    public Sprite Sprite => sprite;

    public string ButtonText => buttonText;

    public string TargetScreen => targetScreen;
}

public class NavigateToWindowSignal : ASignal<string> { }
public class NavigationPanelController : APanelController
{
    [SerializeField] 
    private List<NavigationPanelEntry> navigationTargets = new List<NavigationPanelEntry>();
    [SerializeField] 
    private NavigationPanelButton templateButton = null;
    
    private readonly List<NavigationPanelButton> currentButtons = new List<NavigationPanelButton>();

    // I usually always place AddListeners and RemoveListeners together
    // to reduce the chances of adding a listener and not removing it.
    protected override void AddListeners() {
        Signals.Get<NavigateToWindowSignal>().AddListener(OnExternalNavigation);
    }

    protected override void RemoveListeners() {
        Signals.Get<NavigateToWindowSignal>().RemoveListener(OnExternalNavigation);
    }

    /// <summary>
    /// This is called whenever this screen is opened
    /// be it for the first time or coming from the history/queue
    /// </summary>
    protected override void OnPropertiesSet() {
        ClearEntries();
        foreach (var target in navigationTargets) {
            var newBtn = Instantiate(templateButton, templateButton.transform.parent, false);
            // When using UI, never forget to pass the parameter
            // worldPositionStays as FALSE, otherwise your RectTransform
            // won't layout properly after reparenting.
            // This is the cause for the most common head-scratching issues
            // when starting to deal with Unity UI: adding objects via the editor
            // working fine but objects instanced via code having broken sizes/positions
            newBtn.SetData(target);
            newBtn.gameObject.SetActive(true);
            newBtn.ButtonClicked += OnNavigationButtonClicked;
            currentButtons.Add(newBtn);
        }
        
        // The first button is selected by default
        OnNavigationButtonClicked(currentButtons[0]);
    }

    private void OnNavigationButtonClicked(NavigationPanelButton currentlyClickedButton) {
        // Signals.Get<NavigateToWindowSignal>().Dispatch(currentlyClickedButton.Target);
        
        OnNavigateToWindow(currentlyClickedButton.Target);
        
        foreach (var button in currentButtons) {
            button.SetCurrentNavigationTarget(currentlyClickedButton);
        }
    }

    private FakePlayerData fakePlayerData;

    private Transform hero;
    private void OnNavigateToWindow(string windowId) {
        // You usually don't have to do this as the system takes care of everything
        // automatically, but since we're dealing with navigation and the Window layer
        // has a history stack, this way we can make sure we're not just adding
        // entries to the stack indefinitely
        UIFrame.Instance.CloseCurrentWindow();

        switch (windowId) {
            case ScreenIds.PlayerWindow:
                if (fakePlayerData == null)
                {
                    fakePlayerData = Resources.Load<FakePlayerData>("PlayerData");
                }

                UIFrame.Instance.OpenWindow(windowId, new PlayerWindowProperties(fakePlayerData.LevelProgress));
                
                break;
            case ScreenIds.CameraProjectionWindow:
                if (hero == null)
                {
                    hero = Instantiate(Resources.Load<Transform>("Prefabs/GamePlay/HeroCapsule"));
                }
                hero.gameObject.SetActive(true);
                
                UIFrame.Instance.OpenWindow(windowId, new CameraProjectionWindowProperties(Camera.main, hero.Find("EyePosition")));
                
                break;
            default:
                
                UIFrame.Instance.OpenWindow(windowId);
                break;
        }
    }

    private void OnExternalNavigation(string screenId) {
        foreach (var button in currentButtons) {
            button.SetCurrentNavigationTarget(screenId);
        }
    }
    
    private void ClearEntries() {
        foreach (var button in currentButtons) {
            button.ButtonClicked -= OnNavigationButtonClicked;
            Destroy(button.gameObject);
        }
    }
}
