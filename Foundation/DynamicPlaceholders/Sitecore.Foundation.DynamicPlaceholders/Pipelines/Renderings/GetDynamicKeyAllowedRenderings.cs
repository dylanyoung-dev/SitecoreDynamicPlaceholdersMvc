using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetPlaceholderRenderings;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Foundation.DynamicPlaceholders.Pipelines.Renderings
{
    public class GetDynamicKeyAllowedRenderings
    {
        private GetAllowedRenderings getRenderings = new GetAllowedRenderings();
        public void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull((object)args, "args");

            string initialKey = args.PlaceholderKey;
            if (initialKey.Contains("/"))
                initialKey = initialKey.Substring(args.PlaceholderKey.LastIndexOf('/')).Replace("/", string.Empty);

            if (!string.IsNullOrEmpty(initialKey) && initialKey.Contains("|"))
            {
                string[] arrKey = initialKey.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrKey != null && arrKey.Length > 1)
                {
                    string finalKey = arrKey[0];
                    GetPlaceholderRenderingsArgs basePlaceholderArgs = args.DeviceId.IsNull ? new GetPlaceholderRenderingsArgs(finalKey, args.LayoutDefinition, args.ContentDatabase)
                    : new GetPlaceholderRenderingsArgs(finalKey, args.LayoutDefinition, args.ContentDatabase, args.DeviceId);
                    this.getRenderings.Process(basePlaceholderArgs);
                    this.AssimilateArgumentResults(ref args, basePlaceholderArgs);
                    return;
                }
            }

            this.getRenderings.Process(args);
        }

        private void AssimilateArgumentResults(ref GetPlaceholderRenderingsArgs args, GetPlaceholderRenderingsArgs basePlaceholderArgs)
        {
            args.Options.ShowTree = basePlaceholderArgs.Options.ShowTree;

            ((SelectItemOptions)args.Options).ShowRoot = (((SelectItemOptions)basePlaceholderArgs.Options).ShowRoot);

            ((SelectItemOptions)args.Options).SetRootAsSearchRoot = (((SelectItemOptions)basePlaceholderArgs.Options).SetRootAsSearchRoot);

            args.HasPlaceholderSettings = (basePlaceholderArgs.HasPlaceholderSettings);
            args.OmitNonEditableRenderings = (basePlaceholderArgs.OmitNonEditableRenderings);
            args.PlaceholderRenderings = (basePlaceholderArgs.PlaceholderRenderings);
        }
    }
}
