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
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DeleteJobsCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<string> jobIds;
	  protected internal bool cascade;

	  public DeleteJobsCmd(IList<string> jobIds) : this(jobIds, false)
	  {
	  }

	  public DeleteJobsCmd(IList<string> jobIds, bool cascade)
	  {
		this.jobIds = jobIds;
		this.cascade = cascade;
	  }

	  public DeleteJobsCmd(string jobId) : this(jobId, false)
	  {
	  }

	  public DeleteJobsCmd(string jobId, bool cascade)
	  {
		this.jobIds = new List<string>();
		jobIds.Add(jobId);
		this.cascade = cascade;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		JobEntity jobToDelete = null;
		foreach (string jobId in jobIds)
		{
		  jobToDelete = Context.CommandContext.JobManager.findJobById(jobId);

		  if (jobToDelete != null)
		  {
			// When given job doesn't exist, ignore
			jobToDelete.delete();

			if (cascade)
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			}
		  }
		}
		return null;
	  }
	}

}