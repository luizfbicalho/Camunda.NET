using System;

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
namespace org.camunda.bpm.engine.impl.util.xml
{
	using SAXParseException = org.xml.sax.SAXParseException;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class Problem
	{

	  protected internal string errorMessage;
	  protected internal string resource;
	  protected internal int line;
	  protected internal int column;

	  public Problem(SAXParseException e, string resource)
	  {
		concatenateErrorMessages(e);
		this.resource = resource;
		this.line = e.LineNumber;
		this.column = e.ColumnNumber;
	  }

	  public Problem(string errorMessage, string resourceName, Element element)
	  {
		this.errorMessage = errorMessage;
		this.resource = resourceName;
		if (element != null)
		{
		  this.line = element.Line;
		  this.column = element.Column;
		}
	  }

	  public Problem(BpmnParseException exception, string resourceName)
	  {
		concatenateErrorMessages(exception);
		this.resource = resourceName;
		Element element = exception.Element;
		if (element != null)
		{
		  this.line = element.Line;
		  this.column = element.Column;
		}
	  }

	  protected internal virtual void concatenateErrorMessages(Exception throwable)
	  {
		while (throwable != null)
		{
		  if (string.ReferenceEquals(errorMessage, null))
		  {
			errorMessage = throwable.Message;
		  }
		  else
		  {
			errorMessage += ": " + throwable.Message;
		  }
		  throwable = throwable.InnerException;
		}
	  }

	  public override string ToString()
	  {
		return errorMessage + (!string.ReferenceEquals(resource, null) ? " | " + resource : "") + " | line " + line + " | column " + column;
	  }
	}

}