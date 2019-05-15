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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// Command to delete process definitions by a given key.
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	[Serializable]
	public class DeleteProcessDefinitionsByKeyCmd : AbstractDeleteProcessDefinitionCmd
	{

	  private const long serialVersionUID = 1L;

	  private readonly string processDefinitionKey;
	  private readonly string tenantId;
	  private readonly bool isTenantIdSet;

	  public DeleteProcessDefinitionsByKeyCmd(string processDefinitionKey, bool cascade, bool skipCustomListeners, bool skipIoMappings, string tenantId, bool isTenantIdSet)
	  {
		this.processDefinitionKey = processDefinitionKey;
		this.cascade = cascade;
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;
		this.tenantId = tenantId;
		this.isTenantIdSet = isTenantIdSet;
	  }

	  public override Void execute(CommandContext commandContext)
	  {
		ensureNotNull("processDefinitionKey", processDefinitionKey);

		IList<ProcessDefinition> processDefinitions = commandContext.ProcessDefinitionManager.findDefinitionsByKeyAndTenantId(processDefinitionKey, tenantId, isTenantIdSet);
		ensureNotEmpty(typeof(NotFoundException), "No process definition found with key '" + processDefinitionKey + "'", "processDefinitions", processDefinitions);

		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  string processDefinitionId = processDefinition.Id;
		  deleteProcessDefinitionCmd(commandContext, processDefinitionId, cascade, skipCustomListeners, skipIoMappings);
		}

		return null;
	  }

	}

}