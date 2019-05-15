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

	using MigratingTransitionInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingTransitionInstanceValidationReport;

	public class MigratingTransitionInstanceValidationReportDto
	{

	  protected internal MigrationInstructionDto migrationInstruction;
	  protected internal string transitionInstanceId;
	  protected internal string sourceScopeId;
	  protected internal IList<string> failures;

	  public virtual MigrationInstructionDto MigrationInstruction
	  {
		  get
		  {
			return migrationInstruction;
		  }
		  set
		  {
			this.migrationInstruction = value;
		  }
	  }


	  public virtual string TransitionInstanceId
	  {
		  get
		  {
			return transitionInstanceId;
		  }
		  set
		  {
			this.transitionInstanceId = value;
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


	  public virtual string SourceScopeId
	  {
		  get
		  {
			return sourceScopeId;
		  }
		  set
		  {
			this.sourceScopeId = value;
		  }
	  }


	  public static IList<MigratingTransitionInstanceValidationReportDto> from(IList<MigratingTransitionInstanceValidationReport> reports)
	  {
		List<MigratingTransitionInstanceValidationReportDto> dtos = new List<MigratingTransitionInstanceValidationReportDto>();
		foreach (MigratingTransitionInstanceValidationReport report in reports)
		{
		  dtos.Add(MigratingTransitionInstanceValidationReportDto.from(report));
		}
		return dtos;
	  }

	  public static MigratingTransitionInstanceValidationReportDto from(MigratingTransitionInstanceValidationReport report)
	  {
		MigratingTransitionInstanceValidationReportDto dto = new MigratingTransitionInstanceValidationReportDto();
		dto.MigrationInstruction = MigrationInstructionDto.from(report.MigrationInstruction);
		dto.TransitionInstanceId = report.TransitionInstanceId;
		dto.Failures = report.Failures;
		dto.SourceScopeId = report.SourceScopeId;
		return dto;
	  }

	}

}