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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	[Serializable]
	public abstract class AbstractSetVariableCmd : AbstractVariableCmd
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, ? extends Object> variables;
	  protected internal IDictionary<string, ? extends object> variables;

	  public AbstractSetVariableCmd<T1>(string entityId, IDictionary<T1> variables, bool isLocal) where T1 : object : base(entityId, isLocal)
	  {
		this.variables = variables;
	  }

	  protected internal override void executeOperation(AbstractVariableScope scope)
	  {
		if (isLocal)
		{
		  scope.setVariablesLocal(variables);
		}
		else
		{
		  scope.setVariables(variables);
		}
	  }

	  protected internal override string LogEntryOperation
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE;
		  }
	  }
	}

}