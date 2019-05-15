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
	public abstract class AbstractRemoveVariableCmd : AbstractVariableCmd
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly ICollection<string> variableNames;

	  public AbstractRemoveVariableCmd(string entityId, ICollection<string> variableNames, bool isLocal) : base(entityId, isLocal)
	  {
		this.variableNames = variableNames;
	  }

	  protected internal override void executeOperation(AbstractVariableScope scope)
	  {
		if (isLocal)
		{
		  scope.removeVariablesLocal(variableNames);
		}
		else
		{
		  scope.removeVariables(variableNames);
		}
	  }

	  protected internal override string LogEntryOperation
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE;
		  }
	  }
	}

}