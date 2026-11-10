using UnityEngine;
using UnityEngine.Audio;
namespace Swift.Audio
{
    public enum AudioCategory { UI, Objects, Alerts, Ambience, Music }
    public enum AudioCue { Confirm, Unavailable, NewAlert, Dossier, Evidence, Defense, Teleport, TurnEnd, PhoneRing, PhoneAnswer, PhoneEnd, PageTurn, Chair, Printer }
    [CreateAssetMenu(menuName="SWIFT/Audio/Cue library")]
    public sealed class AudioCueLibrary : ScriptableObject
    {
        [Header("Clips finales pendientes; los tonos TEMP son provisionales")]
        public bool UseTemporaryTones=true;
        public AudioClip Confirmation,Unavailable,NewAlert,Dossier,Evidence,Defense,Teleport,TurnEnd;
        [Header("Herramientas de escritorio: clips opcionales pendientes; sin tonos finales inventados")]
        public AudioClip PhoneRing,PhoneAnswer,PhoneEnd,PageTurn,Chair,Printer;
        public AudioClip AmbienceLoop,MusicLoop;
        [Range(0,1)] public float UI=.14f,Objects=.14f,Alerts=.12f,Ambience=.08f,Music=.05f;
        [Header("Rutas opcionales si se incorpora AudioMixer; buses independientes ya disponibles")]
        public AudioMixerGroup UIGroup,ObjectsGroup,AlertsGroup,AmbienceGroup,MusicGroup;
        public AudioClip Clip(AudioCue cue) => cue switch {
            AudioCue.Confirm=>Confirmation, AudioCue.Unavailable=>Unavailable, AudioCue.NewAlert=>NewAlert,
            AudioCue.Dossier=>Dossier, AudioCue.Evidence=>Evidence, AudioCue.Defense=>Defense,
            AudioCue.Teleport=>Teleport, AudioCue.TurnEnd=>TurnEnd, AudioCue.PhoneRing=>PhoneRing,
            AudioCue.PhoneAnswer=>PhoneAnswer, AudioCue.PhoneEnd=>PhoneEnd, AudioCue.PageTurn=>PageTurn,
            AudioCue.Chair=>Chair, AudioCue.Printer=>Printer, _=>null };
        public float Gain(AudioCategory category) => category==AudioCategory.UI?UI:category==AudioCategory.Objects?Objects:category==AudioCategory.Alerts?Alerts:category==AudioCategory.Ambience?Ambience:Music;
        public AudioMixerGroup Group(AudioCategory category) => category==AudioCategory.UI?UIGroup:category==AudioCategory.Objects?ObjectsGroup:category==AudioCategory.Alerts?AlertsGroup:category==AudioCategory.Ambience?AmbienceGroup:MusicGroup;
    }
}
