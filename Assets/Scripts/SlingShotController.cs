using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotController : MonoBehaviour
{
    // https://www.youtube.com/watch?v=QplEeEAJxck&t=6019s
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("SlingShot Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 9f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;
    [SerializeField] private float _maxAnimationTime = 1f;

    [Header("Scripts")]
    [SerializeField] private SlingShotCollider _slingShotCollider;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Bird")]
    [SerializeField] private AngryBirdController _angryBirdPrefab;
    [SerializeField] private float _angryBirdPositionOffset = 0.275f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip[] _elasticReleasedClips;


    private Vector2 _slingShotLinesPosition;

    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithinArea;
    private bool _isBirdOnSlingShot;

    private AngryBirdController _spawnedAngryBird;

    private AudioSource _audioSource;

    void Awake() {
        _audioSource = GetComponent<AudioSource>();

        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngryBird();
    }

    void Update()
    {
        if(InputManager.WasLeftMouseButtonPressed && _slingShotCollider.IsWithinSlingShotArea()){
            _clickedWithinArea = true;

            if(_isBirdOnSlingShot)
            {
                SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
                _cameraManager.SwitchToFollowCam(_spawnedAngryBird.transform);
            }
        }
        if(InputManager.IsLeftMousePressed && _clickedWithinArea && _isBirdOnSlingShot){
            DrawSlingShot();
            PositionAndRotateAngryBird();
        }

        if(InputManager.WasLeftMouseButtonReleased && _isBirdOnSlingShot && _clickedWithinArea){
            if (GameManager.instance.HasEnoughShots()){
                _clickedWithinArea = false;

                _spawnedAngryBird.LaunchBird(_direction, _shotForce);

                SoundManager.instance.PlayRandomClip(_elasticReleasedClips, _audioSource);
                GameManager.instance.UseShot();

                _isBirdOnSlingShot = false;
                AnimateSlingShot();

                if(GameManager.instance.HasEnoughShots()){
                    StartCoroutine(SpawnAngryBirdAfterTime());
                }  
            }
        }
    }

    #region SlingShot Methods

    private void DrawSlingShot(){ 
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPos - _centerPosition.position, _maxDistance);
        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector3 pos){
        if(!_leftLineRenderer.enabled){
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, pos);
        _leftLineRenderer.SetPosition(1,_leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, pos);
        _rightLineRenderer.SetPosition(1,_rightStartPosition.position);
    }

    #endregion

    #region Angry Bird Methods

    private void SpawnAngryBird()
    {
        _elasticTransform.DOComplete();
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPos = (Vector2)_idlePosition.position + dir * _angryBirdPositionOffset;

        _spawnedAngryBird = Instantiate(_angryBirdPrefab, spawnPos, Quaternion.identity);
        _spawnedAngryBird.transform.right = dir;

        _isBirdOnSlingShot = true;
    }

    private void PositionAndRotateAngryBird()
    {
        _spawnedAngryBird.transform.position = _slingShotLinesPosition + _directionNormalized * _angryBirdPositionOffset;
        _spawnedAngryBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngryBirdAfterTime(){
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);

        SpawnAngryBird();

        _cameraManager.SwitchToIdleCam();
    }

    #endregion

    #region Animate SlingShot

    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);
        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingshotLines(_elasticTransform, time));
    }

    private IEnumerator AnimateSlingshotLines(Transform trans, float time){
        float elapsedTime = 0f;
        while(elapsedTime < time && elapsedTime < _maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }

    #endregion

}
