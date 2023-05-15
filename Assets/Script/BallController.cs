using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BallController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float force;
    [SerializeField] LineRenderer aimLine;
    [SerializeField] Transform aimWorld;
    bool shoot;
    bool shootingMode;
    float forceFactor;
    int shootCount;
    Ray ray;
    Plane plane;
    Vector3 forceDirection;

    public bool ShootingMode { get => shootingMode; }
    public int ShootCount { get => shootCount; }

    public UnityEvent<int> onBallShooted = new UnityEvent<int>(); 

    public void Update()
    {
        if(ShootingMode)
        {
            if(Input.GetMouseButtonDown(0))
            {
                aimLine.gameObject.SetActive(true);
                aimWorld.gameObject.SetActive(true);
                plane = new Plane(Vector3.up, this.transform.position);
            }
            else if (Input.GetMouseButton(0))
            {   
                //force direction
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                plane.Raycast(ray, out var distance);
                forceDirection = this.transform.position - ray.GetPoint(distance);
                forceDirection.Normalize();
                
                //aim visual
                aimWorld.forward = forceDirection;
                aimWorld.transform.position = this.transform.position;
                aimWorld.localScale = new Vector3(1,1,0.5f + forceFactor);
                
                //force factor
                var mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var ballViewportPos = Camera.main.WorldToViewportPoint(this.transform.position);
                var pointerDirection = ballViewportPos - mouseViewportPos;
                pointerDirection.z = 0;
                pointerDirection.x *= Camera.main.aspect;
                pointerDirection.z = Mathf.Clamp(pointerDirection.z, -0.5f, 0.5f);
                forceFactor = pointerDirection.magnitude * 2;

                //draw aim
                var ballScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
                var mouseScreenPos = Input.mousePosition;
                ballScreenPos.z = 1;
                mouseScreenPos.z = 1;

                var position = new Vector3[]
                {
                    Camera.main.ScreenToWorldPoint(ballScreenPos),
                    Camera.main.ScreenToWorldPoint(mouseScreenPos)
                };
                aimLine.SetPositions(position);
                aimLine.endColor = Color.Lerp(Color.blue, Color.red, forceFactor);

            }
            else if (Input.GetMouseButtonUp(0))
            {
                shoot = true;
                shootingMode = false;
                aimLine.gameObject.SetActive(false);
                aimWorld.gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (shoot)
        {
            shoot = false;
            AddForce(forceDirection * force * forceFactor, ForceMode.Impulse);
            shootCount += 1;
            onBallShooted.Invoke(shootCount);
        }

        if (rb.velocity.sqrMagnitude < 0.01f && rb.velocity.sqrMagnitude > 0)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    public bool isMove()
    {
        return rb.velocity != Vector3.zero;
    }

    public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse)
    {
        rb.useGravity = true;
        rb.AddForce(force, forceMode);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(this.isMove())
            return;
            
        shootingMode = true;
    }
}
