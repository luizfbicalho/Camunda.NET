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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[Serializable]
	public class SetJobsRetriesCmd : AbstractSetJobRetriesCmd, Command<Void>
	{
	  protected internal readonly IList<string> jobIds;
	  protected internal readonly int retries;

	  public SetJobsRetriesCmd(IList<string> jobIds, int retries)
	  {
		EnsureUtil.ensureNotEmpty("Job ID's", jobIds);
		EnsureUtil.ensureGreaterThanOrEqual("Retries count", retries, 0);

		this.jobIds = jobIds;
		this.retries = retries;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		foreach (string id in jobIds)
		{
		  setJobRetriesByJobId(id, retries, commandContext);
		}
		return null;
	  }
	}

}