using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject _restartScreenObject;
    [SerializeField] private SlingShotController _slingshotController;
    [SerializeField] private Image _nextLevelImage;

    private int _usedNumberOfShots;

    private IconController _iconController;

    private List<PiggieController> _piggies = new List<PiggieController>();

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }

        _iconController = FindObjectOfType<IconController>();

        PiggieController[] piggies = FindObjectsOfType<PiggieController>();
        foreach(PiggieController pig in piggies){
            _piggies.Add(pig);
        }

        _nextLevelImage.enabled = false;
    }

    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconController.UseShot(_usedNumberOfShots);

        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        return _usedNumberOfShots < MaxNumberOfShots;
    }

    public void CheckForLastShot()
    {
        if(_usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);

        if(_piggies.Count == 0)
        {
            WinGame();
        }
        else 
        {
            RestartGame();
        }
        
    }

    public void RemovePiggie(PiggieController pig)
    {
        _piggies.Remove(pig);
        CheckForAllDeadPiggies();
    }

    private void CheckForAllDeadPiggies(){
        if(_piggies.Count == 0)
        {
            WinGame();
        }
    }

    #region Win/Lose
    private void WinGame()
    {
        _restartScreenObject.SetActive(true);
        _slingshotController.enabled = false;


        // do we have any more levels to load?
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;
        if(currentSceneIndex + 1 < maxLevels)
        {
            _nextLevelImage.enabled = true;
        }
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion
}
