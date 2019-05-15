using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.cmmn.model
{

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CmmnSentryDeclaration
	{

	  private const long serialVersionUID = 1L;

	  public const string PLAN_ITEM_ON_PART = "planItemOnPart";
	  public const string IF_PART = "ifPart";
	  public const string VARIABLE_ON_PART = "variableOnPart";

	  protected internal string id;

	  protected internal IDictionary<string, IList<CmmnOnPartDeclaration>> onPartMap = new Dictionary<string, IList<CmmnOnPartDeclaration>>();
	  protected internal IList<CmmnOnPartDeclaration> onParts = new List<CmmnOnPartDeclaration>();
	  protected internal IList<CmmnVariableOnPartDeclaration> variableOnParts = new List<CmmnVariableOnPartDeclaration>();

	  protected internal CmmnIfPartDeclaration ifPart;

	  public CmmnSentryDeclaration(string id)
	  {
		this.id = id;
	  }

	  // id //////////////////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  // onParts ////////////////////////////////////////////////////////////

	  public virtual IList<CmmnOnPartDeclaration> OnParts
	  {
		  get
		  {
			return onParts;
		  }
	  }

	  public virtual IList<CmmnOnPartDeclaration> getOnParts(string sourceId)
	  {
		return onPartMap[sourceId];
	  }

	  public virtual void addOnPart(CmmnOnPartDeclaration onPart)
	  {
		CmmnActivity source = onPart.Source;
		if (source == null)
		{
		  // do nothing: ignore onPart
		  return;
		}

		string sourceId = source.Id;

		IList<CmmnOnPartDeclaration> onPartDeclarations = onPartMap[sourceId];

		if (onPartDeclarations == null)
		{
		  onPartDeclarations = new List<CmmnOnPartDeclaration>();
		  onPartMap[sourceId] = onPartDeclarations;
		}

		foreach (CmmnOnPartDeclaration onPartDeclaration in onPartDeclarations)
		{
		  if (onPart.StandardEvent.Equals(onPartDeclaration.StandardEvent))
		  {
			// if there already exists an onPartDeclaration which has the
			// same defined standardEvent then ignore this onPartDeclaration.

			if (onPartDeclaration.Sentry == onPart.Sentry)
			{
			  return;
			}

			// but merge the sentryRef into the already existing onPartDeclaration
			if (onPartDeclaration.Sentry == null && onPart.Sentry != null)
			{
			  // According to the specification, when "sentryRef" is specified,
			  // "standardEvent" must have value "exit" (page 39, Table 23).
			  // But there is no further check necessary.
			  onPartDeclaration.Sentry = onPart.Sentry;
			  return;
			}
		  }
		}

		onPartDeclarations.Add(onPart);
		onParts.Add(onPart);

	  }

	  // variableOnParts
	  public virtual void addVariableOnParts(CmmnVariableOnPartDeclaration variableOnPartDeclaration)
	  {
		variableOnParts.Add(variableOnPartDeclaration);
	  }

	  public virtual bool hasVariableOnPart(string variableEventName, string variableName)
	  {
		foreach (CmmnVariableOnPartDeclaration variableOnPartDeclaration in variableOnParts)
		{
		  if (variableOnPartDeclaration.VariableEvent.Equals(variableEventName) && variableOnPartDeclaration.VariableName.Equals(variableName))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public virtual IList<CmmnVariableOnPartDeclaration> VariableOnParts
	  {
		  get
		  {
			return variableOnParts;
		  }
	  }

	  // ifPart //////////////////////////////////////////////////////////////////

	  public virtual CmmnIfPartDeclaration IfPart
	  {
		  get
		  {
			return ifPart;
		  }
		  set
		  {
			this.ifPart = value;
		  }
	  }


	}

}