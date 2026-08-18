using UnityEngine;
using UnityEngine.SceneManagement;

public class OtoLevelChange : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] string nextLevel;


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Huguinho"))
        {
            if (nextLevel != null)
                SceneManager.LoadScene(nextLevel);
            else
                SceneManager.LoadScene("Menu");
        }
    }
}
