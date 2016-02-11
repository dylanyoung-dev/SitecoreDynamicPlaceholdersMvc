using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetChromeData;
using Sitecore.Web.UI.PageModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Source.Pipelines.Renderings
{
    public class GetDynamicPlaceholderChromeData : GetChromeDataProcessor
    {
        public override void Process(GetChromeDataArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.ChromeData, "Chrome Data");
            if (!"placeholder".Equals(args.ChromeType, StringComparison.OrdinalIgnoreCase))
                return;

            string placeholderKey = args.CustomData["placeHolderKey"] as string;

            if (!string.IsNullOrEmpty(placeholderKey) && placeholderKey.Contains('|'))
            {
                string[] arrKey = placeholderKey.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (arrKey != null && arrKey.Length > 1)
                {
                    placeholderKey = arrKey[0];
                }
            }

            // Handles Replacing the Displayname of the Placeholder area
            Item item = null;
            if (args.Item != null)
            {
                string layout = ChromeContext.GetLayout(args.Item);
                item = Sitecore.Client.Page.GetPlaceholderItem(placeholderKey, args.Item.Database, layout);
                if (item != null)
                {
                    args.ChromeData.DisplayName = item.DisplayName;
                }
                if ((item != null) && !string.IsNullOrEmpty(item.Appearance.ShortDescription))
                {
                    args.ChromeData.ExpandedDisplayName = item.Appearance.ShortDescription;
                }
            }
        }
    }
}
