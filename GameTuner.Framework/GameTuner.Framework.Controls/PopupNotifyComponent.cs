using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls
{
	public class PopupNotifyComponent : Component, IPopupNotifyHandler
	{
		private class TipDescriptor : IDisposable
		{
			public IPopupNotifyHandler Handler;

			public string Text;

			public object Tag;

			public INotifyPopupView View;

			public Rectangle Location;

			public int Time;

			public TipDescriptor(IPopupNotifyHandler handler, string text, object tag)
			{
				Time = 0;
				Handler = handler;
				Text = text;
				Tag = tag;
				Location = Rectangle.Empty;
			}

			public void Dispose()
			{
				if (View != null && View is Form)
				{
					Form form = (Form)View;
					form.Close();
					form.Dispose();
					View = null;
				}
			}
		}

		private List<TipDescriptor> tips = new List<TipDescriptor>();

		private IContainer components;

		private Timer timer1;

		[Description("Eddge padding in pixels to position the notification")]
		public int EdgePadding { get; set; }

		[Description("Bottom padding in pixels to position the notification")]
		public int BottomPadding { get; set; }

		[Description("Time in milliseconds to hold the notification")]
		public int HoldTime { get; set; }

		public event PopupNotifyClickedHandler NotifyClicked;

		public PopupNotifyComponent()
		{
			InitializeComponent();
			SetDefaults();
		}

		public PopupNotifyComponent(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			SetDefaults();
		}

		private void SetDefaults()
		{
			HoldTime = 10000;
			EdgePadding = 32;
			BottomPadding = 2;
		}

		public void AddNotify(string text)
		{
			TipDescriptor tipDescriptor = new TipDescriptor(this, text, null);
			tipDescriptor.Tag = tipDescriptor;
			if (ShowTipNotify(tipDescriptor))
			{
				tips.Add(tipDescriptor);
			}
		}

		public void AddNotify(IPopupNotifyHandler handler, object tag)
		{
			TipDescriptor tipDescriptor = new TipDescriptor(handler, null, tag);
			if (ShowTipNotify(tipDescriptor))
			{
				tips.Add(tipDescriptor);
			}
		}

		private bool ShowTipNotify(TipDescriptor tip)
		{
			string text = "";
			Image image = null;
			string description = "";
			if (!tip.Handler.GetNotifyInfo(ref text, ref image, ref description, tip.Tag))
			{
				return false;
			}
			tip.View = CreateNotifyView();
			tip.View.Image = image;
			tip.View.Caption = text;
			tip.View.Description = description;
			Rectangle empty = Rectangle.Empty;
			empty = Screen.GetWorkingArea(empty);
			Size tipSize = tip.View.TipSize;
			tip.View.Position = new Point(empty.Right - tipSize.Width - EdgePadding, TotalHeight(empty) - tipSize.Height - BottomPadding);
			tip.Location = new Rectangle(tip.View.Position, tipSize);
			tip.View.ShowTip(null, true);
			tip.View.ClickedNotification += View_ClickedNotification;
			return true;
		}

		private void View_ClickedNotification(object sender, MouseEventArgs e)
		{
			INotifyPopupView notifyPopupView = sender as INotifyPopupView;
			TipDescriptor tipDescriptor;
			if (notifyPopupView != null && (tipDescriptor = Find(notifyPopupView)) != null)
			{
				PopupNotifyClickedHandler notifyClicked = this.NotifyClicked;
				if (notifyClicked != null)
				{
					notifyClicked(notifyPopupView, tipDescriptor.Tag, e);
				}
			}
		}

		private TipDescriptor Find(INotifyPopupView view)
		{
			foreach (TipDescriptor tip in tips)
			{
				if (tip.View == view)
				{
					return tip;
				}
			}
			return null;
		}

		private int TotalHeight(Rectangle rect)
		{
			int num = rect.Bottom;
			foreach (TipDescriptor tip in tips)
			{
				num = Math.Min(num, tip.Location.Top);
			}
			return num;
		}

		protected virtual INotifyPopupView CreateNotifyView()
		{
			return new ToolTipView();
		}

		public bool GetNotifyInfo(ref string text, ref Image image, ref string description, object tag)
		{
			TipDescriptor tipDescriptor = tag as TipDescriptor;
			if (tipDescriptor != null)
			{
				text = tipDescriptor.Text ?? "";
				return true;
			}
			return false;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			int interval = timer1.Interval;
			List<TipDescriptor> list = new List<TipDescriptor>();
			foreach (TipDescriptor tip in tips)
			{
				switch (tip.View.FadeMode)
				{
				case ToolTipFadeMode.InComplete:
					tip.Time += interval;
					if (tip.Time > HoldTime)
					{
						tip.View.ShowTip(null, false);
					}
					break;
				case ToolTipFadeMode.OutComplete:
					list.Add(tip);
					break;
				}
			}
			foreach (TipDescriptor item in list)
			{
				item.View.ClickedNotification -= View_ClickedNotification;
				item.Dispose();
				tips.Remove(item);
			}
		}

		public void Clear()
		{
			foreach (TipDescriptor tip in tips)
			{
				tip.View.ClickedNotification -= View_ClickedNotification;
				tip.View.Dismiss();
			}
			tips.Clear();
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
			timer1 = new Timer(components);
			timer1.Enabled = true;
			timer1.Tick += timer1_Tick;
		}
	}
}
