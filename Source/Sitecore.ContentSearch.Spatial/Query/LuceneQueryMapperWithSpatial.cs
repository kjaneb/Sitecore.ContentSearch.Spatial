﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch.Spatial.Linq.Nodes;
using Lucene.Net.Search;
using Lucene.Net.Search.Function;
using Lucene.Net.Spatial.Prefix;
using Lucene.Net.Spatial.Prefix.Tree;
using Lucene.Net.Spatial.Queries;
using Lucene.Net.Spatial.Util;
using Lucene.Net.Spatial.Vector;
using Sitecore.ContentSearch.Linq.Lucene;
using Spatial4n.Core.Context;
using Spatial4n.Core.Distance;
using Spatial4n.Core.Shapes;

using Lucene.Net.Search.Function;
using Lucene.Net.Spatial.BBox;

namespace Sitecore.ContentSearch.Spatial.Query
{
    public class LuceneQueryMapperWithSpatial :LuceneQueryMapper
    {
        public LuceneQueryMapperWithSpatial(LuceneIndexParameters parameters)
            : base(parameters)
        {
        }

       SpatialContext ctx =  SpatialContext.GEO;
        protected override Lucene.Net.Search.Query Visit(Sitecore.ContentSearch.Linq.Nodes.QueryNode node, LuceneQueryMapperState mappingState)
        {
	        if (node is WithinRadiusNode)
	        {
		        var withinRadiusNode = node as WithinRadiusNode;

		        return VisitWithinRadius(withinRadiusNode, mappingState);
	        }
			else if (node is WithinBoundsNode)
			{
				var withinBoundsNode = node as WithinBoundsNode;

				return VisitWithinBounds(withinBoundsNode, mappingState);
			}
	        else
		        return base.Visit(node, mappingState);
        }

        protected virtual Lucene.Net.Search.Query VisitWithinRadius(WithinRadiusNode node, LuceneQueryMapperState mappingState)
        {
            SpatialContext ctx = SpatialContext.GEO;
           
            var strategy = new PointVectorStrategy(ctx, Sitecore.ContentSearch.Spatial.Common.Constants.LocationFieldName);

            if (node.Latitude is double && node.Longitude is double && node.Radius is double)
            {
	            var distance = DistanceUtils.Dist2Degrees((double) node.Radius, DistanceUtils.EARTH_MEAN_RADIUS_KM);
                Circle circle = ctx.MakeCircle((double)node.Longitude,(double)node.Latitude, distance);

                var spatialArgs = new SpatialArgs(SpatialOperation.IsWithin, circle);
                var dq = strategy.MakeQuery(spatialArgs);

                DistanceReverseValueSource valueSource = new DistanceReverseValueSource(strategy, circle.GetCenter(), distance);
                ValueSourceFilter vsf = new ValueSourceFilter(new QueryWrapperFilter(dq), valueSource, 0, distance);
                var filteredSpatial = new FilteredQuery(new MatchAllDocsQuery(), vsf);
                mappingState.FilterQuery = filteredSpatial;
                Lucene.Net.Search.Query spatialRankingQuery = new FunctionQuery(valueSource);
                Random r = new Random(DateTime.Now.Millisecond);
                var randomNumber = r.Next(10000101,11000101);
                Lucene.Net.Search.Query dummyQuery = Lucene.Net.Search.NumericRangeQuery.NewIntRange("__smallcreateddate", randomNumber, Int32.Parse(DateTime.Now.ToString("yyyyMMdd")), true, true);
                BooleanQuery bq = new BooleanQuery();

                bq.Add(filteredSpatial, Occur.MUST);
                bq.Add(spatialRankingQuery, Occur.MUST);
                bq.Add(dummyQuery, Occur.SHOULD);
                return bq;
            }
            throw new NotSupportedException("Wrong parameters type, Radius, latitude and longitude must be of type double");
        }

		protected virtual Lucene.Net.Search.Query VisitWithinBounds(WithinBoundsNode node, LuceneQueryMapperState mappingState)
		{
			SpatialContext ctx = SpatialContext.GEO;

			var strategy = new PointVectorStrategy(ctx, Sitecore.ContentSearch.Spatial.Common.Constants.LocationFieldName);

			if (node.MinLatitude is double && node.MinLongitude is double && node.MaxLatitude is double && node.MaxLongitude is double)
			{
				//var distance = DistanceUtils.Dist2Degrees((double)node.Radius, DistanceUtils.EARTH_MEAN_RADIUS_KM);
				var rectangle = ctx.MakeRectangle((double)node.MinLongitude, (double)node.MaxLongitude, (double)node.MinLatitude, (double)node.MaxLatitude);

				var spatialArgs = new SpatialArgs(SpatialOperation.IsWithin, rectangle);
				var filter = strategy.MakeFilter(spatialArgs);


				var dq = strategy.MakeQuery(spatialArgs);

				//DistanceReverseValueSource valueSource = new DistanceReverseValueSource(strategy, rectangle.GetCenter(), double.MaxValue);
				//ValueSourceFilter vsf = new ValueSourceFilter(new QueryWrapperFilter(dq), valueSource, 0, double.MaxValue);
				var filteredSpatial = new FilteredQuery(new MatchAllDocsQuery(), filter);
				//mappingState.FilterQuery = filteredSpatial;
				//Lucene.Net.Search.Query spatialRankingQuery = new FunctionQuery(valueSource);
				Random r = new Random(DateTime.Now.Millisecond);
				var randomNumber = r.Next(10000101, 11000101);
				Lucene.Net.Search.Query dummyQuery = Lucene.Net.Search.NumericRangeQuery.NewIntRange("__smallcreateddate", randomNumber, Int32.Parse(DateTime.Now.ToString("yyyyMMdd")), true, true);
				BooleanQuery bq = new BooleanQuery();

				bq.Add(filteredSpatial, Occur.MUST);
				//bq.Add(spatialRankingQuery, Occur.MUST);
				bq.Add(dummyQuery, Occur.SHOULD);
				return bq;
			}
			throw new NotSupportedException("Wrong parameters type, Radius, latitude and longitude must be of type double");
		}
    }
}