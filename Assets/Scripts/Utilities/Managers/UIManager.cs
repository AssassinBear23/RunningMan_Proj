using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the UI elements in the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// The pages (panels) managed by this UI Manager.
    /// </summary>
    [Header("Page Management")]
    [Tooltip("The pages (panels) managed by this UI Manager")]
    public List<UIPage> pages;

    /// <summary>
    /// The index of the currently active page/panel.
    /// </summary>
    [Tooltip("The index of the currently active page/panel")]
    public int activePageIndex = 0;

    /// <summary>
    /// The default page that is active when UI Manager starts up.
    /// </summary>
    [Tooltip("The default page that is active when UI Manager starts up")]
    public int defaultPageIndex = 0;

    /// <summary>
    /// The pause menu page's index.
    /// </summary>
    [Header("Pause Settings")]
    [Tooltip("The pause menu page's index\n defaults to 1")]
    public int pausePageIndex = 1;

    /// <summary>
    /// Whether or not the game can be paused.
    /// </summary>
    [Tooltip("Whether or not the game can be paused")]
    public bool allowPause = true;

    // Whether or not the game is paused
    private bool isPaused = false;

    // A list of all UIElements classes
    private List<UIElements> UIElements;

    // The event system that manages UI navigation
    [HideInInspector] public EventSystem eventSystem;
    // The input manager to list for pausing
    [SerializeField] InputManager inputManager;

    // =========================================== SETUP METHODS ===============================================

    /// <summary>
    /// Default Unity Method called when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        SetupUIManager();
    }

    /// <summary>
    /// Default Unity Method called when the script is first loaded.
    /// </summary>
    private void Start()
    {
        SetupInputManager();
        SetupEventSystem();
        UpdateElements();
    }

    /// <summary>
    /// Method to get all UI elements in the scene.
    /// </summary>
    void GetUIElements()
    {
        UIElements = FindObjectsOfType<UIElements>().ToList();
        //Debug.Log("Amount of UI Elements:\t" + UIElements.Count);
        //for(int i = 0; i < UIElements.Count; i++)
        //{
        //    Debug.Log("Element " + i + ":\t" + UIElements[i].name);
        //}
    }

    /// <summary>
    /// Updates all UI elements in the <see cref="UIElements"/> list.
    /// </summary>
    public void UpdateElements()
    {
        GetUIElements();
        foreach (UIElements element in UIElements)
        {
            element.UpdateElement();
        }
#if UNITY_EDITOR
        GameManager.instance.UpdateDebug();
#endif
    }

    /// <summary>
    /// Sets up the inputManager singleton reference.
    /// </summary>
    private void SetupInputManager()
    {
        if (inputManager == null)
        {
            inputManager = InputManager.instance;
        }
        if (inputManager == null)
        {
            Debug.LogWarning($"There is no {nameof(inputManager)} in the scene. Make sure to add one to the scene otherwise you cannot pause the game");
        }
    }

    /// <summary>
    /// Sets the <see cref="eventSystem"/> variable to the EventSystem in the scene.
    /// </summary>
    private void SetupEventSystem()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogWarning($"There is no {nameof(eventSystem)} in the scene. Make sure to add one to the scene");
        }
    }

    /// <summary>
    /// Sets up the UIManager singleton instance in <see cref="GameManager.uIManager"/>.
    /// </summary>
    void SetupUIManager()
    {
        if (GameManager.instance.uiManager == null && GameManager.instance != null)
        {
            GameManager.instance.uiManager = this;
        }
    }

    //=========================================== FUNCTIONAL METHODS ===============================================

    /// <summary>
    /// Default Unity Method that is called every frame.
    /// </summary>
    private void Update()
    {
        CheckPauseInput();
    }

    /// <summary>
    /// Checks for pause input.
    /// </summary>
    private void CheckPauseInput()
    {
        if (inputManager == null)
        {
            return;
        }
        if (inputManager.pausePressed)
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Toggles the pause state of the game.
    /// </summary>
    public void TogglePause()
    {
        if (!allowPause)
        {
            return;
        }
        if (isPaused)
        {
            SetActiveAllPages(false);
            Time.timeScale = 1;
            isPaused = false;
        }
        else
        {
            GoToPage(pausePageIndex);
            Time.timeScale = 0;
            isPaused = true;
        }
    }

    /// <summary>
    /// Goes to a page by that page's index.
    /// </summary>
    /// <param name="pageIndex">The index in the page list to go to</param>
    public void GoToPage(int pageIndex)
    {
        if (pageIndex < pages.Count && pages[pageIndex] != null)
        {
            SetActiveAllPages(false);
            pages[pageIndex].gameObject.SetActive(true);
            pages[pageIndex].SetSelectedUIToDefault();
        }
    }

    /// <summary>
    /// Turns all stored pages on or off depending on the passed parameter.
    /// </summary>
    /// <param name="activeState">The state to set all pages to, true to active them all, false to deactivate them all</param>
    private void SetActiveAllPages(bool activeState)
    {
        if (pages == null)
        {
            return;
        }
        foreach (UIPage page in pages)
        {
            if (page != null)
            {
                page.gameObject.SetActive(activeState);
            }
        }
    }
}
