﻿using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace phoenix
{
    class TextboxWriterTraceListener : TraceListener
    {
        private TextBoxBase logbox;

        public TextboxWriterTraceListener(TextBoxBase box, string name)
        {
            Name = name;
            logbox = box;
        }

        public override void Write(string message)
        {
            if (logbox == null || logbox.IsDisposed) return;
            logbox.AppendText(message + Environment.NewLine);
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            Write(message);
        }
    }
}
