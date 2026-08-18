using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Huguinho"))
            SceneManager.LoadScene("Menu");
    }
}
