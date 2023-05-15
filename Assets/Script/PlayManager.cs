using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayManager : MonoBehaviour
{
    [SerializeField] BallController ballController;
    [SerializeField] CameraController camController;
    [SerializeField] GameObject finishWindow;
    [SerializeField] TMP_Text finishText;
    [SerializeField] TMP_Text shootCountText;
    bool isBallOutside;
    bool isBallTeleporting;
    bool isGoal;
    Vector3 lastBallPosition;

    private void OnEnable()
    {
        ballController.onBallShooted.AddListener(UpdateShootCount);
    }
    private void OnDisable()
    {
        ballController.onBallShooted.RemoveListener(UpdateShootCount);
    }
    private void Update()
    {
        if(ballController.ShootingMode)
        {
            lastBallPosition = ballController.transform.position;
        }
        var inputActive = Input.GetMouseButton(0) 
            && ballController.isMove() == false 
            && ballController.ShootingMode == false
            && isBallOutside == false;

        camController.SetInputActive(inputActive);
    }

    public void OnBallGoalEnter()
    {
        isGoal = true;
        ballController.enabled = false;

        finishWindow.gameObject.SetActive(true);
        finishText.text = $"Finisihed!!!\nShoot Count {ballController.ShootCount}";
    }

    public void OnBallOutside()
    {
        if(isGoal)
            return;

        if(isBallTeleporting == false)
            Invoke("TelportBallLastPosition", 1);

        ballController.enabled = false;
        isBallOutside = true;
        isBallTeleporting = true;
    }

    public void TelportBallLastPosition()
    {
        MoveBallLastPosition(lastBallPosition);
    }

    public void MoveBallLastPosition(Vector3 targetPosition)
    {
        var rb = ballController.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        ballController.transform.position = targetPosition;
        rb.isKinematic = false;
        
        ballController.enabled = true;
        isBallOutside = false;
        isBallTeleporting = false;
    }

    public void UpdateShootCount(int shootCount)
    {
        shootCountText.text = shootCount.ToString();
    }
}
