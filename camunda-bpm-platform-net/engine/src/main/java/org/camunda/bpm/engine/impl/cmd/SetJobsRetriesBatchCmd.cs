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
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;


	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class SetJobsRetriesBatchCmd : AbstractSetJobsRetriesBatchCmd
	{
	  protected internal readonly IList<string> jobIds;
	  protected internal readonly JobQuery jobQuery;

	  public SetJobsRetriesBatchCmd(IList<string> jobIds, JobQuery jobQuery, int retries)
	  {
		this.jobQuery = jobQuery;
		this.jobIds = jobIds;
		this.retries = retries;
	  }

	  protected internal override IList<string> collectJobIds(CommandContext commandContext)
	  {
		ISet<string> collectedJobIds = new HashSet<string>();

		IList<string> jobIds = this.JobIds;
		if (jobIds != null)
		{
		  collectedJobIds.addAll(jobIds);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.JobQuery jobQuery = this.jobQuery;
		JobQuery jobQuery = this.jobQuery;
		if (jobQuery != null)
		{
		  foreach (Job job in jobQuery.list())
		  {
			collectedJobIds.Add(job.Id);
		  }
		}

		return new List<string>(collectedJobIds);
	  }

	  public virtual IList<string> JobIds
	  {
		  get
		  {
			return jobIds;
		  }
	  }

	  public virtual int Retries
	  {
		  get
		  {
			return retries;
		  }
	  }

	  public virtual JobQuery JobQuery
	  {
		  get
		  {
			return jobQuery;
		  }
	  }
	}

}