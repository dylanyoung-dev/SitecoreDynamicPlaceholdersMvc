using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Pipelines;
using Sitecore.Pipelines.ExecutePageEditorAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Foundation.DynamicPlaceholders.Infrastructure.Processors.Renderings
{
    public class InsertRendering
    {
        public void Process(PipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            IInsertRenderingArgs insertRenderingArgs = args as IInsertRenderingArgs;

            if (insertRenderingArgs == null)
                return;

            Item renderingItem = insertRenderingArgs.RenderingItem;
            Assert.IsNotNull((object)renderingItem, "renderingItem");
            string placeholderKey = insertRenderingArgs.PlaceholderKey;
            Assert.IsNotNullOrEmpty(placeholderKey, "placeholderKey");
            string lastPart = StringUtil.GetLastPart(placeholderKey, '/', placeholderKey);
            string[] arrKey = lastPart.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string lastPartKey = arrKey[0];

            RenderingDefinition renderingDefinition = new RenderingDefinition()
            {
                ItemID = renderingItem.ID.ToString(),
                Placeholder = lastPartKey
            };

            if (insertRenderingArgs.Datasource != null)
                renderingDefinition.Datasource = insertRenderingArgs.Datasource.ID.ToString();

            Assert.IsNotNull((object)insertRenderingArgs.Device, "device");

            this.InsertRenderingAt(insertRenderingArgs.Device, renderingDefinition, insertRenderingArgs.Position, insertRenderingArgs.AllowedRenderingsIds, arrKey[1]);

            insertRenderingArgs.Result = new RenderingReference(renderingDefinition, insertRenderingArgs.Language, insertRenderingArgs.ContentDatabase);
        }

        protected virtual void InsertRenderingAt(DeviceDefinition device, RenderingDefinition renderingDefinition, int insertPosition, IEnumerable<ID> allowedRenderingsIds, string renderingUniqueId = "")
        {
            Assert.ArgumentNotNull((object)device, "device");
            Assert.ArgumentNotNull((object)renderingDefinition, "renderingDefinition");

            // Insert Rendering by it's Unique Id
            if (!string.IsNullOrEmpty(renderingUniqueId) && device != null && device.Renderings != null)
            {
                string uniqueId = string.Concat("{", renderingUniqueId, "}");
                // Try 
                for (int index = 0; index < device.Renderings.Count; ++index)
                {
                    RenderingDefinition rendering = device.Renderings[index] as RenderingDefinition;

                    if (rendering.UniqueId == uniqueId)
                    {
                        device.Insert(index + 1, renderingDefinition);
                    }
                }
            }

            if (insertPosition == 0)
                device.Insert(insertPosition, renderingDefinition);
            else if (device.Renderings == null)
            {
                device.AddRendering(renderingDefinition);
            }
            else
            {
                ID[] idArray = Enumerable.ToArray<ID>(allowedRenderingsIds);
                int num = 0;
                for (int index = 0; index < device.Renderings.Count; ++index)
                {
                    RenderingDefinition rendering = device.Renderings[index] as RenderingDefinition;
                    Assert.IsNotNull((object)rendering, "rendering");
                    string placeholder = rendering.Placeholder;
                    if (string.IsNullOrEmpty(placeholder) && Enumerable.Any<ID>((IEnumerable<ID>)idArray, (Func<ID, bool>)(p => p == ID.Parse(rendering.ItemID))))
                        placeholder = renderingDefinition.Placeholder;

                    if (InsertRendering.AreEqualPlaceholders(placeholder, renderingDefinition.Placeholder))
                        ++num;

                    if (num == insertPosition)
                    {
                        device.Insert(index + 1, renderingDefinition);
                        break;
                    }
                }
            }
        }

        private static bool AreEqualPlaceholders(string lhsPlaceholderKey, string rhsPlaceholderKey)
        {
            if (lhsPlaceholderKey == null || rhsPlaceholderKey == null)
                return string.Equals(lhsPlaceholderKey, rhsPlaceholderKey, StringComparison.InvariantCulture);

            int num1 = lhsPlaceholderKey.LastIndexOf('/');
            int num2 = rhsPlaceholderKey.LastIndexOf('/');
            if (num1 >= 0 && num2 >= 0)
                return lhsPlaceholderKey.Equals(rhsPlaceholderKey, StringComparison.InvariantCultureIgnoreCase);

            return (num1 >= 0 ? StringUtil.Mid(lhsPlaceholderKey, num1 + 1) : lhsPlaceholderKey).Equals(num2 >= 0 ? StringUtil.Mid(rhsPlaceholderKey, num2 + 1) : rhsPlaceholderKey, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
