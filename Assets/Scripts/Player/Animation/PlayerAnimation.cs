using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private PlayerController _player;

    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _rightLeg;
    [SerializeField] private GameObject _leftLeg;
    [SerializeField] private GameObject _rightArm;
    [SerializeField] private GameObject _leftArm;

    private float _bodyTiltXWhileCharging = 30.15f;
    private float _armTiltYWhileCharging = 40.97f;
    private float _animationtransitionSpeed = 0.25f;

    private bool _isCharging = false;

    private float _bodyTiltNormalized = 0;
    private float _armsTiltNormalized = 0;

    private void Awake()
    {
        if (_player != null)
            _player.GetComponent<PlayerMovement>().OnIsPushingEnent += SetIsCharging;
    }

    private void SetIsCharging(bool isCharging)
    {
        _isCharging = isCharging;
    }

    private void Update()
    {
        TiltBody(GetBodyTiltNormalizedValue());
        TiltArms(GetArmsTiltNormalizedValue());
    }

    private float GetBodyTiltNormalizedValue()
    {
        _bodyTiltNormalized += _isCharging == true ? _animationtransitionSpeed * Time.deltaTime : -_animationtransitionSpeed * Time.deltaTime;
        _bodyTiltNormalized = Mathf.Clamp(_bodyTiltNormalized, 0, 1);
        float bodyTileNormalzied = Mathf.Lerp(0, _bodyTiltXWhileCharging, _bodyTiltNormalized);
        return bodyTileNormalzied;
    }

    private float GetArmsTiltNormalizedValue()
    {
        _armsTiltNormalized += _isCharging == true ? _animationtransitionSpeed * Time.deltaTime : -_animationtransitionSpeed * Time.deltaTime;
        _armsTiltNormalized = Mathf.Clamp(_armsTiltNormalized, 0, 1);
        float bodyTileNormalzied = Mathf.Lerp(0, _bodyTiltXWhileCharging, _bodyTiltNormalized);
        return bodyTileNormalzied;
    }

    private void TiltBody(float normalized)
    {
        float angle = Mathf.Lerp(0, _bodyTiltXWhileCharging, normalized);
        _body.transform.localRotation = Quaternion.Euler(angle, _body.transform.localRotation.y, _body.transform.localRotation.z);
    }


    private void TiltArms(float normalized)
    {
        float angle = Mathf.Lerp(0, _armTiltYWhileCharging, normalized);
        _leftArm.transform.localRotation = Quaternion.Euler(_leftArm.transform.localRotation.x, -angle, _leftArm.transform.localRotation.z);
        _rightArm.transform.localRotation = Quaternion.Euler(_rightArm.transform.localRotation.x, angle, _rightArm.transform.localRotation.z);
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.GetComponent<PlayerMovement>().OnIsPushingEnent -= SetIsCharging;
    }

}
