using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace GameTuner.Framework
{
	public sealed class Context : ISite, IServiceProvider
	{
		private static Context self = new Context();

		private List<object> contexts;

		private string name;

		private bool cachedDesignMode;

		private bool inDesignMode;

		public static ISite Site
		{
			get
			{
				return self;
			}
		}

		public IComponent Component
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IContainer Container
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public static bool InDesignMode
		{
			get
			{
				return self.DesignMode;
			}
		}

		public bool DesignMode
		{
			get
			{
				if (!cachedDesignMode)
				{
					cachedDesignMode = true;
					if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
					{
						inDesignMode = true;
					}
					else if (Process.GetCurrentProcess().ProcessName.ToUpper().Equals("DEVENV"))
					{
						inDesignMode = true;
					}
				}
				return inDesignMode;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		private Context()
		{
			contexts = new List<object>();
			name = "";
		}

		public static object Get(Type type)
		{
			if (type.IsInstanceOfType(self))
			{
				return self;
			}
			foreach (object context in self.contexts)
			{
				if (type.IsInstanceOfType(context))
				{
					return context;
				}
			}
			throw new ArgumentException(string.Format("Type ({0}) does not exist in context", type.ToString()));
		}

		public static T Get<T>()
		{
			return (T)Get(typeof(T));
		}

		public static bool Has(Type type)
		{
			if (type.IsInstanceOfType(self))
			{
				return true;
			}
			foreach (object context in self.contexts)
			{
				if (type.IsInstanceOfType(context))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Has<T>()
		{
			return Has(typeof(T));
		}

		public static void Add(object o)
		{
			self.contexts.Add(o);
		}

		public static void Remove(object o)
		{
			self.contexts.Remove(o);
		}

		public static void Clear()
		{
			self.contexts.Clear();
		}

		public static T GetService<T>()
		{
			return (T)self.GetService(typeof(T));
		}

		public object GetService(Type serviceType)
		{
			if (Has(serviceType))
			{
				return Get(serviceType);
			}
			return null;
		}
	}
}
