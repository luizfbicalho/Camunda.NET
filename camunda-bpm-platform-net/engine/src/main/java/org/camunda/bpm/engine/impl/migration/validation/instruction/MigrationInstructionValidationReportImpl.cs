﻿using System.Collections.Generic;

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

	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationInstructionValidationReport = org.camunda.bpm.engine.migration.MigrationInstructionValidationReport;

	public class MigrationInstructionValidationReportImpl : MigrationInstructionValidationReport
	{

	  protected internal MigrationInstruction migrationInstruction;

	  protected internal IList<string> failures = new List<string>();

	  public MigrationInstructionValidationReportImpl(MigrationInstruction migrationInstruction)
	  {
		this.migrationInstruction = migrationInstruction;
	  }

	  public virtual MigrationInstruction MigrationInstruction
	  {
		  get
		  {
			return migrationInstruction;
		  }
	  }

	  public virtual void addFailure(string failure)
	  {
		failures.Add(failure);
	  }

	  public virtual bool hasFailures()
	  {
		return failures.Count > 0;
	  }

	  public virtual IList<string> Failures
	  {
		  get
		  {
			return failures;
		  }
	  }

	  public override string ToString()
	  {
		return "MigrationInstructionValidationReportImpl{" +
		  "migrationInstruction=" + migrationInstruction +
		  ", failures=" + failures +
		  '}';
	  }

	}

}