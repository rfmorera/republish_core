using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Republish.TagHelpers
{
    [HtmlTargetElement(Attributes = "is-active-route")]
    public class ActiveRouteTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ShouldBeActive())
            {
                MakeActive(output);
            }

            output.Attributes.RemoveAll("is-active-route");
        }

        private bool ShouldBeActive()
        {
            string currentArea = GetRouteValue("area");
            string currentController = GetRouteValue("controller");
            string currentAction = GetRouteValue("action");

            var isValid = !string.IsNullOrWhiteSpace(Area) &&
                !string.IsNullOrWhiteSpace(Controller) &&
                !string.IsNullOrWhiteSpace(currentArea) &&
                !string.IsNullOrWhiteSpace(currentController) &&
                !string.IsNullOrWhiteSpace(currentAction);

            return isValid &&
                Area.ToLower() == currentArea.ToLower() &&
                Controller.ToLower() == currentController.ToLower() &&
                (string.IsNullOrWhiteSpace(Action) || Action.ToLower() == currentAction.ToLower());
        }

        private void MakeActive(TagHelperOutput output)
        {
            var classAttribute = output.Attributes.FirstOrDefault(a => a.Name == "class");

            if (classAttribute == null)
            {
                classAttribute = new TagHelperAttribute("class", "active");
                output.Attributes.Add(classAttribute);
            }
            else if (classAttribute.Value == null)
            {
                output.Attributes.SetAttribute("class", "active");
            }
            else if (classAttribute.Value.ToString().IndexOf("active") < 0)
            {
                output.Attributes.SetAttribute("class", classAttribute.Value.ToString() + " active");
            }
        }

        private string GetRouteValue(string key)
        {
            string value;

            if (ViewContext.RouteData.Values.TryGetValue(key, out object valueObject))
            {
                value = valueObject.ToString();
            }
            else
            {
                value = null;
            }

            return value;
        }
    }
}