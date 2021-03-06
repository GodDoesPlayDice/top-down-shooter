using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseScreen : MonoBehaviour
{

    public UIDocument pauseDoc;
    public ProgressionHolder progressionHolder;
    public ScoreCounter scoreCounter;

    private VisualElement rootEl;
    private Button resumeEl;
    private Button settingsEl;
    private Button newRunEl;
    private Button mainMenuEl;

    private Action settingsAction;
    private Action gameAction;
    private Action newRunAction;
    private Action mainMenuAction;

    private Label scoreLabel;
    private Label topScoreLabel;

    void OnEnable()
    {
        rootEl = pauseDoc.rootVisualElement;
        resumeEl = rootEl.Q<Button>("Resume");
        settingsEl = rootEl.Q<Button>("Settings");
        newRunEl = rootEl.Q<Button>("NewRun");
        mainMenuEl = rootEl.Q<Button>("MainMenu");

        resumeEl.RegisterCallback<ClickEvent>(e => gameAction?.Invoke());
        settingsEl.RegisterCallback<ClickEvent>(e => settingsAction?.Invoke());
        newRunEl.RegisterCallback<ClickEvent>(e => newRunAction?.Invoke());
        mainMenuEl.RegisterCallback<ClickEvent>(e => mainMenuAction?.Invoke());

        scoreLabel = rootEl.Q<Label>("ScoreLabel");
        topScoreLabel = rootEl.Q<Label>("TopScoreLabel");
        SetScore();
    }

    public void SetSettingsAction(Action settingsAction)
    {
        this.settingsAction = settingsAction;
    }

    public void SetGameAction(Action gameAction)
    {
        this.gameAction = gameAction;
    }

    public void SetNewRunAction(Action newRunAction)
    {
        this.newRunAction = newRunAction;
    }

    public void SetMainMenuAction(Action mainMenuAction)
    {
        this.mainMenuAction = mainMenuAction;
    }

    private void SetScore()
    {
        scoreLabel.text = scoreCounter.currentScore.ToString();
        if (scoreCounter.currentScore > progressionHolder.topScore)
        {
            progressionHolder.topScore = scoreCounter.currentScore;
        }
        topScoreLabel.text = progressionHolder.topScore.ToString();
    }
}
