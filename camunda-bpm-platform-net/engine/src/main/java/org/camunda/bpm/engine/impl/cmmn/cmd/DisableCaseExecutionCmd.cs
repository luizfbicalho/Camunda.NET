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
namespace org.camunda.bpm.engine.impl.cmmn.cmd
{

	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class DisableCaseExecutionCmd : StateTransitionCaseExecutionCmd
	{

	  private const long serialVersionUID = 1L;

	  public DisableCaseExecutionCmd(string caseExecutionId, IDictionary<string, object> variables, IDictionary<string, object> variablesLocal, ICollection<string> variableDeletions, ICollection<string> variableLocalDeletions) : base(caseExecutionId, variables, variablesLocal, variableDeletions, variableLocalDeletions)
	  {
	  }

	  public DisableCaseExecutionCmd(CaseExecutionCommandBuilderImpl builder) : base(builder)
	  {
	  }

	  protected internal override void performStateTransition(CommandContext commandContext, CaseExecutionEntity caseExecution)
	  {
		caseExecution.disable();
	  }

	}

}