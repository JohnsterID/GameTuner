using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Resources;

namespace GameTuner.Framework
{
	[Description("List that can contain custom display items")]
	[ToolboxBitmap(typeof(ResourceTag), "scrollablelist.bmp")]
	public class ScrollableList : ScrollUserControl
	{
		private class DisplayScrollableItem
		{
			public int Origin;

			public IScrollableItem Item;

			public DisplayScrollableItem(int origin, IScrollableItem item)
			{
				Origin = origin;
				Item = item;
			}
		}

		private class DisplayScrollableItemCollection : List<DisplayScrollableItem>
		{
		}

		private ScrollableItemCollection items = new ScrollableItemCollection();

		private ScrollableItemCollection selected = new ScrollableItemCollection();

		private DisplayScrollableItemCollection display = new DisplayScrollableItemCollection();

		private bool updating;

		private bool notifyChange = true;

		private int prevSelection = -1;

		public int SelectedIndex
		{
			get
			{
				return items.IndexOf(SelectedItem);
			}
		}

		public ScrollableItemCollection Items
		{
			get
			{
				return items;
			}
		}

		public IScrollableItem SelectedItem
		{
			get
			{
				if (selected.Count <= 0)
				{
					return null;
				}
				return selected[0];
			}
			set
			{
				SelectItem(value);
			}
		}

		public object SelectedItemTag
		{
			get
			{
				if (selected.Count <= 0)
				{
					return null;
				}
				return selected[0].Tag;
			}
		}

		public ScrollableItemCollection SelectedItems
		{
			get
			{
				return selected;
			}
		}

		private IScrollableItem FirstSelected
		{
			get
			{
				if (selected.Count > 0)
				{
					return selected[0];
				}
				return null;
			}
		}

		public event EventHandler SelectedChanged;

		public event EventHandler SelectedIndexChanged;

		public ScrollableList()
		{
			InitializeComponent();
			items.AddedItem += items_OnAddItem;
			items.RemovedItem += items_OnRemoveItem;
			items.ClearedItems += items_OnClear;
			selected.AddedItem += selected_OnAddItem;
			selected.RemovedItem += selected_OnRemoveItem;
			selected.ClearedItems += selected_OnClear;
		}

		public void BeginUpdate()
		{
			updating = true;
		}

		public void EndUpdate()
		{
			updating = false;
			UpdateScrollSizes();
			Invalidate();
		}

		public void SelectItem(IScrollableItem item)
		{
			selected.Clear();
			selected.Add(item);
		}

		private void RebuildDisplayList()
		{
			display.Clear();
			int num = 0;
			SizeF size = new SizeF(base.ClientSize.Width, base.ClientSize.Height);
			using (Graphics g = Graphics.FromHwnd(base.Handle))
			{
				foreach (IScrollableItem item in items)
				{
					if (item.Visible)
					{
						item.CalcLayout(g, size);
						display.Add(new DisplayScrollableItem(num, item));
						num += item.ItemHeight;
					}
				}
			}
			base.VerticalScroll.MaxValue = num;
		}

		private void items_OnClear(object sender, EventArgs e)
		{
			RebuildDisplayList();
			selected.Clear();
			Invalidate();
		}

		public override void UpdateScrollSizes()
		{
			RebuildDisplayList();
			base.UpdateScrollSizes();
		}

		private void items_OnRemoveItem(object sender, ListEvent<IScrollableItem>.ListEventArgs e)
		{
			RebuildDisplayList();
			selected.Remove(e.Item);
			Invalidate();
		}

		private void items_OnAddItem(object sender, EventArgs e)
		{
			RebuildDisplayList();
			Invalidate();
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.DoubleBuffered = true;
			base.Name = "ScrollableList";
			base.Paint += new System.Windows.Forms.PaintEventHandler(ScrollableList_Paint);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(ScrollableList_MouseDown);
			base.SizeChanged += new System.EventHandler(ScrollableList_SizeChanged);
			base.KeyDown += new System.Windows.Forms.KeyEventHandler(ScrollableList_KeyDown);
			base.ResumeLayout(false);
		}

		private void ScrollableList_Paint(object sender, PaintEventArgs e)
		{
			if (!updating)
			{
				PaintItems(e.Graphics);
			}
		}

		private void PaintItems(Graphics g)
		{
			int value = base.VerticalScroll.Value;
			int num = value + base.ClientSize.Height;
			foreach (DisplayScrollableItem item in display)
			{
				if (item.Origin + item.Item.ItemHeight >= value || item.Origin < num)
				{
					Rectangle r = new Rectangle(0, item.Origin - value, base.ClientSize.Width, item.Item.ItemHeight);
					PaintItem(g, item.Item, r);
				}
			}
		}

		private void PaintItem(Graphics g, IScrollableItem item, Rectangle r)
		{
			ScrollableItemState scrollableItemState = (selected.Contains(item) ? ScrollableItemState.Selected : ScrollableItemState.Normal);
			item.PaintItem(this, new ScrollableItemPaintEventArgs(g, r, scrollableItemState));
		}

		private void selected_Changed()
		{
			int selectedIndex = SelectedIndex;
			if (notifyChange && selectedIndex != prevSelection)
			{
				prevSelection = selectedIndex;
				EventHandler selectedChanged = this.SelectedChanged;
				if (selectedChanged != null)
				{
					selectedChanged(this, EventArgs.Empty);
				}
				selectedChanged = this.SelectedIndexChanged;
				if (selectedChanged != null)
				{
					selectedChanged(this, EventArgs.Empty);
				}
				Invalidate();
			}
		}

		private void selected_OnClear(object sender, EventArgs e)
		{
			selected_Changed();
		}

		private void selected_OnRemoveItem(object sender, EventArgs e)
		{
			selected_Changed();
		}

		private void selected_OnAddItem(object sender, EventArgs e)
		{
			selected_Changed();
		}

		private void ScrollableList_SizeChanged(object sender, EventArgs e)
		{
			RebuildDisplayList();
			Invalidate();
		}

		private void ScrollableList_MouseDown(object sender, MouseEventArgs e)
		{
			Focus();
			if (e.Button == MouseButtons.Left)
			{
				IScrollableItem scrollableItem = FindItem(e.X, e.Y);
				if (scrollableItem != null)
				{
					notifyChange = false;
					selected.Clear();
					notifyChange = true;
					selected.Add(scrollableItem);
				}
				else
				{
					selected.Clear();
				}
			}
		}

		public IScrollableItem FindItem(int x, int y)
		{
			x += base.HorizontalScroll.Value;
			y += base.VerticalScroll.Value;
			foreach (DisplayScrollableItem item in display)
			{
				if (y >= item.Origin && y < item.Origin + item.Item.ItemHeight)
				{
					return item.Item;
				}
			}
			return null;
		}

		public void SelectItemAtPoint()
		{
			SelectItemAtPoint(PointToClient(Cursor.Position));
		}

		public void SelectItemAtPoint(Point location)
		{
			IScrollableItem scrollableItem = FindItem(location.X, location.Y);
			if (scrollableItem != null)
			{
				notifyChange = false;
				selected.Clear();
				notifyChange = true;
				selected.Add(scrollableItem);
			}
		}

		private int GetDisplayIndex(IScrollableItem item)
		{
			int num = 0;
			foreach (DisplayScrollableItem item2 in display)
			{
				if (item2.Item == item)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public void EnsureVisible(IScrollableItem item)
		{
			int displayIndex = GetDisplayIndex(item);
			if (displayIndex != -1)
			{
				DisplayScrollableItem displayScrollableItem = display[displayIndex];
				if (displayScrollableItem.Origin <= base.VerticalScroll.Value)
				{
					base.VerticalScroll.Value = displayScrollableItem.Origin;
				}
				else if (displayScrollableItem.Origin + displayScrollableItem.Item.ItemHeight >= base.VerticalScroll.Value + base.ClientSize.Height)
				{
					base.VerticalScroll.Value = displayScrollableItem.Origin + displayScrollableItem.Item.ItemHeight - base.ClientSize.Height;
				}
				Invalidate();
			}
		}

		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
			case Keys.Left:
			case Keys.Up:
			case Keys.Right:
			case Keys.Down:
				return true;
			default:
				return base.IsInputKey(keyData);
			}
		}

		private void ScrollableList_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
			case Keys.Home:
				if (display.Count > 0)
				{
					IScrollableItem item2 = display[0].Item;
					SelectItem(item2);
					EnsureVisible(item2);
				}
				break;
			case Keys.Down:
				if (display.Count > 0)
				{
					int displayIndex2 = GetDisplayIndex(FirstSelected);
					IScrollableItem item4 = display[(displayIndex2 != -1) ? ((displayIndex2 == display.Count - 1) ? (display.Count - 1) : (displayIndex2 + 1)) : 0].Item;
					SelectItem(item4);
					EnsureVisible(item4);
				}
				break;
			case Keys.Up:
				if (display.Count > 0)
				{
					int displayIndex = GetDisplayIndex(FirstSelected);
					DisplayScrollableItemCollection displayScrollableItemCollection = display;
					int index;
					switch (displayIndex)
					{
					default:
						index = displayIndex - 1;
						break;
					case 0:
						index = 0;
						break;
					case -1:
						index = display.Count - 1;
						break;
					}
					IScrollableItem item3 = displayScrollableItemCollection[index].Item;
					SelectItem(item3);
					EnsureVisible(item3);
				}
				break;
			case Keys.End:
				if (display.Count > 0)
				{
					IScrollableItem item = display[display.Count - 1].Item;
					SelectItem(item);
					EnsureVisible(item);
				}
				break;
			case Keys.Left:
			case Keys.Right:
				break;
			}
		}
	}
}
