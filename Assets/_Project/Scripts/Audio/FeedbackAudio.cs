using UnityEngine;
namespace Swift.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class FeedbackAudio : MonoBehaviour
    {
        public Swift.Game.GameManager Manager;
        AudioClip tone;
        AudioSource source;
        void Start()
        {
            source = GetComponent<AudioSource>(); source.spatialBlend = 0; source.volume = 0.12f;
            var samples = new float[4410];
            for (int i = 0; i < samples.Length; i++) samples[i] = Mathf.Sin(i * 2 * Mathf.PI * 660 / 44100) * (1 - i / (float)samples.Length);
            tone = AudioClip.Create("SWIFT generated feedback", samples.Length, 1, 44100, false); tone.SetData(samples, 0);
            if (Manager != null) Manager.Changed += Play;
        }
        void Play() { if (source != null && tone != null) source.PlayOneShot(tone); }
        void OnDestroy() { if (Manager != null) Manager.Changed -= Play; if (tone != null) Destroy(tone); }
    }
}
