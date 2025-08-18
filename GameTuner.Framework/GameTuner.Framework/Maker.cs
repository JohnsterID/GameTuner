using System;

namespace GameTuner.Framework
{
	public class Maker<T> : IMaker
	{
		private object[] args;

		public string Name
		{
			get
			{
				return typeof(T).Name;
			}
		}

		public Type Type
		{
			get
			{
				return typeof(T);
			}
		}

		public Maker()
		{
		}

		public Maker(params object[] args)
		{
			this.args = args;
		}

		public object Make()
		{
			return Make(args);
		}

		public object Make(params object[] args)
		{
			return Activator.CreateInstance(typeof(T), args);
		}

		public override string ToString()
		{
			return ReflectionHelper.GetDisplayName(typeof(T));
		}
	}
}
