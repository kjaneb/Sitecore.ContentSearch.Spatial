using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.Spatial.Configurations;

namespace Sitecore.ContentSearch.Spatial.Provider.Lucene
{
	public class SwitchOnRebuildLuceneIndexWithSpatial : SwitchOnRebuildLuceneIndex
	{
		private static SpatialConfigurations spatialConfigurations;

		public SwitchOnRebuildLuceneIndexWithSpatial(string name, string folder, IIndexPropertyStore propertyStore) : base(name, folder, propertyStore)
		{
		}

		public SwitchOnRebuildLuceneIndexWithSpatial(string name) : base(name)
		{
		}

		public override IProviderSearchContext CreateSearchContext(SearchSecurityOptions securityOptions = SearchSecurityOptions.EnableSecurityCheck)
		{
			this.EnsureInitialized();
			return new LuceneSearchWithSpatialContext(this, securityOptions);
		}
	}
}
