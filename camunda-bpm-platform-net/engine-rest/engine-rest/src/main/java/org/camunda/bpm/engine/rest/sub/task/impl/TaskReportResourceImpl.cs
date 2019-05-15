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
namespace org.camunda.bpm.engine.rest.sub.task.impl
{
	using TaskReportResultToCsvConverter = org.camunda.bpm.engine.rest.dto.converter.TaskReportResultToCsvConverter;
	using TaskCountByCandidateGroupResultDto = org.camunda.bpm.engine.rest.dto.task.TaskCountByCandidateGroupResultDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;


	public class TaskReportResourceImpl : TaskReportResource
	{

	  public static readonly MediaType APPLICATION_CSV_TYPE = new MediaType("application", "csv");
	  public static readonly MediaType TEXT_CSV_TYPE = new MediaType("text", "csv");
	  public static readonly IList<Variant> VARIANTS = Variant.mediaTypes(MediaType.APPLICATION_JSON_TYPE, APPLICATION_CSV_TYPE, TEXT_CSV_TYPE).add().build();

	  protected internal ProcessEngine engine;

	  public TaskReportResourceImpl(ProcessEngine engine)
	  {
		this.engine = engine;
	  }

	  public virtual Response getTaskCountByCandidateGroupReport(Request request)
	  {
		Variant variant = request.selectVariant(VARIANTS);
		if (variant != null)
		{
		  MediaType mediaType = variant.MediaType;

		  if (MediaType.APPLICATION_JSON_TYPE.Equals(mediaType))
		  {
			IList<TaskCountByCandidateGroupResultDto> result = TaskCountByCandidateGroupResultAsJson;
			return Response.ok(result, mediaType).build();
		  }
		  else if (APPLICATION_CSV_TYPE.Equals(mediaType) || TEXT_CSV_TYPE.Equals(mediaType))
		  {
			string csv = ReportResultAsCsv;
			return Response.ok(csv, mediaType).header("Content-Disposition", "attachment; filename=task-count-by-candidate-group.csv").build();
		  }
		}
		throw new InvalidRequestException(Response.Status.NOT_ACCEPTABLE, "No acceptable content-type found");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult> queryTaskCountByCandidateGroupReport()
	  protected internal virtual IList<TaskCountByCandidateGroupResult> queryTaskCountByCandidateGroupReport()
	  {
		TaskCountByCandidateGroupResultDto reportDto = new TaskCountByCandidateGroupResultDto();
		return (IList<TaskCountByCandidateGroupResult>) reportDto.executeTaskCountByCandidateGroupReport(engine);
	  }

	  protected internal virtual IList<TaskCountByCandidateGroupResultDto> TaskCountByCandidateGroupResultAsJson
	  {
		  get
		  {
			IList<TaskCountByCandidateGroupResult> reports = queryTaskCountByCandidateGroupReport();
			IList<TaskCountByCandidateGroupResultDto> result = new List<TaskCountByCandidateGroupResultDto>();
			foreach (TaskCountByCandidateGroupResult report in reports)
			{
			  result.Add(TaskCountByCandidateGroupResultDto.fromTaskCountByCandidateGroupResultDto(report));
			}
			return result;
		  }
	  }

	  protected internal virtual string ReportResultAsCsv
	  {
		  get
		  {
			IList<TaskCountByCandidateGroupResult> reports = queryTaskCountByCandidateGroupReport();
			return TaskReportResultToCsvConverter.convertCandidateGroupReportResult(reports);
		  }
	  }
	}

}