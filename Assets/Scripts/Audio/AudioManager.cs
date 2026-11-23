using UnityEngine;

namespace TrashRush.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public enum SFXType
        {
            Pickup,
            CorrectSort,
            IncorrectSort,
            ButtonClick,
            Victory,
            GameOver,
            Footstep
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip bgmClip;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private AudioClip correctSortClip;
        [SerializeField] private AudioClip incorrectSortClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip victoryClip;
        [SerializeField] private AudioClip gameOverClip;
        [SerializeField] private AudioClip footstepClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
                bgmSource.volume = 0.5f;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = 0.8f;
            }
        }

        private void Start()
        {
            if (bgmClip != null && !bgmSource.isPlaying)
            {
                bgmSource.clip = bgmClip;
                bgmSource.Play();
            }
        }

        public void SetClips(AudioClip bgm, AudioClip pickup, AudioClip correct, AudioClip incorrect, AudioClip btn, AudioClip vic, AudioClip go, AudioClip foot)
        {
            bgmClip = bgm;
            pickupClip = pickup;
            correctSortClip = correct;
            incorrectSortClip = incorrect;
            buttonClickClip = btn;
            victoryClip = vic;
            gameOverClip = go;
            footstepClip = foot;

            if (bgmSource != null && bgmClip != null)
            {
                bgmSource.clip = bgmClip;
                bgmSource.Play();
            }
        }

        public void PlaySFX(SFXType type)
        {
            if (sfxSource == null) return;

            AudioClip clip = null;
            switch (type)
            {
                case SFXType.Pickup: clip = pickupClip; break;
                case SFXType.CorrectSort: clip = correctSortClip; break;
                case SFXType.IncorrectSort: clip = incorrectSortClip; break;
                case SFXType.ButtonClick: clip = buttonClickClip; break;
                case SFXType.Victory: clip = victoryClip; break;
                case SFXType.GameOver: clip = gameOverClip; break;
                case SFXType.Footstep: clip = footstepClip; break;
            }

            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}
