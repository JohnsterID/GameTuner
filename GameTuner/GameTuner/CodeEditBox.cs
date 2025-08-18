using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	internal class CodeEditBox : RichTextBox
	{
		private int[] m_aiSelectionTabs = new int[32];

		private static string[] m_asBlockOpeners = new string[4] { "function", "for", "while", "if" };

		public CodeEditBox()
		{
			Font = new Font("Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			Graphics graphics = CreateGraphics();
			int num = 3 * (int)graphics.MeasureString(" ", Font).Width;
			for (int i = 0; i < 32; i++)
			{
				m_aiSelectionTabs[i] = num * (i + 1);
			}
			SetTabSizes();
			base.TextChanged += OnTextChanged;
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			SetTabSizes();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
			case Keys.Tab:
			{
				if (!SelectionContatinsNewLine())
				{
					break;
				}
				string text5 = Text;
				int num10 = base.SelectionStart;
				int num11 = SelectionLength;
				if (num10 == 0 || text5[num10 - 1] == '\n')
				{
					text5 = text5.Insert(num10, "\t");
					num11++;
				}
				for (int k = num10; k < num10 + num11 - 1; k++)
				{
					if (text5[k] == '\n')
					{
						text5 = text5.Insert(++k, "\t");
						num11++;
					}
				}
				Text = text5;
				base.SelectionStart = num10;
				SelectionLength = num11;
				return true;
			}
			case Keys.Tab | Keys.Shift:
			{
				string text4 = Text;
				int num8 = base.SelectionStart;
				int num9 = SelectionLength;
				if (num9 == 0)
				{
					if (num8 != 0 && text4[num8 - 1] == '\t')
					{
						text4 = text4.Remove(--num8, 1);
					}
				}
				else
				{
					bool flag = false;
					for (int j = num8; j < num8 + num9; j++)
					{
						if (text4[j] == '\t')
						{
							if (!flag)
							{
								text4 = text4.Remove(j--, 1);
								num9--;
							}
							flag = true;
						}
						else
						{
							flag = false;
						}
					}
				}
				Text = text4;
				base.SelectionStart = num8;
				SelectionLength = num9;
				return true;
			}
			case Keys.Return:
			{
				if (SelectionLength != 0)
				{
					break;
				}
				string text = Text;
				int num = base.SelectionStart;
				int num2 = SelectionLength;
				uint num3 = FindTabDepth(base.SelectionStart);
				string text2 = string.Empty;
				int num4 = 0;
				for (int num5 = num - 1; num5 >= 0; num5--)
				{
					if (text[num5] == '\n')
					{
						num4 = num5;
						break;
					}
					text2 = text[num5] + text2;
				}
				if (text2.EndsWith("end"))
				{
					int length = text2.Length;
					uint num6 = 0u;
					for (int i = 0; i < text2.Length; i++)
					{
						if (text2[i] == '\t')
						{
							if (num6 < num3)
							{
								num6++;
							}
							else
							{
								text2 = text2.Remove(i, 1);
							}
						}
					}
					text = text.Remove(num4 + 1, length);
					text = text.Insert(num4 + 1, text2);
					num -= length - text2.Length;
				}
				string text3 = "\n";
				for (uint num7 = 0u; num7 < num3; num7++)
				{
					text3 += '\t';
				}
				text = text.Insert(num, text3);
				num += text3.Length;
				Text = text;
				base.SelectionStart = num;
				SelectionLength = num2;
				return true;
			}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private bool SelectionContatinsNewLine()
		{
			string text = Text;
			int num = base.SelectionStart + SelectionLength;
			for (int i = base.SelectionStart; i < num; i++)
			{
				if (text[i] == '\n')
				{
					return true;
				}
			}
			return false;
		}

		private void SetTabSizes()
		{
			int num = base.SelectionStart;
			int num2 = SelectionLength;
			base.SelectionStart = 0;
			SelectionLength = Text.Length;
			base.SelectionTabs = m_aiSelectionTabs;
			base.SelectionStart = num;
			SelectionLength = num2;
		}

		private bool IsBockOpener(string s)
		{
			string[] asBlockOpeners = m_asBlockOpeners;
			foreach (string text in asBlockOpeners)
			{
				if (s == text)
				{
					return true;
				}
			}
			return false;
		}

		private uint FindTabDepth(int iPos)
		{
			int num = 0;
			string text = Text;
			string text2 = string.Empty;
			for (int i = 0; i < iPos; i++)
			{
				char c = text[i];
				if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
				{
					text2 += c;
					continue;
				}
				text2 = text2.ToLower();
				if (IsBockOpener(text2))
				{
					num++;
				}
				else if (text2 == "end")
				{
					num--;
				}
				text2 = string.Empty;
			}
			text2 = text2.ToLower();
			if (IsBockOpener(text2))
			{
				num++;
			}
			else if (text2 == "end")
			{
				num--;
			}
			return (uint)Math.Max(num, 0);
		}
	}
}
