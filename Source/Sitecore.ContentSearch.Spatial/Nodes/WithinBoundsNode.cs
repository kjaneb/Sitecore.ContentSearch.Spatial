using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch.Linq.Nodes;

namespace Sitecore.ContentSearch.Spatial.Linq.Nodes
{
	public class WithinBoundsNode : QueryNode
	{

		public float Boost { get; protected set; }
		public string Field { get; protected set; }
		public object MinLatitude { get; protected set; }
		public object MinLongitude { get; protected set; }
		public object MaxLatitude { get; protected set; }
		public object MaxLongitude { get; protected set; }

		public override QueryNodeType NodeType
		{
			get { return QueryNodeType.LessThanOrEqual; }
		}

		public override IEnumerable<QueryNode> SubNodes
		{
			get
			{
				return new List<QueryNode>();

			}
		}

		public WithinBoundsNode(string field, object minlat, object minlng, object maxlat, object maxlng)
			: this(field, minlat, minlng, maxlat, maxlng, 1f)
		{

		}

		public WithinBoundsNode(string field, object minlat, object minlng, object maxlat, object maxlng, float boost)
		{
			MinLatitude = minlat;
			MinLongitude = minlng;
			MaxLatitude = maxlat;
			MaxLongitude = maxlng;
			Boost = boost;
			Field = field;
		}
	}
}