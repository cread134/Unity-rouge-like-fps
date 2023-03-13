using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelCreatorV2 : MonoBehaviour
{
    //stuct time baby - this is for holding all rooms we have created
    [System.Serializable]
    public struct DoorPoint
    {
        public Vector3 doorPosition;
        public Vector3 facingVector;
        public bool forceDoor;
        public bool onMainPath;
        public List<GameObject> connectedTo;
    }
    [Space]
    public bool loadPlayer;
    public GameObject portalRoom;
    public bool drawRoomDebugBoxes;
    public int playerSceneIndex;
    public List<DoorPoint> doorPoints = new List<DoorPoint>();
    public NavMeshSurface n_meshSurface;
    public int numOfPortalPairsToMake;
    [Space]
    private GameObject lastRoom;
    public GameObject elevator;
    public GameObject endRoom;
    private GameObject endRoomInstance;


    [Space]

    public List<Light> sceneLights = new List<Light>();
    public bool generateDoors = false;
    public GameObject[] blocker;
    public GameObject[] arches;
    public GameObject[] doorBlocks;

    [Space]
    public List<GameObject> generatedDoors = new List<GameObject>();
    [Range(0, 1f)]
    public float doorChance;
    [Space]

    private List<Transform> pointsToChoose = new List<Transform>(); //these are to points to extrude from to create room
    public List<GameObject> createdRooms = new List<GameObject>();
    private List<GameObject> instancedBlockers = new List<GameObject>();

    public GameObject doorHolder;
    //get all points available
    
    public List<Transform> allPointsAvailable = new List<Transform>();
    List<GameObject> roomPortalInstances = new List<GameObject>();

    [Space]
    public bool generateOffShoots;
    public GameObject[] offShootRooms;
    public int offShootAmountMin;
    public int offshootAmountMax;
    public float offShootDensity;
    public LayerMask doorMask;
    [Space]

    private int roomsGenerated = 0;

    public int roomsToGenerate;
    public GameObject[] rooms;

    [HideInInspector]
    public float progress = 0f;
    public bool isDone;
    public NavMeshSurface nMeshSurface;  

    [Range(0, 10000)]
    public int testSeed;

    public bool randomizeTestSeed;


    List<GameObject> offshootsGenerated = new List<GameObject>();

    //for genereation
    [Header("RequiredStuff")]
    public GameObject originRoom;

    public LevelTemplate levelTemplate;
    public LayoutClass[] layouts; 

    public List<GameObject> roomToGenerateList = new List<GameObject>(); //list of all rooms to make we cycle down this list 

    private void Awake()
    {
        if(randomizeTestSeed == true)
        {
            testSeed = Random.Range(0, 10000);
        }
        StartGeneration(testSeed);
    }
    
    public void StartGeneration(int seed)
    {
        Random.InitState(seed);
        GenerateOrder();
        isDone = false;
        doorPoints = new List<DoorPoint>();
        GenerateMainPath();
    }

    public void Update()
    {
        //YEE
        if (allPointsAvailable.Count > 1)
        {
            foreach (Transform trans in allPointsAvailable)
            {
                Debug.DrawLine(trans.position, new Vector3(trans.position.x, trans.position.y + 25f, trans.position.z));

            }
        }
    }

    private void GenerateMainPath()
    {
        allPointsAvailable.Clear();

        for (int i = 0; i < roomToGenerateList.Count; i++)
        {
            if (i == roomToGenerateList.Count - 1) {
                if (GenerateRoom(roomToGenerateList[i], true, true, true) == false)
                {
                    break;
                }
            }
            else
            {
                if (GenerateRoom(roomToGenerateList[i], false, true, true) == false)
                {
                    break;
                }
            }
        }

        //generate "spice rooms"

        foreach (GameObject g in createdRooms)
        {
            RoomClass r_class = g.GetComponent<RoomClass>();
            foreach (Transform trans in r_class.connectionpoints)
            {
                if (r_class.connectedPoints.Contains(trans) == false)
                {
                    allPointsAvailable.Add(trans);
                }
            }
        }

 

        //generateblockers
        GenerateBlockers();
        //generateDoors
        if(generateDoors == true)
        {
            GenerateDoors();
        }
    }

    private void GenerateOffShoots(List<Transform> pointsToUse)
    {
        if (generateOffShoots == true)
        {
         

            int amountToMake = (int)Mathf.CeilToInt(pointsToUse.Count * (1 / offShootDensity));

            List<Transform> newChoosepoints = new List<Transform>();

            for (int i = 0; i < amountToMake; i++)
            {
                Transform toUse = pointsToUse[Random.Range(0, pointsToUse.Count)];

                GameObject roomtoCreate = offShootRooms[Random.Range(0, offShootRooms.Length)];
                RoomClass toMakeRClass = roomtoCreate.GetComponent<RoomClass>();

                //create it
                //1 Create room
                GameObject roomInstance = Instantiate(roomtoCreate, new Vector3(50, 0, 50), Quaternion.identity); // creates room far away for convience sake
                RoomClass newRoomClass = roomInstance.GetComponent<RoomClass>();//gets script on new room
                Transform newRoomPoint = newRoomClass.connectionpoints[Random.Range(0, toMakeRClass.connectionpoints.Length)]; //finding point as we align to any point on the new room

                //2Align The position of the room
                Align(newRoomPoint, toUse); // we align the room to position

                bool isOverlapping = newRoomClass.TestForOverlap();

                //check here 
                if (isOverlapping) // checkforcollison
                {
                    Destroy(roomInstance);
                }
                else
                {
                    //add to door points
                    DoorPoint pointToAdd = new DoorPoint();
                    pointToAdd.doorPosition = toUse.position;
                    pointToAdd.facingVector = toUse.forward;
                    if (lastRoom.GetComponent<RoomClass>().doForceDoors == true || newRoomClass.doForceDoors == true)
                    {
                        pointToAdd.forceDoor = true;
                    }
                    doorPoints.Add(pointToAdd);

                    //setting up variables
                    pointsToUse.Remove(toUse);
                    createdRooms.Add(roomInstance);
                    offshootsGenerated.Add(roomInstance);
                    newRoomClass.connectedToRooms.Add(lastRoom);
                    lastRoom.GetComponent<RoomClass>().connectedToRooms.Add(roomInstance);

                    lastRoom = roomInstance;
                    // RoomClass lastpointClass = toUse.transform.parent.transform.parent.GetComponent<RoomClass>();

                    //check points for validity
                    foreach (Transform trans in newRoomClass.connectionpoints)//adds all the connections of newly created room
                    {
                        if (Vector3.Distance(trans.position, toUse.position) > 1f)
                        {
                            allPointsAvailable.Add(trans);
                            newChoosepoints.Add(trans);
                        }
                        else
                        {
                            newRoomClass.connectedPoints.Add(trans);
                            if (toUse.parent.transform.parent.GetComponent<RoomClass>().connectedPoints.Contains(toUse) == false)//adds the point to lsit of conenctions in room
                            {
                                toUse.parent.transform.parent.GetComponent<RoomClass>().connectedPoints.Add(toUse);
                            }
                        }
                    }
                }

            }
            //generate the restf
            int offShootAmount = Random.Range(offShootAmountMin, offshootAmountMax);
            for (int i = 0; i < offshootAmountMax; i++)
            {
                Transform toUse = newChoosepoints[Random.Range(0, newChoosepoints.Count)];

                GameObject roomtoCreate = offShootRooms[Random.Range(0, offShootRooms.Length)];
                RoomClass toMakeRClass = roomtoCreate.GetComponent<RoomClass>();

                //create it
                //1 Create room
                GameObject roomInstance = Instantiate(roomtoCreate, new Vector3(50, 0, 50), Quaternion.identity); // creates room far away for convience sake
                RoomClass newRoomClass = roomInstance.GetComponent<RoomClass>();//gets script on new room
                Transform newRoomPoint = newRoomClass.connectionpoints[Random.Range(0, toMakeRClass.connectionpoints.Length)]; //finding point as we align to any point on the new room

                //2Align The position of the room
                Align(newRoomPoint, toUse); // we align the room to position

                bool isOverlapping = newRoomClass.TestForOverlap();

                //check here 
                if (isOverlapping) // checkforcollison
                {
                    Destroy(roomInstance);
                }
                else
                {
                    //add to door points
                    DoorPoint pointToAdd = new DoorPoint();
                    pointToAdd.doorPosition = toUse.position;
                    pointToAdd.facingVector = toUse.forward;
                    if (lastRoom.GetComponent<RoomClass>().doForceDoors == true || newRoomClass.doForceDoors == true)
                    {
                        pointToAdd.forceDoor = true;
                    }
                    doorPoints.Add(pointToAdd);

                    allPointsAvailable.Remove(toUse);
                    newChoosepoints.Remove(toUse);
                    createdRooms.Add(roomInstance);
                    offshootsGenerated.Add(roomInstance);
                    newRoomClass.connectedToRooms.Add(lastRoom);
                    lastRoom.GetComponent<RoomClass>().connectedToRooms.Add(roomInstance);

                    lastRoom = roomInstance;
                    // RoomClass lastpointClass = toUse.transform.parent.transform.parent.GetComponent<RoomClass>();

                    //check points for validity
                    foreach (Transform trans in newRoomClass.connectionpoints)//adds all the connections of newly created room
                    {
                        if (Vector3.Distance(trans.position, toUse.position) > 1f)
                        {
                            allPointsAvailable.Add(trans);
                            newChoosepoints.Add(trans);
                        }
                        else
                        {
                            newRoomClass.connectedPoints.Add(trans);
                            if (toUse.parent.transform.parent.GetComponent<RoomClass>().connectedPoints.Contains(toUse) == false)//adds the point to lsit of conenctions in room
                            {
                                toUse.parent.transform.parent.GetComponent<RoomClass>().connectedPoints.Add(toUse);
                            }
                        }
                    }
                }
            }

            if (numOfPortalPairsToMake > 0)
            {           
                //find all the things
                List<Transform> choosablePoints = new List<Transform>();
                foreach (GameObject roomToCheck in createdRooms)
                {
                    if(roomToCheck != endRoomInstance)//make sure we dont teleport to the end instantly(that would be dumb
                    {
                        RoomClass r_class = roomToCheck.GetComponent<RoomClass>();
                        List<Transform> ranPointlist = new List<Transform>();
                        foreach (Transform trans in r_class.connectionpoints)
                        {
                            
                            if(r_class.connectedPoints.Contains(trans) == false)//check if not connection to anotehr room
                            {
                                ranPointlist.Add(trans);
                            }
                        }
                     //   Debug.Log("length" + choosablePoints.Count);
                        if (ranPointlist.Count > 0)
                        {
                            Shuffle(ranPointlist.ToArray());
                            choosablePoints.Add(ranPointlist[0]); // add point so now we havve point from every room 
                        }
                    }
                   // RoomClass transClass = trans.GetComponent<RoomClass>();
                    
                }

                //make pairs for portals
                pointsToChoose = choosablePoints;
                Debug.Log(pointsToChoose.Count);
                int portalPointsNum = numOfPortalPairsToMake * 2;
                int portalsMade = 0;
                for (int i = 0; i < portalPointsNum; i++)
                {
                    if(GenerateRoom(portalRoom, false, false, false) == true)
                    {        

                        portalsMade++;

                        if(portalsMade == portalPointsNum)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("tried portal");
                    }
                }

                //link portals
                bool iseven = portalsMade % 2 == 0;
                if (iseven == false)//check even num of portals
                {
                    portalsMade--;
                    Destroy(roomPortalInstances[roomPortalInstances.Count - 1]);
                }
                //link the portals together
                for (int i = 0; i < portalsMade / 2; i ++)
                {
                    GameObject portalToUse = roomPortalInstances[i];
                    GameObject portalBefore = roomPortalInstances[(roomPortalInstances.Count - 1) - i];
                    PortalScript p_script = portalToUse.GetComponent<PortalReference>().portalRef;
                    PortalScript beforeP_Script = portalBefore.GetComponent<PortalReference>().portalRef;
                    p_script.linkedPortal = beforeP_Script;
                    beforeP_Script.linkedPortal = p_script;
                }
            }
        }
    }

    private bool GenerateRoom(GameObject toUse, bool isEndRoom, bool allowBlockageAdjust, bool updatePoints)
    {
        List<GameObject> roomsInstanced = new List<GameObject>();
        if (lastRoom != null) // check possible 
        {
            foreach (Transform point in pointsToChoose)
            {
                GameObject roomToCreate = toUse;
                RoomClass toMakeRClass = roomToCreate.GetComponent<RoomClass>(); // get the script for the room from selected room to create

                Transform pointToCreate = point; //point we are aligning to

                //note on order of creation 1:make room 2: RotateRoom 3:AlignRoom 4:CheckForOverlaps

                //1 Create room
                GameObject roomInstance = Instantiate(roomToCreate, new Vector3(50, 0, 50), Quaternion.identity); // creates room far away for convience sake

                RoomClass newRoomClass = roomInstance.GetComponent<RoomClass>();//gets script on new room

                Transform newRoomPoint = newRoomClass.connectionpoints[Random.Range(0, toMakeRClass.connectionpoints.Length)]; //finding point as we align to any point on the new room

                //2Align The position of the room
                Align(newRoomPoint, pointToCreate); // we align the room to position

                //3 we check the room is now overlapping
                bool isOverlapping = newRoomClass.TestForOverlap();

                //check here 
                if (isOverlapping) // checkforcollison
                {
                    if (point == pointsToChoose[pointsToChoose.Count - 1] && allowBlockageAdjust == true) // check if this is last object
                    {
             
                        GameObject roomBeforeLast = createdRooms[createdRooms.Count - 2];

                        Debug.DrawLine(roomBeforeLast.transform.position, lastRoom.transform.position, Color.blue);
                        Debug.DrawLine(lastRoom.transform.position, roomInstance.transform.position);
                        Debug.DrawLine(new Vector3(roomInstance.transform.position.x, roomInstance.transform.position.y + 10, roomInstance.transform.position.z), roomInstance.transform.position, Color.green);

                        Debug.Log("fullCollison");
                        RoomClass lastRoomClass = lastRoom.GetComponent<RoomClass>();

                        //removing from master list
                        createdRooms.Remove(roomInstance);
                        createdRooms.Remove(lastRoom);

                        //handling removing last room

                        foreach (Transform trans in lastRoomClass.connectionpoints)
                        {
                            if (allPointsAvailable.Contains(trans))
                            {
                                allPointsAvailable.Remove(trans);
                            }
                        }

                        //handling room before last connections
                        RoomClass roomBeforeLastClass = roomBeforeLast.GetComponent<RoomClass>();
                        Vector3 lastRoomConnectedToPos = Vector3.zero; //where the last room connects to the one before

                        foreach (Transform trans in roomBeforeLastClass.connectedPoints.ToArray())//handle the conncetio
                        {
                            foreach (Transform t in lastRoomClass.connectedPoints)
                            {
                                if (Vector3.Distance(t.position, trans.position) < 1f)
                                {
                                    Debug.Log("found");
                                    //  Debug.Log(trans.position);
                                    allPointsAvailable.Add(trans);
                                    lastRoomConnectedToPos = trans.position;
                                    roomBeforeLastClass.connectedPoints.Remove(trans);
                                }
                            }

                        }
                        //  Debug.Log(lastRoomConnectedToPos);

                        foreach (GameObject g in roomBeforeLastClass.connectedToRooms.ToArray())
                        {
                            if (g == lastRoom)
                            {
                                roomBeforeLastClass.connectedToRooms.Remove(lastRoom);
                            }
                        }
                        //remove door points
                        foreach (DoorPoint d_point in doorPoints.ToArray())
                        {
                            if (Vector3.Distance(d_point.doorPosition, lastRoomConnectedToPos) < 1f)//use distance so there is margin for floating point erros
                            {
                                doorPoints.Remove(d_point);
                            }
                        }

                        //destroying instances
                        Destroy(lastRoom);

                        Destroy(roomInstance);
                        lastRoom = roomBeforeLast;


                        List<GameObject> roomCollection = new List<GameObject>();
                        int startIndex = roomsGenerated;
                        roomCollection.Add(elevator);

                        // roomCollection.Add(roomToGenerateList[startIndex - 1]);
                        roomCollection.Add(roomToCreate);


                        Debug.Log(startIndex + " start index");

                        for (int i = 0; i < roomToGenerateList.Count; i++)
                        {
                            if (i >= startIndex)
                            {
                                roomCollection.Add(roomToGenerateList[i]);
                            }
                        }


                        pointsToChoose = new List<Transform>();
                        foreach (Transform trans in roomBeforeLastClass.connectionpoints)
                        {
                            if (roomBeforeLastClass.connectedPoints.Contains(trans) == false)
                            {
                                pointsToChoose.Add(trans);
                            }
                        }

                        //    GenerateRoom(elevator);
                        //  GenerateRoom(toUse);
                        GenerateOnBlockage(roomCollection);

                        return false;
                        //Instantiate(elevator, lastRoom.transform.position, lastRoom.transform.rotation);
                        //  ResetGeneration();         

                    }
                    else
                    {
                        Destroy(roomInstance);
                    }

                }
                else//if we didn't collide
                {
                    //add to door points
                    DoorPoint pointToAdd = new DoorPoint();
                    pointToAdd.doorPosition = pointToCreate.position;
                    pointToAdd.facingVector = pointToCreate.forward;
                    if (lastRoom.GetComponent<RoomClass>().doForceDoors == true || newRoomClass.doForceDoors == true)
                    {
                        pointToAdd.forceDoor = true;
                    }
                    doorPoints.Add(pointToAdd);

                    roomsInstanced.Add(toUse);

                    createdRooms.Add(roomInstance);
                    newRoomClass.connectedToRooms.Add(lastRoom);
                    lastRoom.GetComponent<RoomClass>().connectedToRooms.Add(roomInstance);

                    lastRoom = roomInstance;
                    RoomClass lastpointClass = pointToCreate.transform.parent.transform.parent.GetComponent<RoomClass>();

                    //check points for validity
                    if (updatePoints == true)
                    {
                        pointsToChoose.Clear();

                        pointsToChoose = new List<Transform>();
                    }

                        foreach (Transform trans in newRoomClass.connectionpoints)//adds all the connections of newly created room
                        {
                            if (Vector3.Distance(trans.position, pointToCreate.position) > 1f)
                            {
                            if (updatePoints == true)
                            {
                                pointsToChoose.Add(trans);
                            }
                            }
                            else
                            {
                                newRoomClass.connectedPoints.Add(trans);
                                if (pointToCreate.parent.transform.parent.GetComponent<RoomClass>().connectedPoints.Contains(pointToCreate) == false)//adds the point to lsit of conenctions in room
                                {
                                    pointToCreate.parent.transform.parent.GetComponent<RoomClass>().connectedPoints.Add(pointToCreate);
                                }

                            }
                        }
                        Shuffle(pointsToChoose.ToArray()); // shuffle the values
                    
                    roomsGenerated++; //add to the amounts of rooms create
                    
                    if(roomToCreate == portalRoom)//forwhen making portals
                    {
                        roomPortalInstances.Add(roomInstance);
                    }
                    
                    if (isEndRoom)//check last room to generate
                    {
                        Debug.Log("ended!", newRoomClass);
                        endRoomInstance = roomInstance;
                        //generate offshoots     
                        AfterPathGen();
                    }

                    break;
                }
            }


            progress = Mathf.Clamp((roomsGenerated / roomsToGenerate) * 100f, 0f, 100f);
        }
        else
        {
            //1 Create room
            GameObject roomInstance = Instantiate(originRoom, Vector3.zero, Quaternion.identity); // creates room far away for convience sake
            RoomClass newRoomClass = roomInstance.GetComponent<RoomClass>();//gets script on new room
            createdRooms.Add(roomInstance);
            lastRoom = roomInstance;


            foreach (Transform trans in newRoomClass.connectionpoints)//adds all the connections of newly created room
            {
                pointsToChoose.Add(trans);
            }
        }

        return true;
    }

    private void AfterPathGen()
    {
        //generate "spice rooms"
        foreach (GameObject g in createdRooms)
        {
            if (g != endRoomInstance)
            {
                RoomClass r_class = g.GetComponent<RoomClass>();
                foreach (Transform trans in r_class.connectionpoints)
                {
                    if (r_class.connectedPoints.Contains(trans) == false)
                    {
                        allPointsAvailable.Add(trans);
                    }
                }
            }
        }
        if (generateOffShoots == true)
        {
            GenerateOffShoots(allPointsAvailable);

        }
        allPointsAvailable.Clear();
        foreach (GameObject g in createdRooms)//dunno why it works but it does
        {
            //for ease of eyes
            //for ease of eyes
            if (drawRoomDebugBoxes == false)
            {
                g.GetComponent<RoomClass>().doDrawGizmos = false;
            }

            if (g != endRoomInstance)
            {
                RoomClass r_class = g.GetComponent<RoomClass>();
                foreach (Transform trans in r_class.connectionpoints)
                {
                    if (r_class.connectedPoints.Contains(trans) == false)
                    {
                        allPointsAvailable.Add(trans);
                    }
                }
            }
            foreach (Light roomslights in g.GetComponent<RoomClass>().roomlights)
            {
                sceneLights.Add(roomslights);
            }
        }
        //generateblockers
        GenerateBlockers();
        //generateDoors
        if (generateDoors == true)
        {
            GenerateDoors();
        }

        if (loadPlayer == true)
        {
            CreatePlayer();
        }
        n_meshSurface.BuildNavMesh();
        
    }

    public void CreatePlayer()
    {
        SceneManager.LoadSceneAsync(playerSceneIndex, LoadSceneMode.Additive);
    }

    private void GenerateOnBlockage(List<GameObject> toGenerate)
    {
        Debug.Log(lastRoom.name);
        foreach (GameObject g in toGenerate)
        {
            Debug.Log(g.name);
        }

        for (int i = 0; i < toGenerate.Count; i++)
        {
            if (i == toGenerate.Count - 1)
            {
                if (GenerateRoom(toGenerate[i], true, true, true) == false)
                {
                    return;
                }
            }
            else
            {
                if (GenerateRoom(toGenerate[i], false, false, true) == false)
                {
                    if (i == 0)
                    {
                        //complete reset, we have gotten into a corner ay
                        Debug.LogError("completly blocked");
                        RoomClass i_Room = toGenerate[0].GetComponent<RoomClass>();
                        RoomClass.RoomType r_type = i_Room.thisRoomType;

                        //we choose a random thing and just keep generatin yeahh yeah
                        bool succesful = false; //this probs gonnacrash my pc yeah
                        while (succesful == false)// fuck this makes my asshole clench
                        {
                            //choosewhich to generate
                            GameObject roomToG = null;
            
                            switch (r_type)
                            {
                                case RoomClass.RoomType.BossRoom:
                                    roomToG = levelTemplate.bossRooms[Random.Range(0, levelTemplate.bossRooms.Length)];
                                    break;

                                case RoomClass.RoomType.ChestRoom:
                                    roomToG = levelTemplate.chestRooms[Random.Range(0, levelTemplate.chestRooms.Length)];
                                    break;

                                case RoomClass.RoomType.Connector:
                                    roomToG = levelTemplate.connectorRooms[Random.Range(0, levelTemplate.connectorRooms.Length)];
                                    break;

                                case RoomClass.RoomType.StandardRoom:
                                    roomToG = levelTemplate.standardRooms[Random.Range(0, levelTemplate.standardRooms.Length)];
                                    break;

                                case RoomClass.RoomType.StoreRoom:
                                    roomToG = levelTemplate.shopRooms[Random.Range(0, levelTemplate.shopRooms.Length)];
                                    break;
                            }
                  
                            if (GenerateRoom(roomToG, false, false, true) == true)
                            {
                                succesful = true;
                            }
                        }

                        return;
                        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    else
                    {
                        return;
                    }

                }
            }

        }

    }

    private void GenerateOrder()
    {
        LayoutClass layoutToUse = layouts[Random.Range(0, layouts.Length - 1)];
        roomToGenerateList = new List<GameObject>();

        foreach (LayoutClass.Room room in layoutToUse.rooms)
        {
            GameObject roomToAdd = null;
            switch (room.roomType)
            {
                case LayoutClass.RoomType.BossRoom:
                    roomToAdd = levelTemplate.bossRooms[Random.Range(0, levelTemplate.bossRooms.Length)];
                    break;

                case LayoutClass.RoomType.ChestRoom:
                    roomToAdd = levelTemplate.chestRooms[Random.Range(0, levelTemplate.chestRooms.Length)];
                    break;

                case LayoutClass.RoomType.Connector:
                    roomToAdd = levelTemplate.connectorRooms[Random.Range(0, levelTemplate.connectorRooms.Length)];
                    break;

                case LayoutClass.RoomType.StandardRoom:
                    roomToAdd = levelTemplate.standardRooms[Random.Range(0, levelTemplate.standardRooms.Length)];
                    break;

                case LayoutClass.RoomType.StoreRoom:
                    roomToAdd = levelTemplate.shopRooms[Random.Range(0, levelTemplate.shopRooms.Length)];
                    break;
            }

            roomToGenerateList.Add(roomToAdd);
        }

        roomToGenerateList.Add(endRoom);
    }

    private void GenerateDoors()
    {
        //create all the doors
        foreach (DoorPoint d_point in doorPoints)
        {
            //determine if arch or door
            bool doDoor = false;
            if(d_point.forceDoor == true)
            {
                doDoor = true;
            }
            else
            {
                //randomly determine usage of door
                float chance = Random.Range(0, 1f);
                if(chance >= doorChance)
                {
                    doDoor = true;
                }
                else
                {
                    doDoor = false;
                }
            }

            //create the stuff

            GameObject toCreate = doorBlocks[Random.Range(0, doorBlocks.Length)];
            if (doDoor == false)
            {
                toCreate = arches[Random.Range(0, doorBlocks.Length)];
            }

            //instantiationtime
            if (Physics.CheckBox(new Vector3(d_point.doorPosition.x, d_point.doorPosition.y + 5f, d_point.doorPosition.z), Vector3.one, Quaternion.identity,doorMask) == false)
            {
                GameObject d_instance = Instantiate(toCreate, d_point.doorPosition, Quaternion.LookRotation(-d_point.facingVector), doorHolder.transform);
                generatedDoors.Add(d_instance);
            }
        }
    }
    private void GenerateBlockers()
    {
        List<Transform> blockerPoints = new List<Transform>();
        foreach (GameObject g in createdRooms)
        {
            RoomClass r_class = g.GetComponent<RoomClass>();
            foreach (Transform trans in r_class.connectionpoints)
            {
                if (allPointsAvailable.Contains(trans) == true)
                {
                    blockerPoints.Add(trans);
                }
            }
        }

        //generateBlockers
        foreach (Transform createPoint in blockerPoints)
        {
            GameObject blockerInstance = Instantiate(blocker[Random.Range(0, blocker.Length)], createPoint.position, Quaternion.LookRotation(-createPoint.forward));
            instancedBlockers.Add(blockerInstance);
        }
        
    }

    public void Align(Transform fromTransform, Transform toTransform)
    {
        //establishing values
        Transform fromParent = fromTransform.parent.transform.parent;
        Transform pivot = fromTransform.parent;


        //align pivot to point for rotation
        float pivotDistance = Vector3.Distance(fromTransform.position, pivot.position);
        Vector3 toPosition = fromTransform.position - fromTransform.forward * pivotDistance;

       // Debug.DrawLine(fromParent.position, toPosition, Color.cyan, 100f);

        Vector3 savedPos = fromParent.position;
        fromParent.position = toPosition;
        pivot.position = savedPos;

        //align to point across axis
        Vector3 pointTogo = toTransform.position + toTransform.forward * pivotDistance * 2;
        fromParent.position = pointTogo;

        //for finding angle
        float distanceFrom_To = Vector3.Distance(fromTransform.position, toTransform.position);//a (recaulculates position due to pibotChanging
        float distanceFrom_Parent = Vector3.Distance(fromTransform.position, fromParent.position);//b
        float distanceTo_Parent = Vector3.Distance(fromParent.position, toTransform.position); //c

        //debug
        #region 
        //debug
        //Debug.DrawLine(fromTransform.position, toTransform.position, Color.black, 100f);
       // Debug.DrawLine(fromTransform.position, fromParent.position, Color.yellow, 100f);
      //  Debug.DrawLine(toTransform.position, fromParent.position, Color.red, 100f); //important line

      //  Debug.DrawLine(fromTransform.position, fromTransform.position + fromTransform.forward * 1, Color.green, 100f);

        #endregion

        //rotate

        Vector3 dir = (toTransform.position - fromParent.position).normalized;
        Vector3 dir2 = (fromTransform.position - fromParent.position).normalized;
        float radNum = Vector3.Angle(dir, dir2);
      //  Debug.Log(radNum);

        //testdirToRotate
        fromParent.rotation = Quaternion.AngleAxis(1, Vector3.up); // find which is the right way to turn

        if (Vector3.Distance(fromTransform.position, toTransform.position) > distanceFrom_To)
        {
            radNum *= -1f;
        }

        fromParent.rotation = Quaternion.AngleAxis(-1, Vector3.up); // find which is the right way to turn
        //dorotate
        fromParent.rotation = Quaternion.AngleAxis(radNum, Vector3.up); //we must determine whether to add or subtract

        // connect through offset
        float xOffset = fromTransform.position.x - toTransform.position.x; //finds offset
        float zOffset = fromTransform.position.z - toTransform.position.z;

        fromParent.position = new Vector3(fromParent.position.x - xOffset, fromParent.position.y, fromParent.position.z - zOffset);
    }

    private void ResetGeneration()
    { 
        foreach (GameObject g in createdRooms)
        {
            Destroy(g);
        }

        foreach (GameObject g in generatedDoors)
        {
            Destroy(g);
        }
        generatedDoors = new List<GameObject>();

        lastRoom = null;
        doorPoints.Clear();
        roomsGenerated = 0;
        allPointsAvailable.Clear();
        pointsToChoose = new List<Transform>();
        createdRooms = new List<GameObject>();
    }

    private void CompleteGeneration()
    {
        if (nMeshSurface != null)
        {
            nMeshSurface.BuildNavMesh();
        }
        
        isDone = true;
    }

    void Shuffle(Transform[] points)
    {
        List<int> createIndexList = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            createIndexList.Add(i);
        }
     

        int[] a = createIndexList.ToArray();
        // Loops through array
        for (int i = a.Length - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overright when we swap the values
            int temp = a[i];

            // Swap the new and old values
            a[i] = a[rnd];
            a[rnd] = temp;
        }

        List<Transform> newPoints = new List<Transform>();

        for (int i = 0; i < points.Length; i++)
        {
            Transform target = points[a[i]]; //gets transform with index of "a" in transform
            newPoints.Add(target);
        }

        pointsToChoose = newPoints;
    }

    private void OnDrawGizmosSelected()
    {
        if(offshootsGenerated.Count > 0)
        {
            foreach (GameObject g in offshootsGenerated)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(new Vector3(g.transform.position.x, g.transform.position.y + 5, g.transform.position.z), new Vector3(15, 15, 15));
            }
        }

        if(doorPoints.Count > 0)
        {
            foreach (DoorPoint d in doorPoints)
            {

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(d.doorPosition, new Vector3(10, 10, 10));
            }
        }
    }
}
