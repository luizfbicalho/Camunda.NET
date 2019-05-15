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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using UpdateProcessInstanceSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.runtime.UpdateProcessInstanceSuspensionStateBuilderImpl;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	public class UpdateProcessInstancesSuspendStateCmd : AbstractUpdateProcessInstancesSuspendStateCmd<Void>
	{

	  public UpdateProcessInstancesSuspendStateCmd(CommandExecutor commandExecutor, UpdateProcessInstancesSuspensionStateBuilderImpl builder, bool suspendstate) : base(commandExecutor, builder, suspendstate)
	  {
	  }

	  public override Void execute(CommandContext commandContext)
	  {

		ICollection<string> processInstanceIds = collectProcessInstanceIds();

		EnsureUtil.ensureNotEmpty(typeof(BadUserRequestException), "No process instance ids given", "Process Instance ids", processInstanceIds);
		EnsureUtil.ensureNotContainsNull(typeof(BadUserRequestException), "Cannot be null.", "Process Instance ids", processInstanceIds);

		writeUserOperationLog(commandContext, processInstanceIds.Count, false);

		UpdateProcessInstanceSuspensionStateBuilderImpl suspensionStateBuilder = new UpdateProcessInstanceSuspensionStateBuilderImpl(commandExecutor);
		if (suspending)
		{
		  // suspending
		  foreach (string processInstanceId in processInstanceIds)
		  {
			suspensionStateBuilder.byProcessInstanceId(processInstanceId).suspend();
		  }
		}
		else
		{
		  // activating
		  foreach (string processInstanceId in processInstanceIds)
		  {
			suspensionStateBuilder.byProcessInstanceId(processInstanceId).activate();
		  }
		}

		return null;
	  }

	}

}