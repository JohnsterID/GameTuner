using System;
using GameTuner.Framework.Matchers;

namespace GameTuner.Framework
{
	public class SearchProvider
	{
		public class ResultInfo
		{
			public string Location;

			public string Brief;

			public object Context;

			public ISearcher Searcher;

			public ResultInfo(ISearcher searcher, string location, string brief, object context)
			{
				Searcher = searcher;
				Location = location;
				Brief = brief;
				Context = context;
			}
		}

		public class ResultInfoCollection : ListEvent<ResultInfo>
		{
		}

		private class SearcherResults : ISearcherResults
		{
			private SearchProvider provider;

			private ISearcher current;

			private ResultInfoCollection results;

			public ResultInfoCollection Results
			{
				get
				{
					return results;
				}
			}

			public ISearcher Current
			{
				get
				{
					return current;
				}
				set
				{
					current = value;
				}
			}

			public SearcherResults(SearchProvider provider)
			{
				this.provider = provider;
				results = new ResultInfoCollection();
			}

			public void AddMatch(string location, string brief, object context)
			{
				results.Add(new ResultInfo(current, location, brief, context));
			}
		}

		private SearcherCollection searchers;

		private MatcherCollection matchers;

		private SearcherResults results;

		private SearchMethod method;

		public SearcherCollection Searchers
		{
			get
			{
				return searchers;
			}
		}

		public MatcherCollection Matchers
		{
			get
			{
				return matchers;
			}
		}

		public ResultInfoCollection Results
		{
			get
			{
				return results.Results;
			}
		}

		public SearchMethod SearchMethod
		{
			get
			{
				return method;
			}
		}

		public event EventHandler SearchPerformed;

		public event EventHandler ResultsCleared;

		public SearchProvider()
		{
			method = SearchMethod.Search;
			searchers = new SearcherCollection();
			matchers = new MatcherCollection();
			results = new SearcherResults(this);
			results.Results.ClearedItems += Results_OnClear;
			matchers.Add(new MatcherWildCard());
			matchers.Add(new MatcherRegEx());
		}

		private void Results_OnClear(object sender, EventArgs e)
		{
			EventHandler resultsCleared = this.ResultsCleared;
			if (resultsCleared != null)
			{
				resultsCleared(this, EventArgs.Empty);
			}
		}

		public void Search(SearchSettings settings, SearchMethod method)
		{
			Search(settings, null, method);
		}

		public void Search(SearchSettings settings, ISearcher searcher, SearchMethod method)
		{
			this.method = method;
			if (!settings.UseMatcher || settings.Matcher == null)
			{
				settings.Matcher = new MatcherBasic();
			}
			PerformSearch(settings, searcher);
		}

		private void PerformSearch(SearchSettings settings, ISearcher searcher)
		{
			results.Results.Clear();
			if (searcher == null)
			{
				foreach (ISearcher searcher2 in searchers)
				{
					DoSearch(settings, searcher2);
				}
			}
			else
			{
				DoSearch(settings, searcher);
			}
			EventHandler searchPerformed = this.SearchPerformed;
			if (searchPerformed != null)
			{
				searchPerformed(this, EventArgs.Empty);
			}
		}

		private void DoSearch(SearchSettings settings, ISearcher searcher)
		{
			results.Current = searcher;
			searcher.Match(results, settings);
		}
	}
}
