using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.AI;

public class SheepController : MonoBehaviour
{
    public enum SheepState
    {
        Idle,
        IsMovingOut,
        Running,
        Jumping,
        Picking
    }

    SheepState sheepState;

    Animator animator;
    bool isJumping = false;

    NavMeshAgent navMeshAgent;
    bool isMoveOutCage = false;

    Rigidbody rigidbody;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;

    bool isIdle = true;

    public Action onMoveOutOfCageDone = null;

    private Color highLightColor = Color.white;
    private Color canMergeColor = Color.white;

    [SerializeField]
    private int mSheepType;

    public int SheepType { get => mSheepType; private set => mSheepType = value; }
    public SheepState SheepStateProp { get => sheepState; set => sheepState = value; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;
        rigidbody = GetComponent<Rigidbody>();


        ColorUtility.TryParseHtmlString("#FF00E9", out highLightColor);
        ColorUtility.TryParseHtmlString("#00FF41", out canMergeColor);
    }

    private void Start()
    {
        //DOMoveOnPath();
        sheepState = SheepState.Idle;
        rigidbody.isKinematic = true;
        boxCollider.enabled = true;

    }

    private void Update()
    {
        //Right Click Test Move With NavMesh
        //if (Input.GetMouseButtonDown(1))
        //{
        //    Ray camRay = GameController.Instance.mainCam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit rayHit;
        //    int groundLayer = LayerMask.GetMask("Ground");
        //    if (Physics.Raycast(camRay, out rayHit, float.MaxValue, groundLayer))
        //    {
        //        animator.SetBool("Moving", true);
        //        navMeshAgent.SetDestination(rayHit.point);
        //    }
        //}

        if (isMoveOutCage)
        {
            float dist = (this.transform.position - GameController.Instance.GetBeginPos()).sqrMagnitude;
            if (dist <= 0.2f)
            {
                isMoveOutCage = false;
                navMeshAgent.enabled = false;
                DOMoveOnPath();
                //if (onMoveOutOfCageDone != null)
                //    onMoveOutOfCageDone();
                GameController.Instance.CloseGate();
            }
        }
    }


    public void ResetState()
    {
        sheepState = SheepState.Idle;
        rigidbody.isKinematic = true;
        boxCollider.enabled = true;
        animator.SetBool("Moving", false);
        transform.DOKill();
    }

    public void MoveOutOfCage()
    {
        sheepState = SheepState.IsMovingOut;
        animator.SetBool("Moving", true);
        rigidbody.isKinematic = false;
        boxCollider.enabled = false;
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(GameController.Instance.GetBeginPos());
        isMoveOutCage = true;
        //gameObject.layer = LayerMask.GetMask("Default");
    }

    void DOMoveOnPath()
    {
        sheepState = SheepState.Running;
        rigidbody.isKinematic = true;
        boxCollider.enabled = true;
        animator.SetBool("Moving", true);
        float randSpeed = UnityEngine.Random.Range(-2, 2);
        this.transform.DOPath(GameController.Instance.GetMovePathPoints(1), GameController.Instance.pathDuration1 + randSpeed).SetLookAt(0.02f);
    }

    void DoMoveToEndPosition()
    {
        sheepState = SheepState.Running;
        animator.SetBool("Moving", true);
        Tweener moveTween = this.transform.DOPath(GameController.Instance.GetMovePathPoints(2), GameController.Instance.pathDuration2).SetLookAt(0.02f);
        //Sheep will go a gain path, we got a lopp move
        moveTween.onComplete = () =>
        {
            DOMoveOnPath();
        };

    }

    /// <summary>
    /// Using DOTween DOPath with Curve
    /// </summary>
    internal void DoJumpOverFence()
    {
        if (isJumping)
            return;
        animator.SetTrigger("Jump");
        isJumping = true;
        sheepState = SheepState.Jumping;
        //Stop Move Follow Path Tween then DO Jump Tween
        this.transform.DOKill();

        Vector3 curPos = this.transform.position;
        float jumpDistance = 2f;
        float jumpHeight = 0.6f;
        float jumpTime = 0.5f;
        Vector3 jumpTargetPos = curPos + this.transform.forward * jumpDistance;
        Vector3 midPos = (curPos + jumpTargetPos) / 2;
        midPos.y += jumpHeight;

        Vector3[] curvePoints = new Vector3[] { curPos, curPos, midPos, jumpTargetPos, jumpTargetPos };

        //Jump Done, sheep follow path to reach end position
        this.transform.DOPath(curvePoints, jumpTime, PathType.CatmullRom).onComplete = () =>
          {
              isJumping = false;
              DoMoveToEndPosition();
              GameController.Instance.ShowCoutingText();
          };
    }

    public void SetIsPickingUp(bool isPickingUp)
    {
        animator.SetBool("PickingUp", isPickingUp);
      
        if (isPickingUp)
        {
            sheepState = SheepState.Picking;
            skinnedMeshRenderer.material.SetColor("_OutlineColor", highLightColor);
            skinnedMeshRenderer.material.SetFloat("_Outline", 0.5f);
        }
        else
        {
            sheepState = SheepState.Idle;
            skinnedMeshRenderer.material.SetFloat("_Outline", 0);
        }
    }

    public void SetCanMerge(bool isCanMerge)
    {
        if (isCanMerge)
        {
            skinnedMeshRenderer.material.SetColor("_OutlineColor", canMergeColor);
            skinnedMeshRenderer.material.SetFloat("_Outline", 0.5f);
        }
        else
        {
            skinnedMeshRenderer.material.SetFloat("_Outline", 0);
        }

    }

  
}