﻿using System;
using System.Collections.Generic;

namespace KeyMacro
{
    class State<KeyTypeTransition>
    {
        private static int m_idCounter = 0;
        public string Name { get; }
        public int StateId { get; }
        public Dictionary<KeyTypeTransition, State<KeyTypeTransition>> NextState { get; }
        public Action<object> ActionState { get; set; }

        public State(string name, Dictionary<KeyTypeTransition, State<KeyTypeTransition>> nextState = null, Action<object> action = null)
        {
            Name = name;
            ActionState = action;
            if (nextState == null)
                nextState = new Dictionary<KeyTypeTransition, State<KeyTypeTransition>>();
            NextState = nextState;
            StateId = m_idCounter++;
        }

        public void ExecuteActionState(object obj)
        {
            if (ActionState != null)
                ActionState(obj);
        }

        public void AddNextState(KeyTypeTransition value, State<KeyTypeTransition> state)
        {
            NextState.Add(value, state);
        }
    }
}