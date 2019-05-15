﻿using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.form.engine
{

	/// <summary>
	/// <para>The <seealso cref="HtmlDocumentBuilder"/> is part of the <seealso cref="HtmlFormEngine"/>
	/// and maintains a stack of element which are written out to a <seealso cref="StringWriter"/>.</para>
	/// 
	/// <para>Actual writing of the html elements is delegated to the <seealso cref="HtmlElementWriter"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HtmlDocumentBuilder
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			context = new HtmlWriteContext(this);
		}


	  protected internal HtmlWriteContext context;

	  protected internal Stack<HtmlElementWriter> elements = new Stack<HtmlElementWriter>();
	  protected internal StringWriter writer = new StringWriter();

	  public HtmlDocumentBuilder(HtmlElementWriter documentElement)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		startElement(documentElement);
	  }

	  public virtual HtmlDocumentBuilder startElement(HtmlElementWriter renderer)
	  {
		renderer.writeStartTag(context);
		elements.Push(renderer);
		return this;
	  }

	  public virtual HtmlDocumentBuilder endElement()
	  {
		HtmlElementWriter renderer = elements.Pop();
		renderer.writeContent(context);
		renderer.writeEndTag(context);
		return this;
	  }

	  public virtual string HtmlString
	  {
		  get
		  {
			return writer.ToString();
		  }
	  }

	  public class HtmlWriteContext
	  {
		  private readonly HtmlDocumentBuilder outerInstance;

		  public HtmlWriteContext(HtmlDocumentBuilder outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public virtual StringWriter Writer
		{
			get
			{
			  return outerInstance.writer;
			}
		}

		public virtual int ElementStackSize
		{
			get
			{
			  return outerInstance.elements.Count;
			}
		}
	  }
	}
}