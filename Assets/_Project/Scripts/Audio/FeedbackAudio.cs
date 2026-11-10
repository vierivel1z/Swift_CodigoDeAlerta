using UnityEngine;
using Swift.Bank;
using Swift.Game;
using Swift.Interaction;
namespace Swift.Audio
{
    // Changed is observed for transitions only; arbitrary UI refreshes never play a tone.
    [RequireComponent(typeof(AudioSource))]
    public sealed class FeedbackAudio : MonoBehaviour
    {
        public static FeedbackAudio Current { get; private set; }
        public GameManager Manager;
        public AudioCueLibrary Library;
        public int EmittedCount { get; private set; }
        public int PlayedCount { get; private set; }
        public AudioCue LastCue { get; private set; }
        public int LastCueFrame { get; private set; }=-1;
        public event System.Action<AudioCue> CueEmitted;
        public struct Snapshot
        {
            public GameSession Session;
            public TransactionRecord Current;
            public int Evidence,Resolved;
            public bool Defense,Ended;
        }
        readonly AudioSource[] buses=new AudioSource[5];
        readonly AudioClip[] temporary=new AudioClip[System.Enum.GetValues(typeof(AudioCue)).Length];
        readonly float[] lastCue=new float[System.Enum.GetValues(typeof(AudioCue)).Length];
        Snapshot previous;
        GameManager bound;
        AudioSource phoneRingSource;
        Vector3? origin;
        bool ownsLibrary,initialized;
        void Awake() {Current=this;Initialize();}
        void OnEnable() {Bind();}
        void Start() {Bind();RefreshLoops();}
        void Initialize()
        {
            if(initialized)return;initialized=true;
            var ringNode=new GameObject("Audio Objects - telephone ring");ringNode.transform.SetParent(transform,false);
            phoneRingSource=ringNode.AddComponent<AudioSource>();phoneRingSource.playOnAwake=false;phoneRingSource.loop=false;phoneRingSource.minDistance=1;phoneRingSource.maxDistance=8;phoneRingSource.rolloffMode=AudioRolloffMode.Logarithmic;
            if(Library==null)Library=Resources.Load<AudioCueLibrary>("Audio/SWIFT_AudioCues");
            if(Library==null){Library=ScriptableObject.CreateInstance<AudioCueLibrary>();ownsLibrary=true;}
            for(int i=0;i<lastCue.Length;i++)lastCue[i]=float.NegativeInfinity;
            for(int i=0;i<buses.Length;i++) {
                var category=(AudioCategory)i;
                if(i==0)buses[i]=GetComponent<AudioSource>();
                else {var node=new GameObject("Audio bus - "+category);node.transform.SetParent(transform,false);buses[i]=node.AddComponent<AudioSource>();}
                var source=buses[i];source.playOnAwake=false;source.loop=i>=3;source.spatialBlend=0;
                source.volume=Library.Gain(category);source.outputAudioMixerGroup=Library.Group(category);
                source.minDistance=1;source.maxDistance=8;source.rolloffMode=AudioRolloffMode.Logarithmic;
            }
        }
        void Bind()
        {
            if(Manager==null)Manager=FindFirstObjectByType<GameManager>();
            if(bound==Manager)return;
            if(bound!=null)bound.Changed-=Observe;
            bound=Manager;previous=Capture(Manager);if(bound!=null)bound.Changed+=Observe;
        }
        public static Snapshot Capture(GameManager game)
        {
            var session=game?.Session;
            int resolved=0;if(session!=null)foreach(var row in session.Transactions.Records)if(row.Status!=TransactionStatus.Pending)resolved++;
            return new Snapshot{Session=session,Current=session?.Transactions.Current,Evidence=session?.Evidence.Entries.Count??0,Resolved=resolved,Defense=session?.Defense.Active==true,Ended=session?.Ended==true};
        }
        void Observe()
        {
            var next=Capture(Manager);var old=previous;previous=next;
            if(next.Session==null)return;
            if(next.Ended&&!old.Ended){Emit(AudioCue.TurnEnd);return;}
            if(next.Defense&&!old.Defense){Emit(AudioCue.Defense);return;}
            if(next.Session==old.Session&&next.Evidence>old.Evidence){Emit(AudioCue.Evidence,origin);return;}
            if(next.Current!=old.Current&&next.Current!=null&&next.Current.Data.Anomalies!=Anomaly.None){Emit(AudioCue.NewAlert);return;}
            if(next.Session!=old.Session)Emit(AudioCue.Confirm);
        }
        public void SetActionOrigin(Vector3 position) {origin=position;}
        public void AfterAction(BankAction action,Snapshot before)
        {
            var after=Capture(Manager);bool success=true;bool query=false;
            switch(action) {
                case BankAction.Approve:case BankAction.Hold:case BankAction.Return:success=after.Resolved>before.Resolved;break;
                case BankAction.Evidence:success=after.Evidence>before.Evidence;break;
                case BankAction.Defense:success=after.Defense&&!before.Defense;break;
                case BankAction.End:success=after.Ended&&!before.Ended;break;
                case BankAction.Registry:case BankAction.Alerts:case BankAction.Inspect:case BankAction.ViewDossier:query=true;success=after.Current!=null&&!after.Ended;break;
            }
            if(!success)Emit(AudioCue.Unavailable);
            else if(query)Emit(AudioCue.Dossier,origin);
            else if(LastCueFrame!=Time.frameCount)Emit(AudioCue.Confirm);
            origin=null;
        }
        public void Emit(AudioCue cue,Vector3? position=null)
        {
            Initialize();if(!isActiveAndEnabled)return;
            int index=(int)cue;
            if(index<0||index>=lastCue.Length)return;
            if(Time.unscaledTime-lastCue[index]<.12f)return;
            lastCue[index]=Time.unscaledTime;LastCue=cue;LastCueFrame=Time.frameCount;EmittedCount++;CueEmitted?.Invoke(cue);
            var category=cue==AudioCue.NewAlert?AudioCategory.Alerts:cue==AudioCue.Dossier||cue==AudioCue.Evidence||cue>=AudioCue.PhoneRing?AudioCategory.Objects:AudioCategory.UI;
            var source=cue==AudioCue.PhoneRing?phoneRingSource:buses[(int)category];source.volume=Library.Gain(category);source.outputAudioMixerGroup=Library.Group(category);
            if(category==AudioCategory.Objects){source.spatialBlend=position.HasValue?1:0;if(position.HasValue)source.transform.position=position.Value;}
            var clip=Library.Clip(cue);
            if(clip==null&&Library.UseTemporaryTones&&index<=(int)AudioCue.TurnEnd)clip=temporary[index]!=null?temporary[index]:(temporary[index]=Tone(cue));
            if(clip!=null){source.PlayOneShot(clip);PlayedCount++;}
        }
        static AudioClip Tone(AudioCue cue)
        {
            const int rate=44100;float duration=cue==AudioCue.Unavailable ? .12f : cue==AudioCue.TurnEnd ? .18f : .075f;
            float frequency=cue==AudioCue.Unavailable?240:cue==AudioCue.NewAlert?480:cue==AudioCue.Defense?880:cue==AudioCue.Teleport?540:cue==AudioCue.Evidence?760:cue==AudioCue.Dossier?620:cue==AudioCue.TurnEnd?440:720;
            var samples=new float[Mathf.RoundToInt(rate*duration)];
            for(int i=0;i<samples.Length;i++){float t=(float)i/samples.Length;float envelope=Mathf.Min(1,t*30)*(1-t)*(1-t);samples[i]=.4f*Mathf.Sin(2*Mathf.PI*frequency*i/rate)*envelope;}
            var clip=AudioClip.Create("TEMP_SYNTH_SWIFT_"+cue,samples.Length,1,rate,false);clip.SetData(samples,0);return clip;
        }
        public void StopCue(AudioCue cue) { if(cue==AudioCue.PhoneRing&&phoneRingSource!=null)phoneRingSource.Stop(); }
        public void RefreshLoops()
        {
            Initialize();Loop(AudioCategory.Ambience,Library.AmbienceLoop);Loop(AudioCategory.Music,Library.MusicLoop);
        }
        void Loop(AudioCategory category,AudioClip clip)
        {
            var source=buses[(int)category];source.volume=Library.Gain(category);source.outputAudioMixerGroup=Library.Group(category);
            if(source.clip==clip&&source.isPlaying)return;source.Stop();source.clip=clip;if(clip!=null)source.Play();
        }
        void OnApplicationPause(bool paused) {foreach(var source in buses)if(source!=null){if(paused)source.Pause();else source.UnPause();}if(phoneRingSource!=null){if(paused)phoneRingSource.Pause();else phoneRingSource.UnPause();}}
        void OnDisable() {if(bound!=null)bound.Changed-=Observe;bound=null;foreach(var source in buses)if(source!=null)source.Stop();if(phoneRingSource!=null)phoneRingSource.Stop();}
        void OnDestroy() {if(Current==this)Current=null;foreach(var clip in temporary)if(clip!=null)Destroy(clip);if(ownsLibrary)Destroy(Library);}
    }
}
