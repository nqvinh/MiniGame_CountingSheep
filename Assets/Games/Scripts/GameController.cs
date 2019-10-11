using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine.UI;

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

    [SerializeField]
    TextMeshProUGUI txtLevelProgress;

    [SerializeField]
    Image lvlProgress;

    [SerializeField]
    TextMeshProUGUI txtLimitSheep;

    [SerializeField]
    Farmer farmer;

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
    private double currentSheepValue = 0;

    [SerializeField]
    private bool isOnCutScene = true;

    private int currentSheepIsMovingOut = 0;

    private SheepController pickingSheep=null;
    private SheepController lastCanMergeSheep=null;
    private CameraHandler cameraHandler;

    private Vector3 lastPickingPos = Vector3.zero;

    public const int numOfSheepType= 50;
    private bool isLockCamera = false;

    private int currentSheepInCage = 0;

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
        ConfigManager.Instance.LoadConfig();
        SimpleResourcesManager.Instance.LoadResource();
        DialogManager.Instance.LoadDialog();
        DataManager.Instance.LoadData((isSuccess) =>
        {
            BoostTimer.Instance.SetUp();
            DataManager.Instance.PlayerData.userPlayTime += 1;
            currentSheepValue = DataManager.Instance.PlayerData.userSheepCoin;
            LoadSheep(DataManager.Instance.PlayerData.sheepDatas);
            string content = string.Format("<sprite>sheepIcon</sprite>{0}", BigNumbers.BigNumber.ToShortString(currentSheepValue.ToString("F0"), 4));
            txtTopCountingText.SetText(content);

            LevelConfigData lvlConfig = ConfigManager.Instance.GetLevelConfigByLevel(DataManager.Instance.PlayerData.userLevel);
            float lvlProgressVal = DataManager.Instance.PlayerData.userExp*1.0f / lvlConfig.Maxexp;

            txtLevelProgress.text = "Lv. " + DataManager.Instance.PlayerData.userLevel + "- <color=white>" + (lvlProgressVal * 100).ToString("F0") + "%";
            lvlProgress.fillAmount = lvlProgressVal;

            txtLimitSheep.text = currentSheepInCage+"/"+lvlConfig.Maxsheep;

            if (DataManager.Instance.PlayerData.userLevel >= 3)
                SpawnTimer.Instance.Setup();

            if (isOnCutScene && DataManager.Instance.PlayerData.userPlayTime == 1)
            {
                camTarget.gameObject.SetActive(true);
                cameraHandler.enabled = false;
                cutSceneSheep.gameObject.SetActive(true);
                mainCam.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
                OpenGateLeaveSheepOut(cutSceneSheep);

                SheepConfigData sheepConfigData = ConfigManager.Instance.GetSheepConfigByType(1);
                cutSceneSheep.InitSheep(sheepConfigData.Sheepvalue, sheepConfigData.Speed);
            }
            else
            {
                Destroy(camTarget.gameObject);
                Destroy(camOverrall.gameObject);
                Destroy(camCart.transform.parent.gameObject);

                Destroy(mainCam.GetComponent<Cinemachine.CinemachineBrain>());

                cameraHandler.enabled = true;
                cutSceneSheep.gameObject.SetActive(false);

                if (DataManager.Instance.PlayerData.userPlayTime == 1)
                {
                    int initSheepCount = 1;
                    for (int i = 0; i < initSheepCount; ++i)
                    {
                        SheepController sheepController = SheepFactory.Instance.CreateNewSheep(1);
                        GameController.Instance.PutSheepBackToCage(sheepController);
                        SimpleResourcesManager.Instance.ShowParticle("MergeFx", sheepController.transform.position, 1);
                    }
                }
            }
        });
    }


    void LoadSheep(List<SheepData> sheepDatas)
    {
        int sheepCount = sheepDatas.Count;
        for (int i = 0; i < sheepCount; ++i)
        {
            SheepController sheep = SheepFactory.Instance.CreateNewSheep(sheepDatas[i].sheepType);
            sheep.InitSheep(sheepDatas[i].sheepType);
            sheep.transform.position = sheepDatas[i].position;
            sheep.transform.localEulerAngles = sheepDatas[i].localEulerAngles;
            switch ((SheepController.SheepState)sheepDatas[i].sheepState)
            {
                case SheepController.SheepState.Idle:
                    currentSheepInCage++; 
                    break;
                case SheepController.SheepState.IsMovingOut:
                    sheep.MoveOutOfCage();
                    break;
                case SheepController.SheepState.Running:
                    sheep.transform.localEulerAngles = new Vector3(0, 90, 0);
                    sheep.transform.position = beginPos.position;
                    sheep.transform.position = sheep.transform.position  + UnityEngine.Random.Range(-3, 0) * sheep.transform.forward;
                    sheep.DOMoveOnPath();
                    break;
                case SheepController.SheepState.Jumping:
                    sheep.DoJumpOverFence();
                    break;
                case SheepController.SheepState.Picking:
                    var pos = sheep.transform.position;
                    pos.y = 1.8f;
                    sheep.transform.position = pos;
                    currentSheepInCage++;
                    break;
                default:
                    break;
            }
            activeSheeps.Add(sheep);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BoostTimer.Instance.ActiveBoost(BoostType.SheepSpeedUp);
            BoostSpeedOnce();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BoostTimer.Instance.ActiveBoost(BoostType.x2SheepValue);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BoostTimer.Instance.ActiveBoost(BoostType.SuperBox);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            BoostTimer.Instance.ActiveBoost(BoostType.AutoMerge);
        }

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
                    SheepController sheepController = SheepFactory.Instance.CreateNewSheep(1);
                    GameController.Instance.PutSheepBackToCage(sheepController);
                    SimpleResourcesManager.Instance.ShowParticle("MergeFx", sheepController.transform.position, 1);
                });
            }
        }
        else 
        {

            if (BoostTimer.Instance.IsAutoMerge)
            {
                AutoMergeSheep();
            }
            //Mouse down
            if (Input.GetMouseButtonDown(0))
            {
                Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;

                int farmerLayer = LayerMask.GetMask("Farmer");
                int sheepLayer = LayerMask.GetMask("Sheep");
                if (Physics.Raycast(camRay, out rayHit, float.MaxValue, farmerLayer))
                {
                    farmer.Waving();
                    BoostSpeedOnce();
                }
                else if (Physics.Raycast(camRay, out rayHit, float.MaxValue, sheepLayer))
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
                        if (!this.IsCageFull())
                        {
                            PutSheepBackToCage(sheepController);
                        }
                        else
                        {
                            //TODO: Show TextNofity
                        }
                        
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
                    int groundLayer = LayerMask.GetMask("Island");
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

                    int islandLayer = LayerMask.GetMask("Island");

                    int groundLayer = LayerMask.GetMask("Ground");

                    bool hitSheep = Physics.Raycast(camRay, out rayHit, float.MaxValue, sheepLayer);

                    if (hitSheep && rayHit.collider.GetComponent<SheepController>().SheepStateProp == SheepController.SheepState.Idle)
                    {
                            SheepController selectSheep = rayHit.collider.GetComponent<SheepController>();
                            if (selectSheep.SheepType < numOfSheepType && selectSheep.SheepStateProp == SheepController.SheepState.Idle && selectSheep.SheepType == pickingSheep.SheepType)
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
                    else if (Physics.Raycast(camRay, out rayHit, float.MaxValue, groundLayer))
                    {

                        var pos = pickingSheep.transform.position;
                        pos.y -= 0.5f;
                        pickingSheep.transform.position = pos;

                    }
                    else if (Physics.Raycast(camRay, out rayHit, float.MaxValue, islandLayer))
                    {
                        pickingSheep.transform.position = lastPickingPos;
                        OpenGateLeaveSheepOut(pickingSheep);
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

    private void AutoMergeSheep()
    {
        
    }

    void OpenGateLeaveSheepOut(SheepController sheepController)
    {
        currentSheepIsMovingOut++;
        currentSheepInCage--;
        LevelConfigData lvlConfig = ConfigManager.Instance.GetLevelConfigByLevel(DataManager.Instance.PlayerData.userLevel);
        txtLimitSheep.text = currentSheepInCage + "/" + lvlConfig.Maxsheep;
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

    //SheepController SpawnSheep()
    //{
    //    SheepController sheep = sheepControllersPool[currentSheepPoolCursor];

    //    SheepConfigData sheepConfigData = ConfigManager.Instance.GetSheepConfigByType(1);
    //    sheep.InitSheep(sheepConfigData.Sheepvalue, sheepConfigData.Speed);

    //    currentSheepPoolCursor++;
    //    if (currentSheepPoolCursor >= numOfSheep)
    //        currentSheepPoolCursor = 0;

    //    float x = UnityEngine.Random.Range(spawningBox.bounds.center.x - spawningBox.bounds.extents.x, spawningBox.bounds.center.x + spawningBox.bounds.extents.x);
    //    float z = UnityEngine.Random.Range(spawningBox.bounds.center.z - spawningBox.bounds.extents.z, spawningBox.bounds.center.z + spawningBox.bounds.extents.z);

    //    sheep.transform.position = new Vector3(x, 1.8f, z);

    //    sheep.transform.localEulerAngles = new Vector3(0, UnityEngine.Random.Range(0,360), 0);

    //    sheep.gameObject.SetActive(true);
    //    activeSheeps.Add(sheep);
    //    return sheep;
    //}

    public void PutSheepBackToCage(SheepController sheepController)
    {
        float x = UnityEngine.Random.Range(spawningBox.bounds.center.x - spawningBox.bounds.extents.x, spawningBox.bounds.center.x + spawningBox.bounds.extents.x);
        float z = UnityEngine.Random.Range(spawningBox.bounds.center.z - spawningBox.bounds.extents.z, spawningBox.bounds.center.z + spawningBox.bounds.extents.z);

        sheepController.transform.position = new Vector3(x, 1.8f, z);
        sheepController.ResetState();
        if (!activeSheeps.Contains(sheepController))
            activeSheeps.Add(sheepController);
        currentSheepInCage++;
        LevelConfigData lvlConfig = ConfigManager.Instance.GetLevelConfigByLevel(DataManager.Instance.PlayerData.userLevel);
        txtLimitSheep.text = currentSheepInCage + "/" + lvlConfig.Maxsheep;
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

    public void AddSheepValue(double value)
    {
        float doubleValue = BoostTimer.Instance.IsX2SheepValue ? 2 : 1 ;
        value *= 2;
        currentSheepValue += value;
        DataManager.Instance.PlayerData.userSheepCoin = currentSheepValue;

        TMPRichTextX txtMesh = coutingTextPool[currentTextPoolCursor];
        currentTextPoolCursor++;
        if (currentTextPoolCursor >= numOfCountingText)
            currentTextPoolCursor = 0;
       
        string content = string.Format("<sprite>sheepIcon</sprite>{0}", BigNumbers.BigNumber.ToShortString(currentSheepValue.ToString("F0"), 4));
        txtTopCountingText.SetText(content);

        if (BoostTimer.Instance.IsX2SheepValue)
        {
            content = string.Format("<sprite>sheepIcon</sprite><color=blue>{0}", BigNumbers.BigNumber.ToShortString(value.ToString("F0"), 4));
        }
        else
        {
            content = string.Format("<sprite>sheepIcon</sprite>{0}", BigNumbers.BigNumber.ToShortString(value.ToString("F0"), 4));
        }
        
        txtMesh.SetText(content);

        txtMesh.DOKill();
        txtMesh.TextMeshProUGUIComp.DOFade(1, 0);
        txtMesh.gameObject.SetActive(true);
        txtMesh.transform.position = sampleText.transform.position;

        txtMesh.transform.DOPath(coutingTextFlyWayPoint, 5.0f, PathType.CatmullRom);
        txtMesh.TextMeshProUGUIComp.DOFade(0, 5.0f).onComplete = () =>
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
        SheepConfigData sheepConfigData = ConfigManager.Instance.GetSheepConfigByType(nextSheepLevel);
        sheepEvo.InitSheep(sheepConfigData.Sheepvalue, sheepConfigData.Speed);

        sheepEvo.onMoveOutOfCageDone = CloseGate;
        sheepEvo.transform.position = pointSheep.transform.position;

        pickUpSheep.gameObject.SetActive(false);
        pointSheep.gameObject.SetActive(false);

        activeSheeps.Remove(pickUpSheep);
        activeSheeps.Remove(pointSheep);

        activeSheeps.Add(sheepEvo);

        currentSheepInCage--;
        LevelConfigData lvlConfig = ConfigManager.Instance.GetLevelConfigByLevel(DataManager.Instance.PlayerData.userLevel);
        txtLimitSheep.text = currentSheepInCage + "/" + lvlConfig.Maxsheep;
        UpdateLevel(pickUpSheep.SheepType);

        SimpleResourcesManager.Instance.ShowParticle("MergeFx", pointSheep.transform.position, 1);
    }


    private void UpdateLevel(int mergeSheep)
    {
        SheepConfigData sheepConfig = ConfigManager.Instance.GetSheepConfigByType(mergeSheep);
        DataManager.Instance.PlayerData.userExp += sheepConfig.Exp;
        LevelConfigData levelConfig = ConfigManager.Instance.GetLevelConfigByLevel(DataManager.Instance.PlayerData.userLevel);
        long maxExp = levelConfig.Maxexp;

        float progress = DataManager.Instance.PlayerData.userExp*1.0f / maxExp;
        Tweener tween = lvlProgress.DOFillAmount(progress, 0.5f);
        if (progress >= 1.0f)
        {
            tween.onComplete = () =>
            {
                DataManager.Instance.PlayerData.userLevel += 1;
                lvlProgress.fillAmount = 0.0f;
                DataManager.Instance.PlayerData.userExp = 0;
                txtLevelProgress.text = "Lv. " + DataManager.Instance.PlayerData.userLevel + "- <color=white> 0%";
                if (DataManager.Instance.PlayerData.userLevel >= 3)
                    SpawnTimer.Instance.Setup();
            };
        }
        else
        {
            txtLevelProgress.text = "Lv. " + DataManager.Instance.PlayerData.userLevel + "- <color=white>" + (progress* 100).ToString("F0") + "%";
        }




    }


    public bool IsCageFull()
    {
        LevelConfigData lvlConfig = ConfigManager.Instance.GetLevelConfigByLevel(DataManager.Instance.PlayerData.userLevel);
        return currentSheepInCage >= lvlConfig.Maxsheep;
    }

    public void SetLockCamera(bool isLock)
    {
        cameraHandler.enabled = isLock==false;
    }


    public List<SheepData> GetSavingSheepData()
    {
        int activeSheepCount = activeSheeps.Count;
        List<SheepData> res = new List<SheepData>();
         for (int i=0;i<activeSheepCount;++i)
        {
            SheepData data = new SheepData();
            data.sheepType = activeSheeps[i].SheepType;
            data.sheepState = (int)activeSheeps[i].SheepStateProp;
            data.position = activeSheeps[i].transform.position;
            data.localEulerAngles = activeSheeps[i].transform.localEulerAngles;
            res.Add(data);
        }
        return res;
    }

    public void UpdateSheepCoin()
    {
        currentSheepValue = DataManager.Instance.PlayerData.userSheepCoin;
        string content = string.Format("<sprite>sheepIcon</sprite>{0}", BigNumbers.BigNumber.ToShortString(currentSheepValue.ToString("F0"), 4));
        txtTopCountingText.SetText(content);
    }

    void BoostSpeedOnce()
    {
        int activeSheepCount = activeSheeps.Count;
        for (int i = 0; i < activeSheepCount; ++i)
        {
            if (activeSheeps[i].SheepStateProp != SheepController.SheepState.Idle ||
                activeSheeps[i].SheepStateProp != SheepController.SheepState.Picking)
            {
                activeSheeps[i].BoostSpeedOnce();
            }
        }
    }
}
