﻿using System;
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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class GetIdentityLinksForProcessDefinitionCmd : Command<IList<IdentityLink>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId;

	  public GetIdentityLinksForProcessDefinitionCmd(string processDefinitionId)
	  {
		this.processDefinitionId = processDefinitionId;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public java.util.List<org.camunda.bpm.engine.task.IdentityLink> execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual IList<IdentityLink> execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = Context.CommandContext.ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);

		ensureNotNull("Cannot find process definition with id " + processDefinitionId, "processDefinition", processDefinition);

		IList<IdentityLink> identityLinks = (System.Collections.IList) processDefinition.IdentityLinks;
		return identityLinks;
	  }

	}

}