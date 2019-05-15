using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.rest.dto.converter
{
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskReportResultToCsvConverter
	{

	  protected internal static string DELIMITER = ",";
	  protected internal static string NEW_LINE_SEPARATOR = "\n";

	  public static string CANDIDATE_GROUP_HEADER = "CANDIDATE_GROUP_NAME"
													+ DELIMITER + "TASK_COUNT";

	  public static string convertCandidateGroupReportResult(IList<TaskCountByCandidateGroupResult> reports)
	  {
		StringBuilder buffer = new StringBuilder();

		buffer.Append(CANDIDATE_GROUP_HEADER);

		foreach (TaskCountByCandidateGroupResult report in reports)
		{
		  buffer.Append(NEW_LINE_SEPARATOR);
		  buffer.Append(report.GroupName);
		  buffer.Append(DELIMITER);
		  buffer.Append(report.TaskCount);
		}

		return buffer.ToString();
	  }

	}

}