using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] Baraja;
    public GameObject tapadera;
    public Text monedasTxt;
    public Text puntuacionTxt;
    public Text puntuacionLuigiTxt;
    public Button btnBarajear;
    public Button btnApostar;
    public Button btnHit;
    public Button btnHold;
    public int monedas;
    public int apuesta;
    public VideoPlayer luigiRender;
    public VideoClip Idle;
    public VideoClip Win;
    public VideoClip Lose;
    public VideoClip Barajeando;
    public AudioClip Wao;
    public AudioClip Conio;
    public AudioClip HereWeGo;

    private List<GameObject> cartasUsadas;
    [SerializeField]
    private int puntuacion = 0;
    [SerializeField]
    private int puntuacionLuigi = 0;
    private Vector2 currentCardPos;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        monedas = 10;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        monedasTxt.text = "" + monedas;
    }

    public void Barajear()
    {
        luigiRender.clip = Barajeando;
        luigiRender.isLooping = false;

        audioSource.clip = HereWeGo;
        audioSource.Play();

        StartCoroutine(BarajearCoroutine());
    }

    IEnumerator BarajearCoroutine()
    {
        yield return new WaitForSeconds(2.9f);
        cartasUsadas = new List<GameObject>();
        int Draw;

        for (int i = 0; i < 52; i++)
        {
            cartasUsadas.Add(Baraja[i]);
        }

        //Mis Cartas
        Draw = Random.Range(0, cartasUsadas.Count);
        var carta = Instantiate(cartasUsadas[Draw]);
        carta.GetComponent<Cartas>().esMio = true;
        carta.transform.position = new Vector2(-2, -4);
        cartasUsadas.RemoveAt(Draw);
        Draw = Random.Range(0, cartasUsadas.Count);
        carta = Instantiate(cartasUsadas[Draw]);
        carta.GetComponent<Cartas>().esMio = true;
        carta.transform.position = new Vector2(-1, -4);
        cartasUsadas.RemoveAt(Draw);

        //CARTAS DE LUIGI
        Draw = Random.Range(0, cartasUsadas.Count);
        carta = Instantiate(cartasUsadas[Draw]);
        carta.transform.position = new Vector2(-2, -2);
        cartasUsadas.RemoveAt(Draw);
        Draw = Random.Range(0, cartasUsadas.Count);
        carta = Instantiate(cartasUsadas[Draw]);
        carta.transform.position = new Vector2(-1, -2);
        cartasUsadas.RemoveAt(Draw);

        sumarPuntos();

        currentCardPos = new Vector2(0, -4);

        luigiRender.clip = Idle;
        luigiRender.isLooping = true;

        tapadera.SetActive(true);
    }

    public void HitMe()
    {
        int Draw = Random.Range(0, cartasUsadas.Count);
        var carta = Instantiate(cartasUsadas[Draw]);
        carta.GetComponent<Cartas>().esMio = true;
        carta.transform.position = currentCardPos;
        cartasUsadas.RemoveAt(Draw);
        currentCardPos.x++;
        sumarPuntos();
    }

    public void Hold()
    {
        StartCoroutine(HoldCoroutine());
    }

    IEnumerator HoldCoroutine()
    {
        currentCardPos = new Vector2(0, -2);

        for(int i = 0; i<10 && puntuacionLuigi < 17; i++)
        {
            yield return new WaitForSeconds(1.5f);
            int Draw = Random.Range(0, cartasUsadas.Count);
            var carta = Instantiate(cartasUsadas[Draw]);
            carta.transform.position = currentCardPos;
            cartasUsadas.RemoveAt(Draw);
            currentCardPos.x++;
            sumarPuntos();
        }

        tapadera.SetActive(false);
        Terminar();
    }

    public int sumarPuntos()
    {
        puntuacion = 0;
        puntuacionLuigi = 0;
        Cartas[] cartasEnJuego = FindObjectsOfType<Cartas>();
        for (int i = 0; i < cartasEnJuego.Length; i++)
        {
            if (cartasEnJuego[i].esMio)
            {
                puntuacion += cartasEnJuego[i].valor;
            }
            else
            {
                puntuacionLuigi += cartasEnJuego[i].valor;
            }
        }

        if (puntuacion > 21)
        {
            for(int i = 0; i < cartasEnJuego.Length; i++)
            {
                if(cartasEnJuego[i].valor == 11 && puntuacion + 11 > 21 && cartasEnJuego[i].esMio)
                {
                    puntuacion -= 10;
                }
            }
        }

        if (puntuacion > 21)
        {
            Perder();
            Debug.Log("PERDISTE");
            tapadera.SetActive(false);
            return 0;
        }
        else if (puntuacion == 21)
        {
            Ganar();
            Debug.Log("GANASTE");
            tapadera.SetActive(false);
            return 0;
        }

        if (puntuacionLuigi > 21)
        {
            for (int i = 0; i < cartasEnJuego.Length; i++)
            {
                if (cartasEnJuego[i].valor == 11 && puntuacionLuigi + 11 > 21 && !cartasEnJuego[i].esMio)
                {
                    puntuacionLuigi -= 10;
                }
            }
        }

        if (puntuacionLuigi > 21)
        {
            Ganar();
            Debug.Log("GANASTE");
            tapadera.SetActive(false);
            return 0;
        }
        else if (puntuacionLuigi == 21)
        {
            Perder();
            Debug.Log("PERDISTE");
            tapadera.SetActive(false);
            return 0;
        }
        return 0;
    }

    public void Apostar()
    {
        if (monedas > 0)
        {
            apuesta++;
            monedas--;
        }
    }

    public void Ganar()
    {
        puntuacionLuigiTxt.gameObject.SetActive(true);
        puntuacionLuigiTxt.text = "" + puntuacionLuigi;
        puntuacionTxt.gameObject.SetActive(true);
        puntuacionTxt.text = "" + puntuacion;

        btnHit.gameObject.SetActive(false);
        btnHold.gameObject.SetActive(false);

        luigiRender.clip = Win;
        luigiRender.isLooping = false;
        StartCoroutine(ReiniciarCoroutine());

        monedas += apuesta * 2;
        apuesta = 0;

        audioSource.clip = Wao;
        audioSource.Play();

        tapadera.SetActive(false);
    }

    IEnumerator ReiniciarCoroutine()
    {
        yield return new WaitForSeconds(3);
        luigiRender.clip = Idle;
        luigiRender.isLooping = true;
        btnBarajear.gameObject.SetActive(true);
        btnApostar.gameObject.SetActive(true);
        puntuacionLuigiTxt.gameObject.SetActive(false);
        puntuacionTxt.gameObject.SetActive(false);

        tapadera.SetActive(false);

        audioSource.clip = null;

        GameObject[] BorrarCartas = GameObject.FindGameObjectsWithTag("Carta");

        for (int i = 0; i < BorrarCartas.Length; i++)
        {
            Destroy(BorrarCartas[i]);
        }

        if(apuesta == 0 && monedas == 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void Perder()
    {
        puntuacionLuigiTxt.gameObject.SetActive(true); 
        puntuacionLuigiTxt.text = "" + puntuacionLuigi;
        puntuacionTxt.gameObject.SetActive(true);
        puntuacionTxt.text = "" + puntuacion;

        btnHit.gameObject.SetActive(false);
        btnHold.gameObject.SetActive(false);

        luigiRender.clip = Lose;
        luigiRender.isLooping = false;
        StartCoroutine(ReiniciarCoroutine());

        apuesta = 0;
        audioSource.clip = Conio;
        audioSource.Play();

        tapadera.SetActive(false);
    }

    void Terminar()
    {
        if(puntuacion <21 && puntuacionLuigi < 21)
        {
            if (puntuacion > puntuacionLuigi)
            {
                Ganar();
                Debug.Log("GANASTE");
            } else
            {
                Perder();
                Debug.Log("PERDISTE");
            }
        }


    }
}
