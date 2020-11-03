using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.EventSystem;
using RotaryHeart.Lib.SerializableDictionary;
using Tetris.Engine;
using UnityEngine;

[Serializable]
public class PlayerController : MessageListener<BoardEvent>
{
    public List<KeyChecker>                                     InputCheckers { get; } = new List<KeyChecker>();
    [SerializeField]
    private SerializableDictionaryBase<KeyCode, CheckerData>    Checkers;

    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class KeyChecker
    {
        private TinyTimer       m_Timer;
        [SerializeField]
        private float           m_StartRepeatInteval;
        [SerializeField]
        private float           m_RepeatInteval;
        [SerializeField]
        private bool            m_InvokeOnStart;
        [SerializeField]
        private KeyCode         m_Key;

        private bool            m_CancelRepeat;

        public bool             RequireAction { get; private set; }

        public bool             CancelRepeat
        {
            get => m_CancelRepeat;
            set
            {
                if (m_CancelRepeat == value)
                    return;

                RequireAction = false;
                m_CancelRepeat = value;
            }
        }

        public Action           Action { get; set; }

        public KeyCode          KeyCode
        {
            get => m_Key;
            set => m_Key = value;
        }

        //////////////////////////////////////////////////////////////////////////
        public KeyChecker(KeyCode key, Action action, float startRepeatInteval, float repeatInteval, bool invokeOnStart = true)
        {
            KeyCode = key;
            Action = action;
            m_StartRepeatInteval = startRepeatInteval;
            m_RepeatInteval = repeatInteval;
            m_InvokeOnStart = invokeOnStart;
            m_Timer = new TinyTimer();
        }

        public bool InvokeRequiredAction()
        { 
            // run action if required, return true if action invoked
            if (RequireAction)
            {
                RequireAction = false;
                if (Action != null)
                {
                    Action?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode))
            {
                if (m_InvokeOnStart)
                    RequireAction = true;

                m_CancelRepeat = false;
                m_Timer.Reset(0.0f, m_StartRepeatInteval);
            }

            if (Input.GetKeyUp(KeyCode))
            {
                RequireAction = false;
            }

            if (Input.GetKey(KeyCode))
            {
                if (m_CancelRepeat == false && m_Timer.AddTime(Time.deltaTime))
                {
                    RequireAction = true;
                    m_Timer.FinishTime = m_RepeatInteval;
                }
            }
        }
    }

    [Serializable]
    public class CheckerData
    {
        public Move     Move;
        public float    StartRepeatInterval = 12.0f / 50.007f;
        public float    RepeatInterval = 4.0f / 50.007f;
    }

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        // create input
        foreach (var checkerData in Checkers)
        {
            InputCheckers.Add(new KeyChecker(checkerData.Key, () =>
            {
                TetrisManager.Instance.GameManager.MoveBlock(checkerData.Value.Move);
            }, checkerData.Value.StartRepeatInterval, checkerData.Value.RepeatInterval));
        }
        
        // disallow for use fall in soft drop condition
        foreach (var checkerData in Checkers.Where(n => n.Value.Move == Move.Fall))
        {
            var checker = InputCheckers.FirstOrDefault(n => n.KeyCode == checkerData.Key);
            if (checker != null)
                checker.Action = () =>
                {
                    if (TetrisManager.Instance.GameManager.IsSoftDrop)
                    {
                        TetrisManager.Instance.GameManager.MoveBlock(Move.Down);
                        return;
                    }

                    TetrisManager.Instance.GameManager.MoveBlock(Move.Fall);
                };
        }
    }

    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        switch (e.Key)
        {
            case BoardEvent.UnvalidMove:
            {
                // cancel checker repeating
                var move = e.GetData<Move>();
                var keyCode = Checkers.FirstOrDefault(n => n.Value.Move == move).Key;
                var checker = InputCheckers.FirstOrDefault(n => n.KeyCode == keyCode);
                if (checker != null)
                    checker.CancelRepeat = true;
            } break;

            case BoardEvent.PlayerAction:
            {
                // if another action was happen restore cancel repeat for all
                foreach (var inputChecker in InputCheckers)
                {
                    inputChecker.CancelRepeat = false;
                }
            } break;
        }
    }

    public void InvokeCheckers()
    {
        // invoke "active" checkers if at least one was invoked send PlayerAction message
        var checkers = InputCheckers.Where(n => n.RequireAction).ToList();
        if (checkers.IsEmpty() == false)
        {
            MessageSystem.Send(BoardEvent.PlayerAction);

            foreach (var checker in checkers)
                checker.InvokeRequiredAction();
        }

    }

    public void UpdateCheckers()
    {
        // listen input
        foreach (var inputChecker in InputCheckers)
            inputChecker.Update();
    }

}