using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameBiteState;
using static GameCastedState;
using static GameCastingState;
using static GameFightingState;
using static GameIdleState;
using static GameLandingState;
using static GameReelingState;
using static GameScoringState;

public class CameraManager : MonoBehaviour
{
    
    private Camera playerCamera;
    [SerializeField] float lerpSpeed = 2f;

    [Header("Camera Position Transforms")]
    [SerializeField] Transform idleCameraPosition;
    [SerializeField] Transform castingCameraPosition;
    [SerializeField] Transform castedCameraPosition;
    [SerializeField] Transform biteCameraPosition;
    [SerializeField] Transform reelingCameraPosition;
    [SerializeField] Transform landingCameraPosition;
    [SerializeField] Transform fightingCameraPosition;
    [SerializeField] Transform scoringCameraPosition;

    private void Awake()
    {
        playerCamera = GameObject.FindObjectOfType<Camera>();
    }
    private void OnEnable()
    {
        GameIdleState.onStateIdle += StateIsIdle;
        GameCastingState.onStateCasting += StateIsCasting;
        GameCastedState.onStateCasted += StateIsCasted;
        GameBiteState.onStateBite += StateIsBite;
        GameReelingState.onStateReeling += StateIsReeling;
        GameLandingState.onStateLanding += StateIsLanding;
        GameFightingState.onStateFighting += StateIsFighting;
        GameScoringState.onStateScoring += StateIsScoring;
    }

    void LerpToPosition(Transform targetPosition)
    {
        StartCoroutine(LerpCoroutine(targetPosition.position, targetPosition.rotation));
    }

    IEnumerator LerpCoroutine(Vector3 targetPosition, Quaternion targetRotation)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = playerCamera.transform.position;
        Quaternion startingRotation = playerCamera.transform.rotation;

        while (elapsedTime < 1f)
        {
            playerCamera.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime);
            playerCamera.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * lerpSpeed;
            yield return null;
        }

        playerCamera.transform.position = targetPosition; // Ensure reaching the exact target position
        playerCamera.transform.rotation = targetRotation; // Ensure reaching the exact target rotation
    }

    void StateIsIdle()
    {
        LerpToPosition(idleCameraPosition);
    }

    void StateIsCasting()
    {
        LerpToPosition(castingCameraPosition);
    }

    void StateIsCasted()
    {
        LerpToPosition(castedCameraPosition);
    }

    void StateIsBite()
    {
        LerpToPosition(biteCameraPosition);
    }

    void StateIsReeling()
    {
        LerpToPosition(reelingCameraPosition);
    }

    void StateIsLanding()
    {
        LerpToPosition(landingCameraPosition);
    }

    void StateIsFighting()
    {
        LerpToPosition(fightingCameraPosition);
    }

    void StateIsScoring()
    {
        LerpToPosition(scoringCameraPosition);
    }
}
