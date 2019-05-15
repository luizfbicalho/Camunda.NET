﻿/*
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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using ProcessTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.ProcessTaskActivityBehavior;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessTaskItemHandler : ProcessOrCaseTaskItemHandler
	{

	  protected internal override CmmnActivityBehavior ActivityBehavior
	  {
		  get
		  {
			return new ProcessTaskActivityBehavior();
		  }
	  }

	  protected internal override ProcessTask getDefinition(CmmnElement element)
	  {
		return (ProcessTask) base.getDefinition(element);
	  }

	  protected internal override string getDefinitionKey(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		ProcessTask definition = getDefinition(element);

		return definition.Process;
	  }

	  protected internal override string getBinding(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		ProcessTask definition = getDefinition(element);

		return definition.CamundaProcessBinding;
	  }

	  protected internal override string getVersion(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		ProcessTask definition = getDefinition(element);

		return definition.CamundaProcessVersion;
	  }

	  protected internal override string getTenantId(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		ProcessTask definition = getDefinition(element);

		return definition.CamundaProcessTenantId;
	  }

	}

}