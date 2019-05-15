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
namespace org.camunda.bpm.engine.rest.dto.task
{
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;
	using TaskReport = org.camunda.bpm.engine.task.TaskReport;


	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class TaskCountByCandidateGroupResultDto
	{

	  protected internal string groupName;
	  protected internal int taskCount;

	  public virtual int TaskCount
	  {
		  get
		  {
			return taskCount;
		  }
		  set
		  {
			this.taskCount = value;
		  }
	  }


	  public virtual string GroupName
	  {
		  get
		  {
			return groupName;
		  }
		  set
		  {
			this.groupName = value;
		  }
	  }


	  public static TaskCountByCandidateGroupResultDto fromTaskCountByCandidateGroupResultDto(TaskCountByCandidateGroupResult taskCountByCandidateGroupResult)
	  {
		TaskCountByCandidateGroupResultDto dto = new TaskCountByCandidateGroupResultDto();

		dto.groupName = taskCountByCandidateGroupResult.GroupName;
		dto.taskCount = taskCountByCandidateGroupResult.TaskCount;

		return dto;
	  }

	  public virtual IList<TaskCountByCandidateGroupResult> executeTaskCountByCandidateGroupReport(ProcessEngine engine)
	  {
		TaskReport reportQuery = engine.TaskService.createTaskReport();

		try
		{
		  return reportQuery.taskCountByCandidateGroup();
		}
		catch (NotValidException e)
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, e, e.Message);
		}
	  }
	}

}