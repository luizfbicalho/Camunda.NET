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
namespace org.camunda.bpm.engine.impl.cmd
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormField = org.camunda.bpm.engine.form.FormField;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author  Daniel Meyer
	/// </summary>
	[Serializable]
	public abstract class AbstractGetFormVariablesCmd : Command<VariableMap>
	{
		public abstract T execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext);

	  private const long serialVersionUID = 1L;

	  public string resourceId;
	  public ICollection<string> formVariableNames;
	  protected internal bool deserializeObjectValues;

	  public AbstractGetFormVariablesCmd(string resourceId, ICollection<string> formVariableNames, bool deserializeObjectValues)
	  {
		this.resourceId = resourceId;
		this.formVariableNames = formVariableNames;
		this.deserializeObjectValues = deserializeObjectValues;
	  }

	  protected internal virtual TypedValue createVariable(FormField formField, VariableScope variableScope)
	  {
		TypedValue value = formField.Value;

		if (value != null)
		{
		  return value;
		}
		else
		{
		  return null;
		}

	  }

	}
}