using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Cinematics
{
    public class FakePlayableDirector : MonoBehaviour
    {
        public event Action<float> onFinish;

        private void Start()
        {
            Invoke("OnFinish", 3f);
        }

        void OnFinish()
        {
            onFinish(4.3f);
        }
    }

}
