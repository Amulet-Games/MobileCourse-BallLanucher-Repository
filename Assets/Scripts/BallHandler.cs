using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot;
    [SerializeField] float waitSec = 1f;
    [SerializeField] float respawnDelay;

    Rigidbody2D currentBallRigidbody;
    SpringJoint2D currentBallSpringJoint;
    Camera _mainCamera;

    bool isDragging;
    
    private void Start()
    {
        _mainCamera = Camera.main;

        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    
    private void Update()
    {
        if (Touchscreen.current == null || currentBallRigidbody == null)
            return;
        
        if (Touch.activeTouches.Count > 0)
        {
            Vector2 touchPosition = new Vector2();
            Rect screenBounds = new Rect(0, 0, Screen.width, Screen.height);

            foreach (Touch touch in Touch.activeTouches)
            {
                if (!screenBounds.Contains(touch.screenPosition))
                    continue;

                touchPosition += touch.screenPosition;
            }

            touchPosition /= Touch.activeTouches.Count;
            
            Vector3 _worldPoint = _mainCamera.ScreenToWorldPoint(touchPosition);

            currentBallRigidbody.position = _worldPoint;

            currentBallRigidbody.isKinematic = true;

            isDragging = true;
        }
        else
        {
            if (isDragging)
            {
                LaunchBall();
            }
            
            isDragging = false;
        }
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void SpawnNewBall()
    {
        GameObject newBallInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = newBallInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = newBallInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }

    void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        StartCoroutine(DetachBall());
    }

    IEnumerator DetachBall()
    {
        yield return new WaitForSeconds(waitSec);
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        StartCoroutine(RespawnBall());
    }

    IEnumerator RespawnBall()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnNewBall();
    }
}
