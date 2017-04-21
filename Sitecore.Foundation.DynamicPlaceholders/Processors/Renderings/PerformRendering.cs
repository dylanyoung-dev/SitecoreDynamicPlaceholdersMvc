using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.RenderPlaceholder;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Foundation.DynamicPlaceholders.Infrastructure.Processors.Renderings
{
    public class PerformRendering : RenderPlaceholderProcessor
    {
        public override void Process(RenderPlaceholderArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            this.Render(args.PlaceholderName, args.Writer, args);
        }

        protected virtual IEnumerable<Rendering> GetRenderings(string placeholderName, RenderPlaceholderArgs args)
        {
            Guid deviceId = this.GetPageDeviceId(args);
            var childRenderings = new List<Rendering>();

            if (placeholderName.Contains("|"))
            {
                string[] placeholderParts = placeholderName.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);         // 0 = Placeholder Name, 1 = UniqueId (For the Current Rendering)

                List<Rendering> lstRenderings = args.PageContext.PageDefinition.Renderings;

                if (placeholderParts.Count() > 1)
                {
                    string renderingId = string.Empty;
                    bool foundCurrentRendering = false;
                    foreach (Rendering rendering in lstRenderings)
                    {
                        if (rendering.DeviceId != deviceId)
                            continue;

                        if (rendering.UniqueId.ToString().Equals(placeholderParts[1]) || foundCurrentRendering)
                        {
                            // Store RenderingItemPath which is common across all renderings of this type
                            if (!foundCurrentRendering)
                                renderingId = rendering.RenderingItemPath;

                            // Exit Foreach When Next Rendering of Same Type as the current one is found
                            if (foundCurrentRendering && rendering.RenderingItemPath.Equals(renderingId))
                                break;

                            // Only Add Renderings that match Placeholdername
                            if (rendering.Placeholder.Equals(placeholderParts[0]))
                            {
                                childRenderings.Add(rendering);
                            }

                            foundCurrentRendering = true;
                        }
                    }
                }
            }
            else
            {
                string placeholderPath = Sitecore.Mvc.Extensions.StringExtensions.OrEmpty(ObjectExtensions.ValueOrDefault(PlaceholderContext.Current, (Func<PlaceholderContext, string>)(context => context.PlaceholderPath)));
                childRenderings = Enumerable.Where<Rendering>((IEnumerable<Rendering>)args.PageContext.PageDefinition.Renderings, (Func<Rendering, bool>)(r =>
                {
                    if (!(r.DeviceId == deviceId))
                        return false;
                    if (!Sitecore.Mvc.Extensions.StringExtensions.EqualsText(r.Placeholder, placeholderName))
                        return Sitecore.Mvc.Extensions.StringExtensions.EqualsText(r.Placeholder, placeholderPath);
                    return true;
                })).ToList();
            }

            return childRenderings;
        }

        protected virtual Guid GetPageDeviceId(RenderPlaceholderArgs args)
        {
            Guid guid1 = ObjectExtensions.ValueOrDefault<Rendering, Guid>(args.OwnerRendering, (Func<Rendering, Guid>)(rendering => rendering.DeviceId));
            if (guid1 != Guid.Empty)
                return guid1;
            Guid guid2 = ObjectExtensions.ValueOrDefault<Rendering, Guid>(ObjectExtensions.ValueOrDefault<RenderingView, Rendering>(PageContext.Current.PageView as RenderingView, (Func<RenderingView, Rendering>)(view => view.Rendering)), (Func<Rendering, Guid>)(rendering => rendering.DeviceId));
            if (guid2 != Guid.Empty)
                return guid2;
            return Context.Device.ID.ToGuid();
        }

        protected virtual void Render(string placeholderName, TextWriter writer, RenderPlaceholderArgs args)
        {
            IEnumerable<Rendering> renderings = this.GetRenderings(placeholderName, args);
            if (renderings != null)
                foreach (Rendering rendering in renderings)
                    PipelineService.Get().RunPipeline<RenderRenderingArgs>("mvc.renderRendering", new RenderRenderingArgs(rendering, writer));
        }
    }
}
