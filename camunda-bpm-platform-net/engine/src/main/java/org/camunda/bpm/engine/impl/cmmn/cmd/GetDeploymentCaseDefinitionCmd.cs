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
namespace org.camunda.bpm.engine.impl.cmmn.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class GetDeploymentCaseDefinitionCmd : Command<CaseDefinition>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string caseDefinitionId;

	  public GetDeploymentCaseDefinitionCmd(string caseDefinitionId)
	  {
		this.caseDefinitionId = caseDefinitionId;
	  }

	  public virtual CaseDefinition execute(CommandContext commandContext)
	  {
		ensureNotNull("caseDefinitionId", caseDefinitionId);

		CaseDefinitionEntity caseDefinition = Context.ProcessEngineConfiguration.DeploymentCache.findDeployedCaseDefinitionById(caseDefinitionId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadCaseDefinition(caseDefinition);
		}

		return caseDefinition;
	  }

	}

}