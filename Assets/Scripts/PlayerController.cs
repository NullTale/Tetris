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
    public List<CheckerBase>                                    InputCheckers { get; } = new List<CheckerBase>();
    [SerializeField]
    private SerializableDictionaryBase<KeyCode, CheckerData>    Checkers;

    [SerializeField]
    private CheckerData m_DPadLeft;

    [SerializeField]
    private CheckerData m_DPadRight;

    [SerializeField]
    private CheckerData m_DPadUp;

    [SerializeField]
    private CheckerData m_DPadDown;

    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public abstract class CheckerBase
    {
        private TinyTimer       m_Timer;
        [SerializeField]
        private float           m_StartRepeatInteval;
        [SerializeField]
        private float           m_RepeatInteval;
        [SerializeField]
        private bool            m_InvokeOnStart;

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

        public Action Action { get; set; }
        public Move   Move   { get; set; }

        //////////////////////////////////////////////////////////////////////////
        protected CheckerBase(Move move, Action action, float startRepeatInteval, float repeatInteval, bool invokeOnStart = true)
        {
            Move = move;
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
            if (KeyDown())
            {
                if (m_InvokeOnStart)
                    RequireAction = true;

                m_CancelRepeat = false;
                m_Timer.Reset(0.0f, m_StartRepeatInteval);
            }

            if (KeyUp())
            {
                RequireAction = false;
            }

            if (KeyPressed())
            {
                if (m_CancelRepeat == false && m_Timer.AddTime(Time.deltaTime))
                {
                    RequireAction = true;
                    m_Timer.FinishTime = m_RepeatInteval;
                }
            }
        }

        protected abstract bool KeyDown();
        protected abstract bool KeyUp();
        protected abstract bool KeyPressed();
    }

    [Serializable]
    public class KeyChecker : CheckerBase
    {
        [SerializeField]
        private KeyCode         m_Key;

        //////////////////////////////////////////////////////////////////////////
        protected override bool KeyDown()
        {
            return Input.GetKeyDown(m_Key);
        }

        protected override bool KeyUp()
        {
            return Input.GetKeyUp(m_Key);
        }

        protected override bool KeyPressed()
        {
            return Input.GetKey(m_Key);
        }

        public KeyCode          KeyCode
        {
            get => m_Key;
            set => m_Key = value;
        }

        public KeyChecker(Move move, KeyCode key, Action action, float startRepeatInteval, float repeatInteval, bool invokeOnStart = true) 
            : base(move, action, startRepeatInteval, repeatInteval, invokeOnStart)
        {
            KeyCode = key;
        }
    }

    [Serializable]
    public class DPadChecker : CheckerBase
    {
        private string        m_WindowsAxis;
        private string        m_LinuxAxis;
        private KeyCode       m_MacKey;
        private float         m_AxisDirection;
        private bool          m_KeyDown;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum AxisDirection
        {
            Positiove,
            Negative
        }

        //////////////////////////////////////////////////////////////////////////
        public DPadChecker(Move move, string windowsAxis, string linuxAxis, AxisDirection axisDirection, KeyCode macKey, Action action,
                           float startRepeatInteval, float repeatInteval, bool invokeOnStart = true) 
            : base(move, action, startRepeatInteval, repeatInteval, invokeOnStart)
        {
            m_AxisDirection = axisDirection == AxisDirection.Positiove ? 1.0f : -1.0f;
            m_WindowsAxis   = windowsAxis;
            m_LinuxAxis     = linuxAxis;
            m_MacKey        = macKey;
        }

        protected override bool KeyDown()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.MacOSX:
                    return Input.GetKeyDown(m_MacKey);

                case OperatingSystemFamily.Other:
                case OperatingSystemFamily.Windows:
                    if (m_KeyDown == false)
                    {
                        if (Input.GetAxis(m_WindowsAxis) == m_AxisDirection)
                        {
                            m_KeyDown = true;
                            return true;
                        }
                    }
                    break;

                case OperatingSystemFamily.Linux:
                    if (m_KeyDown == false)
                    {
                        if (Input.GetAxis(m_LinuxAxis) == m_AxisDirection)
                        {
                            m_KeyDown = true;
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        protected override bool KeyUp()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.MacOSX:
                    return Input.GetKeyUp(m_MacKey);

                case OperatingSystemFamily.Other:
                case OperatingSystemFamily.Windows:
                    if (m_KeyDown)
                    {
                        if (Input.GetAxis(m_WindowsAxis) != m_AxisDirection)
                        {
                            m_KeyDown = false;
                            return true;
                        }
                    }
                    break;

                case OperatingSystemFamily.Linux:
                    if (m_KeyDown)
                    {
                        if (Input.GetAxis(m_LinuxAxis) != m_AxisDirection)
                        {
                            m_KeyDown = false;
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        protected override bool KeyPressed()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.MacOSX:
                    return Input.GetKey(m_MacKey);

                case OperatingSystemFamily.Other:
                case OperatingSystemFamily.Windows:
                    if (m_KeyDown && Input.GetAxis(m_WindowsAxis) == m_AxisDirection)
                            return true;
                    break;

                case OperatingSystemFamily.Linux:
                    if (m_KeyDown && Input.GetAxis(m_LinuxAxis) == m_AxisDirection)
                        return true;
                    break;
            }

            return false;
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
            InputCheckers.Add(new KeyChecker(checkerData.Value.Move, checkerData.Key, () =>
            {
                TetrisManager.Instance.GameManager.MoveBlock(checkerData.Value.Move);
            }, checkerData.Value.StartRepeatInterval, checkerData.Value.RepeatInterval));
        }

        // create D'Pad input
        initDPad("D'PadX", "D'PadXLinux", DPadChecker.AxisDirection.Negative, KeyCode.JoystickButton7, m_DPadLeft);
        initDPad("D'PadX", "D'PadXLinux", DPadChecker.AxisDirection.Positiove, KeyCode.JoystickButton8, m_DPadRight);
        initDPad("D'PadY", "D'PadYLinux", DPadChecker.AxisDirection.Positiove, KeyCode.JoystickButton5, m_DPadUp);
        initDPad("D'PadY", "D'PadYLinux", DPadChecker.AxisDirection.Negative, KeyCode.JoystickButton6, m_DPadDown);

        // disallow for use fall in soft drop condition
        foreach (var checker in InputCheckers.Where(n => n.Move == Move.Fall))
        {
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

        /////////////////////////////////////
        void initDPad(string winAxis, string linuxAxis, DPadChecker.AxisDirection dir, KeyCode macKey, CheckerData data)
        {
            InputCheckers.Add(new DPadChecker(data.Move, winAxis, linuxAxis, dir, macKey, () =>
            {
                TetrisManager.Instance.GameManager.MoveBlock(data.Move);
            }, data.StartRepeatInterval, data.RepeatInterval));

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
                foreach (var checker in InputCheckers.Where(n => n.Move == move))
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