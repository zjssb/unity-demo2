using System;
using Unity.VisualScripting;
using UnityEngine;
using XLua;

[Hotfix]
public class Player : MonoBehaviour
{
    private float hor; // 左右
    private float vert; // 前后
    public float moveSpeed = 5f;
    public float rotateSpeed = 75f;
    private float shootTime = 1f;
    private float shootCooling = 1f;
    private float shootPower = 5f;

    /// <summary>
    /// 子弹实体
    /// </summary>
    public GameObject Ammo;

    // Update is called once per frame
    void Update()
    {
        Init();
        Move();
        Rotate();
        Shoot();
    }

    private void Init()
    {
        
    }

    private void Move()
    {
        hor = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
        transform.position =
            transform.position
            + transform.right * hor * moveSpeed * Time.deltaTime
            + transform.forward * vert * moveSpeed * Time.deltaTime;
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(rotateSpeed * Time.deltaTime * Vector3.down);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(rotateSpeed * Time.deltaTime * Vector3.up);
        }
    }

    private void Shoot()
    {
        if (shootTime < shootCooling)
        {
            shootTime += Time.deltaTime;
        }

        if (Input.GetMouseButton(0) && shootTime >= shootCooling)
        {
            shootTime = 0f;
            GameObject g = Instantiate(Ammo);
            g.transform.position = transform.Find("gan").position;
            g.GetComponent<Rigidbody>()
                .AddForce(transform.Find("gan").forward * shootPower, ForceMode.Impulse);
            Destroy(g, 3f);
        }
    }

    public void lua()
    {

    }
}
