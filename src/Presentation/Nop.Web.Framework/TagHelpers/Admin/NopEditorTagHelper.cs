﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-editor" tag helper
    /// </summary>
    [HtmlTargetElement("nop-editor", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class NopEditorTagHelper : TagHelper
    {
        #region Constants

        private const string FOR_ATTRIBUTE_NAME = "asp-for";
        private const string CUSTOM_HTML_ATTRIBUTES = "html-attributes";
        private const string REQUIRED_ATTRIBUTE_NAME = "asp-required";
        private const string RENDER_FORM_CONTROL_CLASS_ATTRIBUTE_NAME = "asp-render-form-control-class";
        private const string TEMPLATE_ATTRIBUTE_NAME = "asp-template";
        private const string POSTFIX_ATTRIBUTE_NAME = "asp-postfix";

        #endregion

        #region Properties

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Custom html attributes
        /// </summary>
        [HtmlAttributeName(CUSTOM_HTML_ATTRIBUTES)]
        public object CustomHtmlAttributes { set; get; }

        /// <summary>
        /// Indicates whether the field is required
        /// </summary>
        [HtmlAttributeName(REQUIRED_ATTRIBUTE_NAME)]
        public string IsRequired { set; get; }

        /// <summary>
        /// Indicates whether the "form-control" class shold be added to the input
        /// </summary>
        [HtmlAttributeName(RENDER_FORM_CONTROL_CLASS_ATTRIBUTE_NAME)]
        public string RenderFormControlClass { set; get; }

        /// <summary>
        /// Editor template for the field
        /// </summary>
        [HtmlAttributeName(TEMPLATE_ATTRIBUTE_NAME)]
        public string Template { set; get; }

        /// <summary>
        /// Postfix
        /// </summary>
        [HtmlAttributeName(POSTFIX_ATTRIBUTE_NAME)]
        public string Postfix { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion

        #region Fields

        protected IHtmlHelper HtmlHelper { get; }

        #endregion

        #region Ctor

        public NopEditorTagHelper(IHtmlHelper htmlHelper)
        {
            HtmlHelper = htmlHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously executes the tag helper with the given context and output
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //clear the output
            output.SuppressOutput();

            //container for additional attributes
            var htmlAttributes = new Dictionary<string, object>();

            //set custom html attributes
            var htmlAttributesDictionary = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper.AnonymousObjectToHtmlAttributes(CustomHtmlAttributes);
            if (htmlAttributesDictionary?.Count > 0)
            {
                foreach (var (key, value) in htmlAttributesDictionary)
                {
                    htmlAttributes.Add(key, value);
                }
            }

            //required asterisk
            if (bool.TryParse(IsRequired, out var required) && required)
            {
                output.PreElement.SetHtmlContent("<div class='input-group input-group-required'>");
                output.PostElement.SetHtmlContent("<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>");
            }

            //contextualize IHtmlHelper
            var viewContextAware = HtmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //add form-control class
            bool.TryParse(RenderFormControlClass, out var renderFormControlClass);
            if (string.IsNullOrEmpty(RenderFormControlClass) && For.Metadata.ModelType.Name.Equals("String") || renderFormControlClass)
                htmlAttributes.Add("class", "form-control");

            //generate editor
            var pattern = $"{nameof(ILocalizedModel<object>.Locales)}" + @"(?=\[\w+\]\.)";
            if (!HtmlHelper.ViewData.ContainsKey(For.Name) && Regex.IsMatch(For.Name, pattern))
            {
                var prefix = HtmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
                var key = string.IsNullOrEmpty(prefix) ? For.Name : $"{prefix}.{For.Name}";
                HtmlHelper.ViewData.Add(key, For.Model);
            }

            var htmlOutput = HtmlHelper.Editor(For.Name, Template, new { htmlAttributes, postfix = Postfix });
            output.Content.SetHtmlContent(htmlOutput);

            return Task.CompletedTask;
        }

        #endregion
    }
}