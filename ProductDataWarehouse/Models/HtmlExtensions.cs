﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;

namespace ProductDataWarehouse.Models
{
	public static class HtmlExtensions
	{
		public static MvcHtmlString MJLabelFor<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool bAddColon )
		{
			return MJLabelFor( html, expression, bAddColon, null );
		}

		public static MvcHtmlString MJLabelFor<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool bAddColon, object htmlAttributes )
		{
			return MJLabelFor( html, expression, bAddColon, new RouteValueDictionary( htmlAttributes ) );
		}

		public static MvcHtmlString MJLabelFor<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool bAddColon, IDictionary<string, object> htmlAttributes )
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression( expression, html.ViewData );
			string htmlFieldName = ExpressionHelper.GetExpressionText( expression );
			string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split( '.' ).Last();
			if( String.IsNullOrEmpty( labelText ) )
			{
				return MvcHtmlString.Empty;
			}

			TagBuilder tag = new TagBuilder( "label" );
			tag.MergeAttributes( htmlAttributes );
			tag.Attributes.Add( "for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId( htmlFieldName ) );
			tag.SetInnerText( labelText + (bAddColon ? ":" : "") );
			return MvcHtmlString.Create( tag.ToString( TagRenderMode.Normal ) );
		}
	}
}