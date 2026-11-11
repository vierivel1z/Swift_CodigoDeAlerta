using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Swift.Bank;
using Swift.Game;
using Swift.UI;
using Swift.VR;
using Swift.DeskTools;

namespace Swift.EditorTools
{
    // Explicit local queue. This component is Editor-only and never enters the Android build.
    [InitializeOnLoad]
    public static class SecondPolishReview
    {
        const string Queue="Artifacts/SecondAutonomousPolish/command.txt";
        const string Pending="SWIFT_SECOND_POLISH";
        const string Main="Assets/_Project/Scenes/BancoCorresponsal.unity";
        static IEnumerator routine;
        static string stage;
        static int errors;

        static SecondPolishReview()
        {
            EditorApplication.update+=Tick;
            EditorApplication.playModeStateChanged+=PlayChanged;
        }

        static string Folder => "Docs/Previews/SecondAutonomousPolish/"+stage;
        static string Evidence => "Artifacts/SecondAutonomousPolish/"+stage;

        static void Tick()
        {
            if(routine!=null){try{if(!routine.MoveNext())Finish(null);}catch(Exception ex){Finish(ex);}return;}
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||!File.Exists(Queue))return;
            stage=File.ReadAllText(Queue).Trim();File.Delete(Queue);
            try{
                Directory.CreateDirectory(Evidence);
                Require(stage=="Before"||stage=="After","Unknown second polish command");
                Require(SceneManager.sceneCount==1&&SceneManager.GetActiveScene().path==Main&&!SceneManager.GetActiveScene().isDirty,"Saved clean BancoCorresponsal required");
                Directory.CreateDirectory(Folder);Directory.CreateDirectory(Evidence);
                SessionState.SetString(Pending,stage);EditorApplication.EnterPlaymode();
            }catch(Exception ex){Finish(ex);}
        }

        static void PlayChanged(PlayModeStateChange state)
        {
            if(state!=PlayModeStateChange.EnteredPlayMode)return;
            var value=SessionState.GetString(Pending,"");if(value=="")return;
            SessionState.EraseString(Pending);stage=value;errors=0;
            Application.logMessageReceived+=Log;
            routine=Run();
        }

        static void Log(string message,string stack,LogType type)
        {
            if(type!=LogType.Error&&type!=LogType.Exception&&type!=LogType.Assert)return;
            errors++;File.AppendAllText(Evidence+"/console-errors.txt",type+": "+message+"\n"+stack+"\n");
        }

        static void Finish(Exception error)
        {
            routine=null;Application.logMessageReceived-=Log;
            Directory.CreateDirectory(Evidence);
            string receipt=(error==null&&errors==0?"PASS ":"FAIL ")+stage+" "+DateTime.UtcNow.ToString("O")+
                " Unity="+Application.unityVersion+" EDITOR ONLY; ANDROID NOT TESTED\n"+(error?.ToString()??(errors==0?"":"Runtime errors="+errors));
            File.WriteAllText(Evidence+"/result.txt",receipt);
            if(EditorApplication.isPlaying)EditorApplication.ExitPlaymode();
        }

        static void Require(bool okay,string message){File.AppendAllText(Evidence+"/checks.txt",(okay?"PASS ":"FAIL ")+message+"\n");if(!okay)throw new Exception(message);}
        static IEnumerator Wait(float seconds){float end=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<end)yield return null;}

        static IEnumerator Run()
        {
            yield return null;yield return null;yield return null;
            var game=UnityEngine.Object.FindFirstObjectByType<GameManager>();
            var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();
            var gaze=UnityEngine.Object.FindFirstObjectByType<Swift.Interaction.VRInteractionManager>();
            var seat=AnalystSeat.Current;
            Require(game!=null&&ui!=null&&gaze!=null&&seat!=null&&seat.Rig!=null,"Bank, UI, gaze and seat are loaded");
            Capture("01-portal.png",new Vector3(0,1.6f,-6.6f),new Vector3(0,1.45f,-2.1f));
            game.EnterScenario(ScenarioMode.Attack);yield return null;
            Require(game.IntroPlaying,"Prologue starts from mode entry");
            Capture("02-prologo.png",gaze.View.transform.position,gaze.View.transform.position+gaze.View.transform.forward*2);
            game.SkipIntro();yield return null;
            Require(!game.IntroPlaying&&game.Session?.Mode==ScenarioMode.Attack,"Prologue skip starts selected mode");
            seat.Rig.position+=seat.StandingFloor.position+Vector3.up*1.6f-gaze.View.transform.position;
            yield return null;
            Require(seat.Request(true),"Analyst can sit without moving the camera artificially");
            while(seat.IsTransitioning)yield return null;
            yield return null;
            Require(seat.IsSeated,"Seated position reached");
            Capture("03-expediente.png",gaze.View.transform.position,ui.PanelAnchor.position);

            game.Notify("Aviso estable de rendimiento.");yield return null;
            int beforeObjects=UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length;
            long beforeMemory=GC.GetAllocatedBytesForCurrentThread();float start=Time.realtimeSinceStartup;
            for(int i=0;i<20;i++)game.Notify("Aviso estable de rendimiento.");
            float elapsed=(Time.realtimeSinceStartup-start)*1000;
            long allocated=GC.GetAllocatedBytesForCurrentThread()-beforeMemory;
            int afterObjects=UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length;
            File.WriteAllLines(Evidence+"/repeat-notify-metrics.txt",new[]{"SameMessageNotifications=20","MillisecondsSynchronous="+elapsed.ToString("F3"),
                "ManagedAllocatedBytesCurrentThread="+allocated,"RectTransformsBefore="+beforeObjects,"RectTransformsImmediatelyAfter="+afterObjects,
                "RectTransformDelta="+(afterObjects-beforeObjects),"TotalAllocatedMemory="+UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong(),
                "Note=Editor Play measurement; not Android FPS or GPU data."});
            yield return null;

            var record=game.Session.Transactions.Current;var data=record.Data;
            string oldOrigin=data.OriginBank,oldBeneficiary=data.Beneficiary,oldAccount=data.Account,oldIntermediary=data.IntermediaryBank,oldDate=data.Date;
            long oldAmount=data.Amount;string oldMessage=game.Message;
            try{
                data.OriginBank="BANCO INTERNACIONAL DE OPERACIONES Y CORRESPONSALIA DEL PACIFICO SUR (FICTICIO)";
                data.Beneficiary="Asociacion Cultural y Educativa de Cooperacion Regional San Miguel del Valle";
                data.Account="FIC-2026-000000000000000000000000000000012345";
                data.IntermediaryBank="Banco Intermediario de Servicios Financieros Internacionales (ficticio)";
                data.Date=null;data.Amount=9876543210123;
                ui.RefreshPresentation();yield return null;
                Capture("04-datos-largos.png",gaze.View.transform.position,ui.PanelAnchor.position);
                var root=ui.PanelAnchor.GetComponentInChildren<Canvas>().transform;
                Canvas.ForceUpdateCanvases();
                var overflow=root.GetComponentsInChildren<TMP_Text>().Where(x=>{x.ForceMeshUpdate();return x.isTextOverflowing;}).Select(x=>x.name+"="+x.text).ToArray();
                File.WriteAllLines(Evidence+"/long-data-overflow.txt",new[]{"TMPOverflowCount="+overflow.Length}.Concat(overflow));
                game.Notify(new string('O',700));ui.ShowDocument(BankDocumentUI.Page.Observations);yield return null;
                var detail=ui.PanelAnchor.GetComponentsInChildren<TMP_Text>(true).First(x=>x.name=="Detalle paginado");detail.ForceMeshUpdate();
                Require(detail.textInfo.pageCount>1,"Long optional observations paginate instead of shrinking all text");
                Require(detail.text.Contains(data.OriginBank)&&detail.text.Contains(data.Beneficiary)&&detail.text.Contains(data.Account),"Full long identifiers remain available in the paginated detail");
                var docs=ui.GetComponent<BankDocumentUI>();docs.StepObservations(1);yield return null;
                detail=ui.PanelAnchor.GetComponentsInChildren<TMP_Text>(true).First(x=>x.name=="Detalle paginado");
                Require(detail.pageToDisplay==2,"Second observation page is reachable");
                Capture("05-observaciones-pagina2.png",gaze.View.transform.position,ui.PanelAnchor.position);
            }finally{
                data.OriginBank=oldOrigin;data.Beneficiary=oldBeneficiary;data.Account=oldAccount;
                data.IntermediaryBank=oldIntermediary;data.Date=oldDate;data.Amount=oldAmount;
                game.Notify(oldMessage);ui.ShowDocument(BankDocumentUI.Page.Dossier);ui.RefreshPresentation();
            }
            foreach(var page in new[]{BankDocumentUI.Page.Registry,BankDocumentUI.Page.Alerts,BankDocumentUI.Page.Procedure,BankDocumentUI.Page.Dossier})
                for(int i=0;i<5;i++)ui.ShowDocument(page);
            Require(ui.GetComponent<BankDocumentUI>().CurrentPage==BankDocumentUI.Page.Dossier,"Rapid tab switches return to dossier");
            var tools=DeskInteractionCoordinator.Current;
            Require(tools!=null&&tools.CanWork,"Desk tool coordinator is ready");
            tools.Handle(DeskCommand.Clipboard);Require(tools.InspectionOpen,"Clipboard inspection opens");
            Require(seat.Request(false),"Standing remains available with inspection open");
            while(seat.IsTransitioning)yield return null;
            yield return null;Require(!tools.InspectionOpen,"Standing closes inspection without leaving modal behind");
            Require(seat.Request(true),"Seat can be entered again");while(seat.IsTransitioning)yield return null;

            game.StartScenario(ScenarioMode.Attack);for(int i=0;i<6;i++)game.Decide(Decision.Approve);
            game.StartScenario(ScenarioMode.Defense);game.ApplyDefense();for(int i=0;i<6;i++)game.Decide(Decision.Approve);
            yield return null;
            Require(game.Session.Ended&&game.LastAttackLoss==965000&&game.LastDefenseLoss==0,"Attack and defense calculations unchanged");
            Capture("06-resultados.png",gaze.View.transform.position,ui.PanelAnchor.position);
            tools.Handle(DeskCommand.Debrief);Require(tools.ActiveTool=="Debrief","Educational review opens after turn");
            tools.Handle(DeskCommand.Next);Capture("07-revision-educativa.png",gaze.View.transform.position,tools.View.transform.position);
            tools.CloseInspection();
            game.StartScenario(ScenarioMode.Defense);game.ApplyDefense();
            tools.ObserveQuery(Swift.Interaction.BankAction.Registry);game.InspectRegistry();game.RegisterEvidence();
            for(int i=0;i<6;i++)game.Decide(Decision.Approve);
            yield return null;
            Require(game.Session.Ended&&game.Session.Evidence.Entries.Count==1&&tools.Journal.For("SIM-001").Sources.Contains("Registro"),
                "Results retain registered evidence and consulted source");
            Capture("08-resultados-con-evidencia.png",gaze.View.transform.position,ui.PanelAnchor.position);
            Require(errors==0,"No runtime errors during second polish review");
        }

        static void Capture(string file,Vector3 eye,Vector3 look)
        {
            Canvas.ForceUpdateCanvases();Physics.SyncTransforms();
            var view=new SceneReviewCapture.View{file=file,position=eye,target=look,fieldOfView=60};
            var render=typeof(SceneReviewCapture).GetMethod("Render",BindingFlags.NonPublic|BindingFlags.Static);
            Require(render!=null,"Review renderer is present");
            render.Invoke(null,new object[]{SceneManager.GetActiveScene(),view,Folder+"/"+file});
            File.AppendAllText(Folder+"/capture-receipt.txt",DateTime.UtcNow.ToString("O")+" "+file+" Unity="+Application.unityVersion+" Play=True Android=False\n");
        }
    }
}
