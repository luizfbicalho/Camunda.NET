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
namespace org.camunda.bpm.engine.impl.migration
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using MigrateProcessInstanceBatchCmd = org.camunda.bpm.engine.impl.migration.batch.MigrateProcessInstanceBatchCmd;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanExecutionBuilder = org.camunda.bpm.engine.migration.MigrationPlanExecutionBuilder;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	public class MigrationPlanExecutionBuilderImpl : MigrationPlanExecutionBuilder
	{

	  protected internal CommandExecutor commandExecutor;
	  protected internal MigrationPlan migrationPlan;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<string> processInstanceIds_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ProcessInstanceQuery processInstanceQuery_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool skipCustomListeners_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool skipIoMappings_Renamed;

	  public MigrationPlanExecutionBuilderImpl(CommandExecutor commandExecutor, MigrationPlan migrationPlan)
	  {
		this.commandExecutor = commandExecutor;
		this.migrationPlan = migrationPlan;
	  }

	  public virtual MigrationPlan MigrationPlan
	  {
		  get
		  {
			return migrationPlan;
		  }
	  }

	  public virtual MigrationPlanExecutionBuilder processInstanceIds(IList<string> processInstanceIds)
	  {
		this.processInstanceIds_Renamed = processInstanceIds;
		return this;
	  }

	  public virtual MigrationPlanExecutionBuilder processInstanceIds(params string[] processInstanceIds)
	  {
		if (processInstanceIds == null)
		{
		  this.processInstanceIds_Renamed = Collections.emptyList();
		}
		else
		{
		  this.processInstanceIds_Renamed = Arrays.asList(processInstanceIds);
		}
		return this;
	  }

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds_Renamed;
		  }
	  }

	  public virtual MigrationPlanExecutionBuilder processInstanceQuery(ProcessInstanceQuery processInstanceQuery)
	  {
		this.processInstanceQuery_Renamed = processInstanceQuery;
		return this;
	  }

	  public virtual ProcessInstanceQuery ProcessInstanceQuery
	  {
		  get
		  {
			return processInstanceQuery_Renamed;
		  }
	  }

	  public virtual MigrationPlanExecutionBuilder skipCustomListeners()
	  {
		this.skipCustomListeners_Renamed = true;
		return this;
	  }

	  public virtual bool SkipCustomListeners
	  {
		  get
		  {
			return skipCustomListeners_Renamed;
		  }
	  }

	  public virtual MigrationPlanExecutionBuilder skipIoMappings()
	  {
		this.skipIoMappings_Renamed = true;
		return this;
	  }

	  public virtual bool SkipIoMappings
	  {
		  get
		  {
			return skipIoMappings_Renamed;
		  }
	  }

	  public virtual void execute()
	  {
		execute(true);
	  }

	  public virtual void execute(bool writeOperationLog)
	  {
		commandExecutor.execute(new MigrateProcessInstanceCmd(this, writeOperationLog));
	  }

	  public virtual Batch executeAsync()
	  {
		return commandExecutor.execute(new MigrateProcessInstanceBatchCmd(this));
	  }

	}

}