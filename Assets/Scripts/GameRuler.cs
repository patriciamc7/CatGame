using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRuler : MonoBehaviour
{
    /// <summary> Current Scene. </summary>
    private Scene s;

    /// <summary> List of current and next outcomes. </summary>
    private List<GameObject> mObjectList;

    /// <summary> Current outcomes. </summary>
    private GameObject currentObject;

    /// <summary> Current points. </summary>
    private int systemPoints;

    /// <summary> List with all the possible outcomes. </summary>
    public List<GameObject> mAllObjects;

    /// <summary> Max layer for the object to collide. </summary>
    public LayerMask maxLayer;

    /// <summary> Min layer for the object to collide. </summary>
    public LayerMask minLayer;

    /// <summary> Text to update the current points. </summary>
    public TMP_Text systemPointsText;

    /// <summary> Particle system of smoke when two objects collide. </summary>
    public ParticleSystem particleSmoke;

    /// <summary> Particle system of smoke when the two objects with max layer collides. </summary>
    public ParticleSystem particleFireworks;

    /// <summary> Audio that plays when the player does click. </summary>
    public AudioSource audioDrop;

    /// <summary> Audio that plays when two game objects join. </summary>
    public AudioSource audioJoin;

    /// <summary> Audio that plays when playing game. </summary>
    public AudioSource audioIdle;

    /// <summary> Audio that plays when collide two max layers. </summary>
    public AudioSource audioMaxWin;

    // Start is called before the first frame update
    private void Start()
    {
        AudioSource audioIdleSource = Instantiate(audioIdle);
        audioIdleSource.Play();
        systemPoints = 0;
        mObjectList = new List<GameObject>();
        ChoseNextObject();
        MoveObjectToCursor();

        s = SceneManager.GetSceneByName("SampleScene");
    }

    // Update is called once per frame
    private void Update()
    {
        CheckCollisions();

        if (Input.GetMouseButtonDown(0))
        {
            AudioSource audioDropSource = Instantiate(audioDrop);
            audioDropSource.Play();

            currentObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            currentObject.GetComponent<Collider2D>().enabled = true;

            if (mObjectList != null && mObjectList.Count > 0)
            {
                mObjectList.RemoveAt(0);
                ChoseNextObject();
            }
        }
        MoveObjectToCursor();
    }

    private void CheckCollisions()
    {
        List<GameObject> sceneObjects = s.GetRootGameObjects().ToList();

        //Layer boundaries
        sceneObjects = sceneObjects.Where(v => v.layer <= Mathf.Log(maxLayer, 2) && v.layer >= Mathf.Log(minLayer.value, 2)).ToList();

        if (sceneObjects != null)
        {
            for (int i = 0; i < sceneObjects.Count; i++)
            {
                if (sceneObjects[i] != null && currentObject != sceneObjects[i])
                {
                    Collider2D collider = sceneObjects[i].GetComponentInChildren<Collider2D>();
                    List<GameObject> collisionsObjects = new List<GameObject>(sceneObjects);
                    collisionsObjects.RemoveAt(i);
                    List<GameObject> layerObject = collisionsObjects.Where(v => v.layer == sceneObjects[i].layer).ToList();

                    if (collider.gameObject != null && layerObject != null)
                    {
                        for (int j = 0; j < layerObject.Count; j++)
                        {
                            if (currentObject == layerObject[j])
                            {
                                continue;
                            }
                            //Check if they are colliding
                            if (layerObject[j] != null && collider.gameObject.GetComponent<collisionManager>().CollisionObjects.Contains(layerObject[j]))
                            {
                                if (mAllObjects.Find(v => v.layer == layerObject[j].layer + 1) != null)
                                {
                                    if (sceneObjects.Find(v => v == collider.gameObject) != null)
                                    {
                                        //Update points
                                        UpdatePoints(layerObject[j].layer);

                                        Destroy(sceneObjects.Find(v => v == collider.gameObject));
                                        Destroy(sceneObjects.Find(v => v == layerObject[j]));
                                        layerObject[j] = Instantiate(mAllObjects.Find(v => v.layer == layerObject[j].layer + 1));
                                        layerObject[j].transform.position = collider.gameObject.transform.position;

                                        //Smoke animation
                                        AudioSource audioJoinSource = Instantiate(audioJoin);
                                        audioJoinSource.Play();
                                        PlayParticle(collider.gameObject.transform.position, particleSmoke);
                                    }
                                    return;
                                }
                                else if (layerObject[j].layer == Mathf.Log(maxLayer, 2))
                                {
                                    //Update points
                                    UpdatePoints(layerObject[j].layer);

                                    Destroy(sceneObjects.Find(v => v == collider.gameObject));
                                    Destroy(sceneObjects.Find(v => v == layerObject[j]));

                                    //Firework animation
                                    PlayParticle(collider.gameObject.transform.position, particleFireworks);

                                    AudioSource audioaudioMaxWinSource = Instantiate(audioMaxWin);
                                    audioaudioMaxWinSource.Play();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Play the particle animation when two objects collides
    /// </summary>
    private void PlayParticle(Vector3 mPosition, ParticleSystem mParticleSystem)
    {
        ParticleSystem pSystem = Instantiate(mParticleSystem);
        pSystem.transform.position = mPosition;
        pSystem.Play();
    }

    /// <summary>
    /// Calculate the total game puntuaction
    /// </summary>
    private void UpdatePoints(LayerMask mLayer)
    {
        switch (LayerMask.LayerToName(mLayer))
        {
            case "Brown":
                systemPoints += 1;
                break;
            case "Heart":
                systemPoints += 3;
                break;
            case "Pink":
                systemPoints += 7;
                break;
            case "Cute":
                systemPoints += 9;
                break;
            case "Black":
                systemPoints += 20;
                break;
            case "Ribbon":
                systemPoints *= 2;
                break;
        }
        systemPointsText.text = systemPoints.ToString();
    }

    /// <summary>
    /// We calculate with the same probability of outcome the next object
    /// </summary>
    private void ChoseNextObject()
    {
        mObjectList?.Add(mAllObjects[Random.Range(0, mAllObjects.Count - 1)]);
        currentObject = Instantiate(mObjectList[0]);
        currentObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        currentObject.GetComponent<Collider2D>().enabled = false;

        float currentPos = 3.2f + currentObject.transform.position.y;
        currentObject.transform.position = new Vector3(0, currentPos, 0);
    }

    /// <summary>
    /// Moves the object we are going to drop at the top of the cursor
    /// </summary>
    private void MoveObjectToCursor()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentObject.transform.position = new Vector3(worldPosition.x, currentObject.transform.position.y, 0);
    }
}
