using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Character2DController : MonoBehaviour
{
    public float maxSpeed = 1;
    public float jumpForce = 1;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer spriteRenderer;
    Animator anim;
    PhotonView view;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine) //내 케릭터인지 체크
        {
            //Move Speed - 가로 방향으로 버튼 누르면 힘 주어 가속도 증가
            float h = Input.GetAxisRaw("Horizontal");
            _rigidbody.AddForce(Vector2.right*h, ForceMode2D.Impulse);

            //Max Speed - 속도를 maxSpeed 만큼 제어
            if(_rigidbody.velocity.x > maxSpeed)
                _rigidbody.velocity = new Vector2(maxSpeed, _rigidbody.velocity.y);
            else if(_rigidbody.velocity.x < maxSpeed*(-1))
                _rigidbody.velocity = new Vector2(maxSpeed*(-1), _rigidbody.velocity.y);


            //Stop Speed - 가로 방향 버튼 때면 속도 급감 - 마찰력 때문에 멈추게 됨
            if(Input.GetButtonUp("Horizontal")){
                _rigidbody.velocity = new Vector2(0.5f*_rigidbody.velocity.normalized.x, _rigidbody.velocity.y);
            }

            //jump 다른 구현 방식
            // if(Input.GetButtonDown("Jump") && Mathf.Abs(_rigidbody.velocity.y) < 0.001f)
            // {
            //     _rigidbody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            // }
            
            //jump 버튼(space) 누르고 현재 점프중(에니매이션으로 체크)이 아니라면 위방향으로 힘 가해줌(jumpForce만큼)
            if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
            {
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetBool("isJumping", true);
            }

            //Landing platform - platform layer에 Ray가 닿으면 점프 모션 끔
            if (_rigidbody.velocity.y < 0)
            {
                Debug.DrawRay(_rigidbody.position, Vector3.down, new Color(0, 1, 0));

                RaycastHit2D rayHit = Physics2D.Raycast(_rigidbody.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

                if(rayHit.collider != null)
                {
                    if(rayHit.distance < 0.5f)
                        anim.SetBool("isJumping", false);
                }

            }


            //Direction Sprite
            if (Input.GetButtonDown("Horizontal"))
                spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

            //Animation
            if (Mathf.Abs(_rigidbody.velocity.x) < 0.3)
                anim.SetBool("isWalking", false);
            else
                anim.SetBool("isWalking", true);
        }
    }
        
}