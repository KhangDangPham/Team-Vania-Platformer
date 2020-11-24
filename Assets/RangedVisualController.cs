using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedVisualController : MonoBehaviour
{

    public GameObject projectile;

    //public float offset;
    public float launchForce;

    public Transform startPoint;

    private float fireRate;
    private float hideCountdown = 0;
    public float startFireRate;
    public float fireDelay = .25f;
    public float hideDelay = .0f;

    public Transform playerTransform;
    public PlayerController playerController;
    public SpriteRenderer playerSprite;
    public GameObject head;
    public GameObject bowArm;
    public GameObject drawArm;
    bool shootMode = false;
    bool flipRot = false;
    float currentFireDelay;
    float currentHideDelay;
    private void Start()
    {
        currentFireDelay = fireDelay;
    }

    public void EnterShootMode()
    {
        shootMode = true;
        currentFireDelay = fireDelay;
        head.SetActive(true);
        bowArm.SetActive(true);
        drawArm.SetActive(true);
    }

    public void ExitShootMode()
    {
        shootMode = false;
        currentHideDelay = hideDelay;
        playerController.EnableMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if(shootMode)
        {
            if(!Input.GetKey(KeyCode.Mouse1) && currentFireDelay < 0)
            {
                Shoot();
            }

            Vector2 objectPosition = transform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - objectPosition;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                playerSprite.flipX = true;
                flipRot = true;
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                playerSprite.flipX = false;
                flipRot = false;
            }
        }

        if(!shootMode && currentHideDelay < 0 && head.activeInHierarchy)
        {
            head.SetActive(false);
            bowArm.SetActive(false);
            drawArm.SetActive(false);
        }

        currentFireDelay -= Time.deltaTime;
        currentHideDelay -= Time.deltaTime;
        fireRate -= Time.deltaTime;
    }

    public void Shoot()
    {
        if (fireRate <= 0)
        {
            if (fireRate <= 0)
            {
                Quaternion startRotation = Quaternion.Euler(startPoint.eulerAngles);
                GameObject arrow = Instantiate(projectile, startPoint.position, startRotation);

                if(flipRot)
                {
                    arrow.GetComponent<Rigidbody2D>().velocity = -arrow.transform.right * launchForce;
                }
                else
                {
                    arrow.GetComponent<Rigidbody2D>().velocity = arrow.transform.right * launchForce;
                }
                FindObjectOfType<AudioManager>().Play("Bow"); //sfx
                fireRate = startFireRate;
            }
        }
        ExitShootMode();
    }
}
