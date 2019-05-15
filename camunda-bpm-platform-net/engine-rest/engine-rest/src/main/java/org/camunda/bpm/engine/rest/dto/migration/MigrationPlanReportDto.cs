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

	using MigrationPlanValidationReport = org.camunda.bpm.engine.migration.MigrationPlanValidationReport;

	public class MigrationPlanReportDto
	{

	  protected internal IList<MigrationInstructionValidationReportDto> instructionReports;

	  public virtual IList<MigrationInstructionValidationReportDto> InstructionReports
	  {
		  get
		  {
			return instructionReports;
		  }
		  set
		  {
			this.instructionReports = value;
		  }
	  }


	  public static MigrationPlanReportDto form(MigrationPlanValidationReport validationReport)
	  {
		MigrationPlanReportDto dto = new MigrationPlanReportDto();
		dto.InstructionReports = MigrationInstructionValidationReportDto.from(validationReport.InstructionReports);
		return dto;
	  }

	  public static MigrationPlanReportDto emptyReport()
	  {
		MigrationPlanReportDto dto = new MigrationPlanReportDto();
		dto.InstructionReports = System.Linq.Enumerable.Empty<MigrationInstructionValidationReportDto>();
		return dto;
	  }

	}

}