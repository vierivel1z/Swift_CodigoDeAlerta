using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Swift.Interaction;
namespace Swift.EditorTools
{
    // Preserve original destinations by identity and coordinates; allow extra exploration pads.
    public static class BankNavigationValidation
    {
        public static void Check(IEnumerable<BankInteractable> actions)
        {
            var pads=actions.Where(x=>x.Action==BankAction.Teleport).ToArray();
            var names=new[]{"Teleport Entry","Teleport Inspection","Teleport Evidence","Teleport Security"};
            var points=new[]{new Vector3(0,0,-3.3f),new Vector3(0,0,-.65f),new Vector3(-2.8f,0,-.1f),new Vector3(3.3f,0,2)};
            for(int i=0;i<names.Length;i++){
                var matches=pads.Where(x=>x.name==names[i]).ToArray();
                if(matches.Length!=1||matches[0].Player==null||Vector3.Distance(matches[0].Destination,points[i])>.001f)
                    throw new Exception("Original navigation destination changed/missing: "+names[i]);
            }
            if(pads.Any(x=>x.Player==null||!float.IsFinite(x.Destination.x)||!float.IsFinite(x.Destination.y)||!float.IsFinite(x.Destination.z)))throw new Exception("Invalid additional navigation destination");
        }
    }
}
