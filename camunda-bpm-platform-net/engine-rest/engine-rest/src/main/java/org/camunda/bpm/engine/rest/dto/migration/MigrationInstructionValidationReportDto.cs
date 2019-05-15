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

	using MigrationInstructionValidationReport = org.camunda.bpm.engine.migration.MigrationInstructionValidationReport;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationInstructionValidationReportDto
	{

	  protected internal MigrationInstructionDto instruction;
	  protected internal IList<string> failures;

	  public virtual MigrationInstructionDto Instruction
	  {
		  get
		  {
			return instruction;
		  }
		  set
		  {
			this.instruction = value;
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


	  public static IList<MigrationInstructionValidationReportDto> from(IList<MigrationInstructionValidationReport> instructionReports)
	  {
		IList<MigrationInstructionValidationReportDto> dtos = new List<MigrationInstructionValidationReportDto>();
		foreach (MigrationInstructionValidationReport instructionReport in instructionReports)
		{
		  dtos.Add(MigrationInstructionValidationReportDto.from(instructionReport));
		}
		return dtos;
	  }

	  public static MigrationInstructionValidationReportDto from(MigrationInstructionValidationReport instructionReport)
	  {
		MigrationInstructionValidationReportDto dto = new MigrationInstructionValidationReportDto();
		dto.Instruction = MigrationInstructionDto.from(instructionReport.MigrationInstruction);
		dto.Failures = instructionReport.Failures;
		return dto;
	  }

	}

}