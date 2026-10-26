using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource sfxSource;     // Sonidos cortos
    [SerializeField] private AudioSource bgmSource;     // Música de fondo en bucle
    [SerializeField] private AudioSource ambientSource; // Ambiente de agua en bucle (opcional)

    [Header("Efectos de Sonido (SFX)")]
    [SerializeField] private AudioClip trashSound;   // Sonido genérico de basura (si el objeto no tiene uno propio)
    [SerializeField] private AudioClip plantSound;   // arreglo planta/plantas
    [SerializeField] private AudioClip animalSound;  // efectos/curar animal
    [SerializeField] private AudioClip winSound;     // efectos/humedal completo
    [SerializeField] private AudioClip goalSound;    // efectos/destello logro

    [Header("Herramientas")]
    [SerializeField] private AudioClip pinzaSound;
    [SerializeField] private AudioClip palaSound;

    [Header("Música de Fondo (BGM)")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip ambientLoop;

    private AudioSource[] pool;
    private int poolIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // La música sigue al reiniciar con R
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Fuentes extra para poder recortar sonidos largos sin cortar los demás
        pool = new AudioSource[6];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = gameObject.AddComponent<AudioSource>();
            pool[i].playOnAwake = false;
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    // Reproduce un clip desde 'start' y lo apaga suavemente tras 'maxDuration' segundos (0 = completo)
    public void PlayClip(AudioClip clip, float start = 0f, float maxDuration = 0f, float volume = 1f)
    {
        if (clip == null || pool == null) return;
        AudioSource src = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;
        src.Stop();
        src.clip = clip;
        src.volume = volume;
        src.time = Mathf.Clamp(start, 0f, Mathf.Max(0f, clip.length - 0.05f));
        src.Play();
        if (maxDuration > 0f) StartCoroutine(FadeOut(src, clip, maxDuration));
    }

    private IEnumerator FadeOut(AudioSource src, AudioClip clip, float after)
    {
        yield return new WaitForSeconds(after);
        if (src.clip != clip) yield break;
        float v = src.volume;
        for (float t = 0; t < 0.4f; t += Time.deltaTime)
        {
            if (src.clip != clip) yield break;
            src.volume = Mathf.Lerp(v, 0f, t / 0.4f);
            yield return null;
        }
        if (src.clip == clip) src.Stop();
    }

    public void PlayTrash()  { if (trashSound != null && sfxSource != null) sfxSource.PlayOneShot(trashSound); }
    public void PlayPlant()  { PlayClip(plantSound, 0.15f, 3f); }
    public void PlayAnimal() { PlayClip(animalSound, 0f, 1.6f); }
    public void PlayWin()    { if (winSound != null && sfxSource != null) sfxSource.PlayOneShot(winSound); }
    public void PlayGoal()   { if (goalSound != null && sfxSource != null) sfxSource.PlayOneShot(goalSound); }
    public void PlayPinza()  { if (pinzaSound != null && sfxSource != null) sfxSource.PlayOneShot(pinzaSound); }
    public void PlayPala()   { PlayClip(palaSound, 0f, 2f); }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && bgmSource != null)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        if (ambientLoop != null && ambientSource != null)
        {
            ambientSource.clip = ambientLoop;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }
}
