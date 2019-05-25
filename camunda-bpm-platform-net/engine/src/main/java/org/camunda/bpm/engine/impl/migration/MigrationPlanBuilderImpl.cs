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

	using MigrationPlanBuilder = org.camunda.bpm.engine.migration.MigrationPlanBuilder;
	using CreateMigrationPlanCmd = org.camunda.bpm.engine.impl.cmd.CreateMigrationPlanCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using MigrationInstructionBuilder = org.camunda.bpm.engine.migration.MigrationInstructionBuilder;
	using MigrationInstructionsBuilder = org.camunda.bpm.engine.migration.MigrationInstructionsBuilder;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationPlanBuilderImpl : MigrationInstructionBuilder, MigrationInstructionsBuilder
	{

	  protected internal CommandExecutor commandExecutor;

	  protected internal string sourceProcessDefinitionId;
	  protected internal string targetProcessDefinitionId;
	  protected internal IList<MigrationInstructionImpl> explicitMigrationInstructions;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool mapEqualActivities_Conflict = false;
	  protected internal bool updateEventTriggersForGeneratedInstructions = false;

	  public MigrationPlanBuilderImpl(CommandExecutor commandExecutor, string sourceProcessDefinitionId, string targetProcessDefinitionId)
	  {
		this.commandExecutor = commandExecutor;
		this.sourceProcessDefinitionId = sourceProcessDefinitionId;
		this.targetProcessDefinitionId = targetProcessDefinitionId;
		this.explicitMigrationInstructions = new List<MigrationInstructionImpl>();
	  }

	  public virtual MigrationInstructionsBuilder mapEqualActivities()
	  {
		this.mapEqualActivities_Conflict = true;
		return this;
	  }

	  public virtual MigrationInstructionBuilder mapActivities(string sourceActivityId, string targetActivityId)
	  {
		this.explicitMigrationInstructions.add(new MigrationInstructionImpl(sourceActivityId, targetActivityId)
	   );
		return this;
	  }

	  public virtual MigrationInstructionBuilder updateEventTrigger()
	  {
		explicitMigrationInstructions[explicitMigrationInstructions.Count - 1].UpdateEventTrigger = true;
		return this;
	  }

	  public virtual MigrationInstructionsBuilder updateEventTriggers()
	  {
		this.updateEventTriggersForGeneratedInstructions = true;
		return this;
	  }

	  public virtual string SourceProcessDefinitionId
	  {
		  get
		  {
			return sourceProcessDefinitionId;
		  }
	  }

	  public virtual string TargetProcessDefinitionId
	  {
		  get
		  {
			return targetProcessDefinitionId;
		  }
	  }

	  public virtual bool MapEqualActivities
	  {
		  get
		  {
			return mapEqualActivities_Conflict;
		  }
	  }

	  public virtual bool UpdateEventTriggersForGeneratedInstructions
	  {
		  get
		  {
			return updateEventTriggersForGeneratedInstructions;
		  }
	  }

	  public virtual IList<MigrationInstructionImpl> ExplicitMigrationInstructions
	  {
		  get
		  {
			return explicitMigrationInstructions;
		  }
	  }

	  public virtual MigrationPlan build()
	  {
		return commandExecutor.execute(new CreateMigrationPlanCmd(this));
	  }

	}

}