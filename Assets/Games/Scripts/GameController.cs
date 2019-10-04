using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class GameController : Singleton<GameController>
{
    GameController() { }

    [SerializeField]
    Transform[] movePath1;
    Vector3[] movePathPoints1;

    [SerializeField]
    Transform[] movePath2;
    Vector3[] movePathPoints2;

    [SerializeField]
    Transform beginPos;

    [SerializeField]
    Transform cageGate;

    [SerializeField]
    BoxCollider spawningBox;

    [SerializeField]
    SheepController sheepPrefab;

    [SerializeField]
    SheepController cutSceneSheep;

    [SerializeField]
    TMPRichTextX sampleText;

    [SerializeField]
    Transform jumpingFence;

    [SerializeField]
    Canvas camCanvas;

    [SerializeField]
    Cinemachine.CinemachineVirtualCamera camTarget;
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera camOverrall;
    [SerializeField]
    Cinemachine.CinemachineDollyCart camCart;
    [SerializeField]
    Cinemachine.CinemachineSmoothPath camPath;

    [SerializeField]
    GameObject moveOutTrigger;

    [SerializeField]
    TMPRichTextX txtTopCountingText;

    //Create Sheep Pool
    SheepController[] sheepControllersPool;
    int numOfSheep=20;
    int currentSheepPoolCursor = 0;


    //Create Text Pool
    TMPRichTextX[] coutingTextPool;
    int numOfCountingText = 20;
    int currentTextPoolCursor = 0;
    Vector3[] coutingTextFlyWayPoint = new Vector3[] { new Vector3(-2.850415f, 4.118391f, -1.21047f), new Vector3(-0.94301f, 5.670042f, -2.24463f), new Vector3(-1.534367f, 7.890669f, -0.4035393f), new Vector3(0.2940083f, 9.312242f, -1.431876f) };

    List<SheepController> activeSheeps = new List<SheepController>();
    List<SheepController> canMergeSheeps = new List<SheepController>();
    GameObject sheepGroup;

    public float pathDuration1;
    public float pathDuration2;
    public Camera mainCam;

    private int currentSheepCount = 0;
    [SerializeField]
    private bool isOnCutScene = true;

    private int currentSheepIsMovingOut = 0;

    private SheepController pickingSheep=null;
    private SheepController lastCanMergeSheep=null;
    private CameraHandler cameraHandler;

    private Vector3 lastPickingPos = Vector3.zero;

    private void Awake()
    {
        mainCam = Camera.main;
        int movePathLeng = movePath1.Length;
        movePathPoints1 = new Vector3[movePathLeng];

        for (int i = 0; i < movePathLeng; ++i)
        {
            movePathPoints1[i] = movePath1[i].position;
        }

        movePathLeng = movePath2.Length;
        movePathPoints2 = new Vector3[movePathLeng];

        for (int i = 0; i < movePathLeng; ++i)
        {
            movePathPoints2[i] = movePath2[i].position;
        }
        
        InitSheepPool();
        InitTextPool();

        sampleText.gameObject.SetActive(false);
        cutSceneSheep.onMoveOutOfCageDone = CloseGate;
        cameraHandler = mainCam.GetComponent <CameraHandler>();
        currentSheepIsMovingOut = 0;

        moveOutTrigger.gameObject.SetActive(false);
    }


    private void Start()
    {
        if (isOnCutScene)
        {
            cameraHandler.enabled = false;
            cutSceneSheep.gameObject.SetActive(true);
            OpenGateLeaveSheepOut(cutSceneSheep);
        }
        else
        {
            Destroy(camTarget.gameObject);
            Destroy(camOverrall.gameObject);
            Destroy(camCart.transform.parent.gameObject);

            Destroy(mainCam.GetComponent<Cinemachine.CinemachineBrain>());

            cameraHandler.enabled = true;
            cutSceneSheep.gameObject.SetActive(false);
            int initSheepCount = 10;
            for (int i = 0; i < initSheepCount; ++i)
            {
                SpawnSheep();
            }
        }

    }


    private void Update()
    {

        if (isOnCutScene)
        {
            if (camCart.m_Position == camPath.PathLength && camTarget.gameObject.activeSelf)
            {
                camTarget.gameObject.SetActive(false);
                camOverrall.gameObject.SetActive(true);
                DOVirtual.DelayedCall(2f, () =>
                {
                    //camOverrall.gameObject.SetActive(false);
                    Destroy(camTarget.gameObject);
                    Destroy(camOverrall.gameObject);
                    Destroy(camCart.transform.parent.gameObject);

                    Destroy(mainCam.GetComponent<Cinemachine.CinemachineBrain>());
                    cameraHandler.enabled = true;
                    isOnCutScene = false;
                    SpawnSheep();
                });
            }
        }
        else 
        {
            //Mouse down
            if (Input.GetMouseButtonDown(0))
            {
                Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                int sheepLayer = LayerMask.GetMask("Sheep");
                if (Physics.Raycast(camRay, out rayHit, float.MaxValue, sheepLayer))
                {
                    SheepController sheepController = rayHit.collider.GetComponent<SheepController>();
                    if (sheepController.SheepStateProp == SheepController.SheepState.Idle)
                    {
                        pickingSheep = sheepController;
                        var pos = lastPickingPos = pickingSheep.transform.position;
                        pos.y += 0.5f;
                        pickingSheep.transform.position = pos;
                        cameraHandler.enabled = false;
                        pickingSheep.SetIsPickingUp(true);
                        moveOutTrigger.gameObject.SetActive(true);
                    }else if (sheepController.SheepStateProp == SheepController.SheepState.Running)
                    {
                        PutSheepBackToCage(sheepController);
                    }
                  
                    //MarkCanMergeSheep(pickingSheep);
                }
            }else if (Input.GetMouseButton(0))//Mouse move
            {
                if (pickingSheep != null)
                {
                    float currentHeight = pickingSheep.transform.position.y;
                    Ray camRay = GameController.Instance.mainCam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit rayHit;
                    int groundLayer = LayerMask.GetMask("Ground");
                    if (Physics.Raycast(camRay, out rayHit, float.MaxValue, groundLayer))
                    {
                        Vector3 mouseWorldPos = rayHit.point;
                        mouseWorldPos.y = currentHeight;
                        pickingSheep.transform.position = mouseWorldPos;
                    }

                    int sheepLayer = LayerMask.GetMask("Sheep");
                    if (Physics.Raycast(camRay, out rayHit, float.MaxValue, sheepLayer))
                    {
                        SheepController selectSheep = rayHit.collider.GetComponent<SheepController>();
                        if (lastCanMergeSheep != null && lastCanMergeSheep != selectSheep)
                        {
                            lastCanMergeSheep.SetCanMerge(false);
                        }
                        if (selectSheep.SheepStateProp == SheepController.SheepState.Idle && selectSheep.SheepType == pickingSheep.SheepType)
                        {
                            selectSheep.SetCanMerge(true);
                        }
                        lastCanMergeSheep = selectSheep;
                    }
                    else
                    {
                        if (lastCanMergeSheep != null)
                        {
                            lastCanMergeSheep.SetCanMerge(false);
                        }
                    }
                }
                
            }
            else if (Input.GetMouseButtonUp(0))
            {
                
                if (pickingSheep != null)
                {

                    Ray camRay = GameController.Instance.mainCam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit rayHit;
                    int sheepLayer = LayerMask.GetMask("Sheep");

                    int roadLayer = LayerMask.GetMask("Road");

                   
                    if (Physics.Raycast(camRay, out rayHit, float.MaxValue, roadLayer))
                    {
                        pickingSheep.transform.position = lastPickingPos;
                        OpenGateLeaveSheepOut(pickingSheep);
                    }
                    else if (Physics.Raycast(camRay, out rayHit, float.MaxValue, sheepLayer))
                    {
                            SheepController selectSheep = rayHit.collider.GetComponent<SheepController>();
                            if (selectSheep.SheepStateProp == SheepController.SheepState.Idle && selectSheep.SheepType == pickingSheep.SheepType)
                            {
                                //DoMerge();
                                DoMerge(pickingSheep, selectSheep);
                            }
                            else
                            {
                                var pos = pickingSheep.transform.position;
                                pos.y -= 0.5f;
                                pickingSheep.transform.position = pos;
                            }
                    }
                    else
                    {

                        var pos = pickingSheep.transform.position;
                        pos.y -= 0.5f;
                        pickingSheep.transform.position = pos;

                    }

                    pickingSheep.SetIsPickingUp(false);
                    pickingSheep = null;
                    lastCanMergeSheep = null;
                    cameraHandler.enabled = true;
                    moveOutTrigger.gameObject.SetActive(false);
                }


            }
        }


    }


    void OpenGateLeaveSheepOut(SheepController sheepController)
    {
        currentSheepIsMovingOut++;
        cageGate.DORotate(Vector3.zero, 0.5f).onComplete = () =>
        {
            sheepController.MoveOutOfCage();
        };
    }


    void InitSheepPool()
    {
        sheepGroup = new GameObject("---------SheepGroup-----------");
        Transform sheepGroupTrans = sheepGroup.transform;
        sheepGroup.transform.position = Vector3.zero;
        currentSheepPoolCursor = 0;
        sheepControllersPool = new SheepController[numOfSheep];
        for (int i = 0; i < numOfSheep; ++i)
        {
            sheepControllersPool[i] = Instantiate(sheepPrefab, sheepGroupTrans);
            sheepControllersPool[i].gameObject.SetActive(false);
            sheepControllersPool[i].onMoveOutOfCageDone = CloseGate;
        }
    }

    void InitTextPool()
    {
        currentTextPoolCursor = 0;
        coutingTextPool = new TMPRichTextX[numOfCountingText];
        for (int i = 0; i < numOfCountingText; ++i)
        {
            coutingTextPool[i] = Instantiate(sampleText, sampleText.transform.parent);
            coutingTextPool[i].gameObject.SetActive(false);
        }
    }

    public void CloseGate()
    {
        currentSheepIsMovingOut--;
        if (currentSheepIsMovingOut == 0)
        {
            cageGate.DORotate(new Vector3(0, 90, 0), 0.5f).onComplete = () =>
            {

            };
        }
        
    }

    SheepController SpawnSheep()
    {
        SheepController sheep = sheepControllersPool[currentSheepPoolCursor];
        currentSheepPoolCursor++;
        if (currentSheepPoolCursor >= numOfSheep)
            currentSheepPoolCursor = 0;

        float x = UnityEngine.Random.Range(spawningBox.bounds.center.x - spawningBox.bounds.extents.x, spawningBox.bounds.center.x + spawningBox.bounds.extents.x);
        float z = UnityEngine.Random.Range(spawningBox.bounds.center.z - spawningBox.bounds.extents.z, spawningBox.bounds.center.z + spawningBox.bounds.extents.z);

        sheep.transform.position = new Vector3(x, 1.8f, z);

        sheep.transform.localEulerAngles = new Vector3(0, UnityEngine.Random.Range(0,360), 0);

        sheep.gameObject.SetActive(true);
        activeSheeps.Add(sheep);
        return sheep;
    }

    void PutSheepBackToCage(SheepController sheepController)
    {
        float x = UnityEngine.Random.Range(spawningBox.bounds.center.x - spawningBox.bounds.extents.x, spawningBox.bounds.center.x + spawningBox.bounds.extents.x);
        float z = UnityEngine.Random.Range(spawningBox.bounds.center.z - spawningBox.bounds.extents.z, spawningBox.bounds.center.z + spawningBox.bounds.extents.z);

        sheepController.transform.position = new Vector3(x, 1.8f, z);
        sheepController.ResetState();
        
    }

    public Vector3[] GetMovePathPoints(int index)
    {
        if (index == 1)
            return movePathPoints1;
        return movePathPoints2;
    }

    public Vector3 GetBeginPos()
    {
        return beginPos.transform.position;
    }

    public void ShowCoutingText()
    {

        currentSheepCount++;

        TMPRichTextX txtMesh = coutingTextPool[currentTextPoolCursor];
        currentTextPoolCursor++;
        if (currentTextPoolCursor >= numOfCountingText)
            currentTextPoolCursor = 0;

        string content = string.Format("<sprite>sheepIcon</sprite>{0}", currentSheepCount.ToString());
        txtTopCountingText.SetText(content);
        txtMesh.SetText(content);

        txtMesh.DOKill();
        txtMesh.TextMeshProUGUIComp.DOFade(1, 0);
        txtMesh.gameObject.SetActive(true);
        txtMesh.transform.position = sampleText.transform.position;

        txtMesh.transform.DOPath(coutingTextFlyWayPoint,5.0f,PathType.CatmullRom);
        txtMesh.TextMeshProUGUIComp.DOFade(0, 5.0f).onComplete=()=>
        {
            txtMesh.gameObject.SetActive(false);
        };
    }


    void MarkCanMergeSheep(SheepController pickingUpSheep)
    {
        int activeSheepCount = activeSheeps.Count;
        for (int i = 0; i < activeSheepCount; ++i)
        {
            if (pickingUpSheep != activeSheeps[i]  && activeSheeps[i].SheepStateProp == SheepController.SheepState.Idle && activeSheeps[i].SheepType == pickingUpSheep.SheepType)
            {
                activeSheeps[i].SetCanMerge(true);
                canMergeSheeps.Add(activeSheeps[i]);
            }
        }
    }

    void ResetMergeSheep()
    {
        int mergeSheepCount = canMergeSheeps.Count;
        for (int i = 0; i < mergeSheepCount; ++i)
        {
            canMergeSheeps[i].SetCanMerge(false);
        }
        canMergeSheeps.Clear();
    }

    void DoMerge(SheepController pickUpSheep, SheepController pointSheep)
    {
        int nextSheepLevel = pickUpSheep.SheepType + 1;
        SheepController sheepEvo = SheepFactory.Instance.CreateNewSheep(nextSheepLevel);
        sheepEvo.onMoveOutOfCageDone = CloseGate;
        sheepEvo.transform.position = pointSheep.transform.position;

        pickUpSheep.gameObject.SetActive(false);
        pointSheep.gameObject.SetActive(false);

        activeSheeps.Remove(pickUpSheep);
        activeSheeps.Remove(pointSheep);

        activeSheeps.Add(sheepEvo);

    }
}
