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
namespace org.camunda.bpm.engine.rest.dto.migration
{

	using MigratingProcessInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationReport;

	public class MigratingProcessInstanceValidationReportDto
	{

	  protected internal string processInstanceId;
	  protected internal IList<string> failures;
	  protected internal IList<MigratingActivityInstanceValidationReportDto> activityInstanceValidationReports;
	  protected internal IList<MigratingTransitionInstanceValidationReportDto> transitionInstanceValidationReports;

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual IList<string> Failures
	  {
		  get
		  {
			return failures;
		  }
		  set
		  {
			this.failures = value;
		  }
	  }


	  public virtual IList<MigratingActivityInstanceValidationReportDto> ActivityInstanceValidationReports
	  {
		  get
		  {
			return activityInstanceValidationReports;
		  }
		  set
		  {
			this.activityInstanceValidationReports = value;
		  }
	  }


	  public virtual IList<MigratingTransitionInstanceValidationReportDto> TransitionInstanceValidationReports
	  {
		  get
		  {
			return transitionInstanceValidationReports;
		  }
		  set
		  {
			this.transitionInstanceValidationReports = value;
		  }
	  }


	  public static MigratingProcessInstanceValidationReportDto from(MigratingProcessInstanceValidationReport validationReport)
	  {
		MigratingProcessInstanceValidationReportDto dto = new MigratingProcessInstanceValidationReportDto();
		dto.ProcessInstanceId = validationReport.ProcessInstanceId;
		dto.Failures = validationReport.Failures;
		dto.ActivityInstanceValidationReports = MigratingActivityInstanceValidationReportDto.from(validationReport.ActivityInstanceReports);
		dto.TransitionInstanceValidationReports = MigratingTransitionInstanceValidationReportDto.from(validationReport.TransitionInstanceReports);
		return dto;
	  }

	}

}