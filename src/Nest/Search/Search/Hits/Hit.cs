﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Nest
{
	[ContractJsonConverter(typeof(DefaultHitJsonConverter))]
	public interface IHit<out T> where T : class
	{
		IFieldSelection<T> Fields { get; }
		T Source { get; }
		string Index { get; }
		string Type { get; }
		long? Version { get; }
		double Score { get; }
		string Id { get; }
		IEnumerable<object> Sorts { get; }
		HighlightFieldDictionary Highlights { get; }
		Explanation Explanation { get; }
		IEnumerable<string> MatchedQueries { get; }
		IDictionary<string, InnerHitsResult> InnerHits { get; }
	}

	[JsonObject]
	public class Hit<T> : IHit<T>
		where T : class
	{
		[JsonProperty(PropertyName = "fields")]
		internal IDictionary<string, object> _fields { get; set; }
		[JsonIgnore]
		public IFieldSelection<T> Fields { get; internal set; }
		[JsonProperty(PropertyName = "_source")]
		public T Source { get; internal set; }
		[JsonProperty(PropertyName = "_index")]
		public string Index { get; internal set; }
		
		[JsonProperty(PropertyName = "inner_hits")]
		[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter))]
		public IDictionary<string, InnerHitsResult> InnerHits { get; internal set; }
		
		[JsonProperty(PropertyName = "_score")]
		public double Score { get; set; }

		[JsonProperty(PropertyName = "_type")]
		public string Type { get; internal set; }

		[JsonProperty(PropertyName = "_version")]
		public long? Version { get; internal set; }

		[JsonProperty(PropertyName = "_id")]
		public string Id { get; internal set; }

		[JsonProperty(PropertyName = "sort")]
		public IEnumerable<object> Sorts { get; internal set; }

		[JsonProperty(PropertyName = "highlight")]
		[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter))]
		internal Dictionary<string, List<string>> _Highlight { get; set; }

		public HighlightFieldDictionary Highlights
		{
			get
			{
				if (_Highlight == null)
					return new HighlightFieldDictionary();

				var highlights = _Highlight.Select(kv => new HighlightHit
				{
					DocumentId = this.Id,
					Field = kv.Key,
					Highlights = kv.Value
				}).ToDictionary(k => k.Field, v => v);

				return new HighlightFieldDictionary(highlights);
			}
		}

		[JsonProperty(PropertyName = "_explanation")]
		public Explanation Explanation { get; internal set; }

		[JsonProperty(PropertyName = "matched_queries")]
		public IEnumerable<string> MatchedQueries { get; internal set; }
	}
}
