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
namespace org.camunda.bpm.engine.rest.util.migration
{

	using ProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.runtime.ProcessInstanceQueryDto;

	public class MigrationExecutionDtoBuilder
	{

	  public const string PROP_PROCESS_INSTANCE_IDS = "processInstanceIds";
	  public const string PROP_PROCESS_INSTANCE_QUERY = "processInstanceQuery";
	  public const string PROP_MIGRATION_PLAN = "migrationPlan";
	  public const string PROP_SKIP_CUSTOM_LISTENERS = "skipCustomListeners";
	  public const string PROP_SKIP_IO_MAPPINGS = "skipIoMappings";

	  protected internal readonly IDictionary<string, object> migrationExecution;

	  public MigrationExecutionDtoBuilder()
	  {
		migrationExecution = new Dictionary<string, object>();
	  }

	  public virtual MigrationExecutionDtoBuilder processInstances(params string[] processInstanceIds)
	  {
		migrationExecution[PROP_PROCESS_INSTANCE_IDS] = Arrays.asList(processInstanceIds);
		return this;
	  }

	  public virtual MigrationExecutionDtoBuilder processInstanceQuery(ProcessInstanceQueryDto processInstanceQuery)
	  {
		migrationExecution[PROP_PROCESS_INSTANCE_QUERY] = processInstanceQuery;
		return this;
	  }

	  public virtual MigrationPlanExecutionDtoBuilder migrationPlan(string sourceProcessDefinitionId, string targetProcessDefinitionId)
	  {
		return new MigrationPlanExecutionDtoBuilder(this, this, sourceProcessDefinitionId, targetProcessDefinitionId);
	  }

	  public virtual MigrationExecutionDtoBuilder migrationPlan(IDictionary<string, object> migrationPlan)
	  {
		migrationExecution[PROP_MIGRATION_PLAN] = migrationPlan;
		return this;
	  }

	  public virtual MigrationExecutionDtoBuilder skipCustomListeners(bool skipCustomListeners)
	  {
		migrationExecution[PROP_SKIP_CUSTOM_LISTENERS] = skipCustomListeners;
		return this;
	  }

	  public virtual MigrationExecutionDtoBuilder skipIoMappings(bool skipIoMappings)
	  {
		migrationExecution[PROP_SKIP_IO_MAPPINGS] = skipIoMappings;
		return this;
	  }

	  public virtual IDictionary<string, object> build()
	  {
		return migrationExecution;
	  }

	  public class MigrationPlanExecutionDtoBuilder : MigrationPlanDtoBuilder
	  {
		  private readonly MigrationExecutionDtoBuilder outerInstance;


		protected internal readonly MigrationExecutionDtoBuilder migrationExecutionDtoBuilder;

		public MigrationPlanExecutionDtoBuilder(MigrationExecutionDtoBuilder outerInstance, MigrationExecutionDtoBuilder migrationExecutionDtoBuilder, string sourceProcessDefinitionId, string targetProcessDefinitionId) : base(sourceProcessDefinitionId, targetProcessDefinitionId)
		{
			this.outerInstance = outerInstance;
		  this.migrationExecutionDtoBuilder = migrationExecutionDtoBuilder;
		}

		public override MigrationPlanExecutionDtoBuilder instruction(string sourceActivityId, string targetActivityId)
		{
		  base.instruction(sourceActivityId, targetActivityId);
		  return this;
		}

		public override MigrationPlanExecutionDtoBuilder instruction(string sourceActivityId, string targetActivityId, bool? updateEventTrigger)
		{
		  base.instruction(sourceActivityId, targetActivityId, updateEventTrigger);
		  return this;
		}

		public override MigrationPlanExecutionDtoBuilder instructions(IList<IDictionary<string, object>> instructions)
		{
		  base.instructions(instructions);
		  return this;
		}

		public override IDictionary<string, object> build()
		{
		  throw new System.NotSupportedException("Please use the done() method to finish the migration plan building");
		}

		public virtual MigrationExecutionDtoBuilder done()
		{
		  IDictionary<string, object> migrationPlan = base.build();
		  return migrationExecutionDtoBuilder.migrationPlan(migrationPlan);
		}
	  }

	}

}