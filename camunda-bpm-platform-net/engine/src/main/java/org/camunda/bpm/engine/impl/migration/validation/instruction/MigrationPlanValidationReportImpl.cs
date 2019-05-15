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
namespace org.camunda.bpm.engine.impl.migration.validation.instruction
{

	using MigrationInstructionValidationReport = org.camunda.bpm.engine.migration.MigrationInstructionValidationReport;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationReport = org.camunda.bpm.engine.migration.MigrationPlanValidationReport;

	public class MigrationPlanValidationReportImpl : MigrationPlanValidationReport
	{

	  protected internal MigrationPlan migrationPlan;
	  protected internal IList<MigrationInstructionValidationReport> instructionReports = new List<MigrationInstructionValidationReport>();

	  public MigrationPlanValidationReportImpl(MigrationPlan migrationPlan)
	  {
		this.migrationPlan = migrationPlan;
	  }

	  public virtual MigrationPlan MigrationPlan
	  {
		  get
		  {
			return migrationPlan;
		  }
	  }

	  public virtual void addInstructionReport(MigrationInstructionValidationReport instructionReport)
	  {
		instructionReports.Add(instructionReport);
	  }

	  public virtual bool hasInstructionReports()
	  {
		return instructionReports.Count > 0;
	  }

	  public virtual IList<MigrationInstructionValidationReport> InstructionReports
	  {
		  get
		  {
			return instructionReports;
		  }
	  }

	  public virtual void writeTo(StringBuilder sb)
	  {
		sb.Append("Migration plan for process definition '").Append(migrationPlan.SourceProcessDefinitionId).Append("' to '").Append(migrationPlan.TargetProcessDefinitionId).Append("' is not valid:\n");

		foreach (MigrationInstructionValidationReport instructionReport in instructionReports)
		{
		  sb.Append("\t Migration instruction ").Append(instructionReport.MigrationInstruction).Append(" is not valid:\n");
		  foreach (string failure in instructionReport.Failures)
		  {
			sb.Append("\t\t").Append(failure).Append("\n");
		  }
		}
	  }

	}

}