using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMove : MonoBehaviour

{    // Allows remote access for unique instance
    public static CameraMove Instance
    {
        get
        {
            return instance;
        }
    }
    private static CameraMove instance;

    private Vector3 velocity = Vector3.zero;
    public float dampTime = 5f;
    public float rotationTime = 2f;
    float travelTime;
    public float quickTime = 0.5f;

    public Vector3 playerTurnPos, playerTurnRot, enemyTurnRot, baseRotationFriendly;
    private Vector3 destinationPosition, destinationRotation;
    private bool cameraIsMovingSmooth, cameraIsMovingQuick, cameraIsRotating;

    private Vector3 partyFocusRot, partyFocusPos, partyBottomRot, partyBottomPos, partyZoomPos, partyZoomRot;

    public Vector3 friendlyPos, enemyPos, friendlyPartyZoomPos, enemyTurnPos, enemyTargetPos;

    private void OnEnable()
    {
        EventManager.RhythmGameStopEvent += ReturnCamera;
        EventManager.MoveCameraSmoothEvent += MoveCameraSmooth;
    }

    private void OnDisable()
    {
        EventManager.RhythmGameStopEvent -= ReturnCamera;
        EventManager.MoveCameraSmoothEvent -= MoveCameraSmooth;

    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerTurnPos = new Vector3(0f, 13.7f, -78f);
        playerTurnRot = new Vector3(0f, 0, 0);

        baseRotationFriendly = new Vector3(0, 0, 0); //replace
        friendlyPartyZoomPos = new Vector3(11f, 13.7f, -78f); //replace

        enemyTurnPos = new Vector3(0f, 20f, -70f);
        enemyTurnRot = new Vector3(13f, 0, 0);

        enemyTargetPos = new Vector3(-3f, 20f, -75f);

        partyFocusPos = new Vector3(3f, 13f, -70f);
        partyFocusRot = new Vector3(0f, 5f, 0f);

        partyBottomPos = new Vector3(2f, 4.5f, -70f);
        partyBottomRot = new Vector3(-6f, 0f, 0f);

        partyZoomPos = new Vector3(11f, 15.5f, -45f);
        partyZoomRot = new Vector3(20f, 9f, 1.5f);
    }

    public void PlayerTurn()
    {
        MoveCameraSmooth(playerTurnPos, playerTurnRot);
    }

    public void EnemyTurn()
    {
        MoveCameraSmooth(enemyTurnPos, enemyTurnRot);
    }

    public void EnemyTargetZoom()
    {
        MoveCameraSmooth(enemyTargetPos, enemyTurnRot);
    }

    public void PartyFocus()
    {
        MoveCameraSmooth(partyFocusPos, partyFocusRot);
    }

    public void PartyBottomView()
    {
        MoveCameraSmooth(partyBottomPos, partyBottomRot);
    }

    public void PartyZoom()
    {
        MoveCameraQuick(partyZoomPos, partyZoomRot);
    }

    void Update()
    {
        if (cameraIsMovingSmooth)   // Move to destination smoothly
        {
            transform.position = Vector3.SmoothDamp(transform.position, destinationPosition, ref velocity, dampTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(destinationRotation), rotationTime * Time.deltaTime);

            if (Vector3.Distance(transform.position, destinationPosition) < 0.05f &&
                Vector3.Distance(transform.eulerAngles, destinationRotation) < 0.05f)
            {
                cameraIsMovingSmooth = false;
                transform.position = destinationPosition;
                transform.eulerAngles = destinationRotation;
            }
        }

        if (cameraIsMovingQuick)   // Move to destination quickly
        {
            travelTime += quickTime * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, destinationPosition, travelTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(destinationRotation), rotationTime * Time.deltaTime);

            if (transform.position == destinationPosition)
            {
                cameraIsMovingQuick = false;
            }
        }
    }

    public void MoveCameraSmooth(Vector3 pos, Vector3 rot)
    {
        destinationPosition = pos;
        destinationRotation = rot;
        cameraIsMovingQuick = false;
        cameraIsMovingSmooth = true;
    }

    public void MoveCameraQuick(Vector3 pos, Vector3 rot)
    {
        travelTime = 0f;
        destinationPosition = pos;
        destinationRotation = rot;
        cameraIsMovingSmooth = false;
        cameraIsMovingQuick = true;
    }



    public void ReturnCamera()
    {
        if (CombatController.Instance.playerTurn)
        {
            PlayerTurn();
        } else
        {
            EnemyTurn();
        }
    }

    public void ReturnCameraQuick()
    {
        destinationPosition = playerTurnPos;
        destinationRotation = playerTurnRot;

        cameraIsMovingSmooth = false;
        cameraIsMovingQuick = true;
    }

}