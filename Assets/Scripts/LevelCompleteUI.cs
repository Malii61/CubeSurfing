using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI calculateText;
    [SerializeField] TextMeshProUGUI goldRewardText;
    private void Start()
    {
        GameManager.Instance.OnLevelCompleted += GameManager_OnLevelCompleted;
        gameObject.SetActive(false);
    }
    private void GameManager_OnLevelCompleted(object sender, System.EventArgs e)
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        calculateText.text = "(" + InGameCoinManager.Instance.GetCoinAmount() + " x " + CollectorCube.Instance.GetGoldMultiplier() + ")";
        int reward = InGameCoinManager.Instance.GetCoinAmount() * CollectorCube.Instance.GetGoldMultiplier();
        goldRewardText.text = "+" + reward;
        CloudSave.Save("coin", reward + GeneralCoinManager.Instance.GetCoin());
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnLevelCompleted -= GameManager_OnLevelCompleted;
    }
    public void OnClick_NextButton()
    {
        Time.timeScale = 1;
        if (!IsThisLastScene())
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(1);
    }

    private bool IsThisLastScene()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings)
            return true;
        return false;
    }

    public void OnClick_BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}
