using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class UIThreadEventComponent : Component
	{
		private List<DelayedDelegate> delegates = new List<DelayedDelegate>();

		private object handle = new object();

		private IContainer components;

		private Timer timer;

		public UIThreadEventComponent()
		{
			InitializeComponent();
			InitTimer();
		}

		public UIThreadEventComponent(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			InitTimer();
		}

		private void InitTimer()
		{
			timer.Tick += timer_Tick;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			lock (handle)
			{
				if (delegates.Count <= 0)
				{
					return;
				}
				foreach (DelayedDelegate @delegate in delegates)
				{
					@delegate.Execute();
				}
				delegates.Clear();
			}
		}

		public void PostDelegate(Delegate d, params object[] args)
		{
			lock (handle)
			{
				delegates.Add(new DelayedDelegate(d, args));
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new Container();
			timer = new Timer(components);
			timer.Enabled = true;
			timer.Interval = 500;
		}
	}
}
