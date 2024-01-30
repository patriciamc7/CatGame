using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LooseCondition : MonoBehaviour
{
    /// <summary> Loose fade when limit has reached. </summary>
    public GameObject loosePage;

    /// <summary> Loose text when limit has reached. </summary>
    public GameObject looseText;

    /// <summary> Audio that plays when game is over. </summary>
    public AudioSource audioIdle;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Cursor.visible = true;

        Scene s = SceneManager.GetSceneByName("SampleScene");
        List<GameObject> sceneObjects = s.GetRootGameObjects().ToList();

        GameObject GameRuler = sceneObjects.Where(v => v.name == "GameRuler").First();
        GameRuler.GetComponent<GameRuler>().enabled = false;

        GameObject gameCursor = sceneObjects.Where(v => v.name == "Cursor").First();
        gameCursor.GetComponent<CursorLogic>().enabled = false;

        Instantiate(loosePage);
        Instantiate(looseText);

        AudioSource audioIdleSource = Instantiate(audioIdle);
        audioIdleSource.Play();
    }
}
