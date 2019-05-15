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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class GetJobExceptionStacktraceCmd : Command<string>
	{

	  private const long serialVersionUID = 1L;
	  private string jobId;

	  public GetJobExceptionStacktraceCmd(string jobId)
	  {
		this.jobId = jobId;
	  }


	  public virtual string execute(CommandContext commandContext)
	  {
		ensureNotNull("jobId", jobId);

		JobEntity job = commandContext.JobManager.findJobById(jobId);

		ensureNotNull("No job found with id " + jobId, "job", job);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadJob(job);
		}

		return job.ExceptionStacktrace;
	  }


	}

}