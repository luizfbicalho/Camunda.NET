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
namespace org.camunda.bpm.engine.impl.migration.validation.instance
{

	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingActivityInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingActivityInstanceValidationReport;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	public class MigratingActivityInstanceValidationReportImpl : MigratingActivityInstanceValidationReport
	{

	  protected internal string activityInstanceId;
	  protected internal string sourceScopeId;
	  protected internal MigrationInstruction migrationInstruction;
	  protected internal IList<string> failures = new List<string>();

	  public MigratingActivityInstanceValidationReportImpl(MigratingActivityInstance migratingActivityInstance)
	  {
		this.activityInstanceId = migratingActivityInstance.ActivityInstance.Id;
		this.sourceScopeId = migratingActivityInstance.SourceScope.Id;
		this.migrationInstruction = migratingActivityInstance.MigrationInstruction;
	  }

	  public virtual string SourceScopeId
	  {
		  get
		  {
			return sourceScopeId;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
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

	}

}