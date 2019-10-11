using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.AI;

public class SheepController : MonoBehaviour
{
    [Serializable]
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

    [SerializeField]
    ParticleSystem runParticle;

    [SerializeField]
    TrailRenderer speedTrail;

    [SerializeField]
    Color trailColor = Color.white;


    bool isIdle = true;

    public Action onMoveOutOfCageDone = null;

    private Color highLightColor = Color.white;
    private Color canMergeColor = Color.white;

    [SerializeField]
    private int mSheepType;

    double sheepValue;
    float sheepSpeed;

    public int SheepType { get => mSheepType; private set => mSheepType = value; }
    public SheepState SheepStateProp { get => sheepState; set => sheepState = value; }
    public double SheepValue { get => sheepValue; set => sheepValue = value; }

    Tweener currentTween = null;
    Tween particleRunTween = null;

    bool isBoostSpeedUpOnce=false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;
        rigidbody = GetComponent<Rigidbody>();

        if (speedTrail)
            speedTrail.startColor = speedTrail.endColor = trailColor;
        ColorUtility.TryParseHtmlString("#FF00E9", out highLightColor);
        ColorUtility.TryParseHtmlString("#00FF41", out canMergeColor);
    }

    private void Start()
    {
        //DOMoveOnPath();
        //sheepState = SheepState.Idle;
        //rigidbody.isKinematic = true;
        //boxCollider.enabled = true;

    }


    public void InitSheep(double psheepValue,float psheepSpeed)
    {
        sheepState = SheepState.Idle;
        rigidbody.isKinematic = true;
        boxCollider.enabled = true;

       
        sheepSpeed = psheepSpeed;
        sheepValue = psheepValue;
        skinnedMeshRenderer.material.SetFloat("_Outline", 0);
    }

    public void InitSheep(int sheepType)
    {
        SheepConfigData sheepConfigData = ConfigManager.Instance.GetSheepConfigByType(sheepType);
        InitSheep(sheepConfigData.Sheepvalue, sheepConfigData.Speed);
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

        speedTrail.gameObject.SetActive(BoostTimer.Instance.IsBoostSpeedUp && (sheepState == SheepState.Running || sheepState == SheepState.Jumping));
    }


    public void ResetState()
    {
        sheepState = SheepState.Idle;
        rigidbody.isKinematic = true;
        boxCollider.enabled = true;
        animator.SetBool("Moving", false);
        transform.DOKill();
        skinnedMeshRenderer.material.SetFloat("_Outline", 0);

        SheepConfigData sheepConfigData = ConfigManager.Instance.GetSheepConfigByType(mSheepType);
        InitSheep(sheepConfigData.Sheepvalue, sheepConfigData.Speed);

        speedTrail.gameObject.SetActive(false);
        runParticle.gameObject.SetActive(false);
        particleRunTween.Kill();
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

    public void DOMoveOnPath()
    {
        sheepState = SheepState.Running;
        rigidbody.isKinematic = true;
        boxCollider.enabled = true;
        animator.SetBool("Moving", true);
        float randSpeed = UnityEngine.Random.Range(-2, 2);
        //this.transform.DOPath(GameController.Instance.GetMovePathPoints(1), GameController.Instance.pathDuration1 + randSpeed).SetLookAt(0.02f);
        float speedUp = BoostTimer.Instance.IsBoostSpeedUp ? 0.25f : 1f;
        
        currentTween=this.transform.DOPath(GameController.Instance.GetMovePathPoints(1), sheepSpeed*speedUp).SetLookAt(0.02f);
        //currentTween.SetSpeedBased(true);
        
        if (isBoostSpeedUpOnce && speedTrail.gameObject.activeSelf)
        {
            speedTrail.gameObject.SetActive(false);
        }
        if (runParticle.gameObject.activeSelf == false)
        {
            runParticle.gameObject.SetActive(true);
            particleRunTween = DOVirtual.DelayedCall(sheepSpeed / 10, () =>
              {
                  runParticle.Stop();
                  runParticle.Play();
              });
            particleRunTween.SetLoops(-1);
        }
        
    }

    void DoMoveToEndPosition()
    {
        sheepState = SheepState.Running;
        animator.SetBool("Moving", true);
        Tweener moveTween = this.transform.DOPath(GameController.Instance.GetMovePathPoints(2), sheepSpeed*0.07f).SetLookAt(0.02f);
        //Sheep will go a gain path, we got a lopp move
        moveTween.onComplete = () =>
        {
            DOMoveOnPath();
        };

        currentTween = moveTween;

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
        currentTween = this.transform.DOPath(curvePoints, jumpTime, PathType.CatmullRom);
        currentTween.onComplete = () =>
          {
              isJumping = false;
              DoMoveToEndPosition();
              GameController.Instance.AddSheepValue(this.sheepValue);
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

    public void BoostSpeedOnce()
    {
        if (BoostTimer.Instance.IsBoostSpeedUp || isBoostSpeedUpOnce)
            return;
        isBoostSpeedUpOnce = true;
        if (currentTween != null && speedTrail.gameObject.activeSelf==false)
        {
            currentTween.timeScale *= 4;
            speedTrail.gameObject.SetActive(true);
        }
    }

  
}