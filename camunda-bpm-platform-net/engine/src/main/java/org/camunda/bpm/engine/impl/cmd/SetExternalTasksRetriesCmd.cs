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
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	public class SetExternalTasksRetriesCmd : AbstractSetExternalTaskRetriesCmd<Void>
	{

	  public SetExternalTasksRetriesCmd(UpdateExternalTaskRetriesBuilderImpl builder) : base(builder)
	  {
	  }

	  public override Void execute(CommandContext commandContext)
	  {
		IList<string> collectedIds = collectExternalTaskIds();
		EnsureUtil.ensureNotEmpty(typeof(BadUserRequestException), "externalTaskIds", collectedIds);

		int retries = builder.Retries;
		writeUserOperationLog(commandContext, retries, collectedIds.Count, false);

		foreach (string externalTaskId in collectedIds)
		{
		  (new SetExternalTaskRetriesCmd(externalTaskId, retries, false)).execute(commandContext);
		}

		return null;
	  }

	}

}