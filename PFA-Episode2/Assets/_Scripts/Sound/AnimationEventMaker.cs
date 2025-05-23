#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace mup
{
    public class AnimationEventMaker : MonoBehaviour
    {
        // used for clip events
        [System.Serializable]
        public class AnimationEvent
        {
#if UNITY_EDITOR
            public UnityEngine.AnimationEvent animationEvent;
#endif
            public int frame = 0;
        }
        public List<AnimationEvent> animationEvents;

        //méthode de l'event appelée dans "SoundEventsReceiver"ooio

#if UNITY_EDITOR
            [Tooltip("Mode EDITOR uniquement. Utilisé pour paramétrer la phase. !!! phaseName = NOM DU CLIP ")]
        public AnimationClip clip;
#endif

#if UNITY_EDITOR
        public void RefreshEventsOfClip(AnimationClip animationCip)
        {
            List<UnityEngine.AnimationEvent> events = AnimationUtility.GetAnimationEvents(animationCip).ToList();

            events = events.Where(x => x.functionName != "AttackEvent").ToList();

            animationEvents = animationEvents.OrderBy(a => a.frame).ToList();
            foreach (AnimationEvent a in animationEvents)
            {
                if (a.animationEvent == null || a.animationEvent.time != a.frame * (1.0f / animationCip.frameRate))
                {
                    a.animationEvent = new UnityEngine.AnimationEvent();
                    a.animationEvent.time = (float)a.frame * (1.0f/animationCip.frameRate);
                }

                a.animationEvent.functionName = "AttackEvent";

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
    [CustomEditor(typeof(AnimationEventMaker), true)]
    [CanEditMultipleObjects]
    public class SoundEventsEditor : Editor
    {
        AnimationEventMaker soundEvents = null;

        string functionName = "";
        float eventTime = 0.0f;
        float offset = 0.0f;

        public override void OnInspectorGUI()
        {
            soundEvents = (AnimationEventMaker)target;

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