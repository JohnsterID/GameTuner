namespace GameTuner.Framework
{
	public class SearchSettings
	{
		private bool matchCase;

		private bool matchWholeWord;

		private bool useMatcher;

		private string searchValue;

		private string replaceValue;

		private IMatcher matcher;

		public IMatcher Matcher
		{
			get
			{
				return matcher;
			}
			set
			{
				matcher = value;
			}
		}

		public string SearchValue
		{
			get
			{
				return searchValue;
			}
			set
			{
				searchValue = value;
			}
		}

		public string ReplaceValue
		{
			get
			{
				return replaceValue;
			}
			set
			{
				replaceValue = value;
			}
		}

		public bool MatchCase
		{
			get
			{
				return matchCase;
			}
			set
			{
				matchCase = value;
			}
		}

		public bool MatchWholeWord
		{
			get
			{
				return matchWholeWord;
			}
			set
			{
				matchWholeWord = value;
			}
		}

		public bool UseMatcher
		{
			get
			{
				return useMatcher;
			}
			set
			{
				useMatcher = value;
			}
		}

		public SearchSettings()
		{
		}

		public SearchSettings(string searchValue)
		{
			this.searchValue = searchValue;
		}

		public SearchSettings(string searchValue, string replaceValue)
		{
			this.searchValue = searchValue;
			this.replaceValue = replaceValue;
		}
	}
}
