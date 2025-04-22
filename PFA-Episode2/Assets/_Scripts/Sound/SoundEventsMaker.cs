#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

namespace mup
{
    public class SoundEventsMaker : MonoBehaviour
    {
        // used for clip events
        [System.Serializable]
        public class SoundEvent
        {
#if UNITY_EDITOR
            public AnimationEvent animationEvent;
#endif
            public int frame = 0;
            //public float time = 0.0f;
            public Sounds sound = 0; //<- ici l'enum en question
        }
        public List<SoundEvent> soundEvents;

        //méthode de l'event appelée dans "SoundEventsReceiver"ooio

#if UNITY_EDITOR
            [Tooltip("Mode EDITOR uniquement. Utilisé pour paramétrer la phase. !!! phaseName = NOM DU CLIP ")]
        public AnimationClip clip;
#endif

#if UNITY_EDITOR
        public void RefreshEventsOfClip(AnimationClip animationCip)
        {
            List<AnimationEvent> events = AnimationUtility.GetAnimationEvents(animationCip).ToList();

            

            events = events.Where(x => x.functionName != "CallSound").ToList();

            soundEvents = soundEvents.OrderBy(a => a.frame).ToList();
            foreach (SoundEvent a in soundEvents)
            {
                if (a.animationEvent == null || a.animationEvent.time != a.frame * (1.0f / animationCip.frameRate))
                {
                    a.animationEvent = new AnimationEvent();
                    a.animationEvent.time = (float)a.frame * (1.0f/animationCip.frameRate);
                }

                a.animationEvent.functionName = "CallSound";
                a.animationEvent.stringParameter = ((int)a.sound).ToString();

                events.Add(a.animationEvent);
            }

            AnimationUtility.SetAnimationEvents(animationCip, events.ToArray());

        }
#endif
    }
}


#if UNITY_EDITOR
namespace mup
{
    [CustomEditor(typeof(SoundEventsMaker), true)]
    [CanEditMultipleObjects]
    public class SoundEventsEditor : Editor
    {
        SoundEventsMaker soundEvents = null;

        string functionName = "";
        float eventTime = 0.0f;
        float offset = 0.0f;

        public override void OnInspectorGUI()
        {
            soundEvents = (SoundEventsMaker)target;

            if (soundEvents.clip != null && GUILayout.Button("Refresh sound events"))
            {
                soundEvents.RefreshEventsOfClip(soundEvents.clip);
            }

            EditorGUILayout.LabelField("----------- DEFAULT INSPECTOR", EditorStyles.boldLabel);
            DrawDefaultInspector();
            serializedObject.Update();
        }
    }
}
#endif