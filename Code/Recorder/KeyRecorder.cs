﻿using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public class KeyRecorder
    {
        private IKeyboardMouseEvents _globalHook;
        public List<KeyEvent> Keys { get; private set; }
        public event EventHandler<KeyEvent> ActionRecorded;
        private KeyEvent _keyEvent;

        public KeyRecorder()
        {
            Keys = new List<KeyEvent>(1000);
        }

        public void Initialize()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += KeyDown;
            _globalHook.KeyPress += KeyPress;

        }

        private void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_keyEvent == null) return;
            _keyEvent.Value = e.KeyChar;
            PersistKey();
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {

            _keyEvent = KeyEvent.FromEvent(e);
        }

        protected virtual void OnActionRecorded(KeyEvent e)
        {
            var handler = ActionRecorded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Dispose()
        {
            if (_globalHook != null) _globalHook.Dispose();
        }

        protected virtual void PersistKey()
        {
            Keys.Add(_keyEvent);
            OnActionRecorded(_keyEvent);
            _keyEvent = null;
        }
    }

    public class KeyEvent : RecorderEvent
    {
        protected KeyEvent()
        {
            IsEmpty = false;
        }

        public static KeyEvent FromEvent(KeyEventArgs e)
        {
            return new KeyEvent()
            {
                Key = e.KeyValue, Alt = e.Alt, Control = e.Control,
                Shift = e.Shift,
                Tick = DateTime.UtcNow.Ticks, Code = e.KeyCode, Data = e.KeyData,
                Modifiers = e.Modifiers
            };
        }

        public bool IsEmpty { get; protected set; }

        public Keys Data { get; set; }
        public Keys Code { get; set; }
        public Keys Modifiers { get; set; }
        public bool Alt { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public int Key { get; set; }
        public char Value { get; set; }
    }

    public class EmptyKeyEvent : KeyEvent
    {
        private EmptyKeyEvent()
        {
            IsEmpty = true;
        }
    }

}
