using Unity.VisualScripting;
using UnityEngine;
public class SkinSelector : MonoBehaviour
{
    [Header("Personajes (Skins)")]
    [SerializeField] private GameObject skinMarisol;
    [SerializeField] private GameObject skinChamp;
    public GameObject menuSeleccion;

    void Start()
    {
        Time.timeScale = 0f;
        menuSeleccion.SetActive(true);
    }

    public void SeleccionarMarisol()
    {
        skinMarisol.SetActive(true);
        //skinChamp.SetActive(false);
        Destroy(skinChamp);
        Time.timeScale = 1f;
        menuSeleccion.SetActive(false);
    }

    public void SeleccionarChamp()
    {
        skinChamp.SetActive(true);
        //skinMarisol.SetActive(false);
        Destroy(skinMarisol);
        Time.timeScale = 1f;
        menuSeleccion.SetActive(false);
    }
}