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
namespace org.camunda.bpm.engine.impl.migration
{
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingInstance;
	using MigratingProcessElementInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessElementInstance;
	using MigratingScopeInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingScopeInstance;
	using MigratingProcessInstanceValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingProcessInstanceValidationReportImpl;
	using MigrationPlanValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instruction.MigrationPlanValidationReportImpl;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationLogger : ProcessEngineLogger
	{

	  public virtual MigrationPlanValidationException failingMigrationPlanValidation(MigrationPlanValidationReportImpl validationReport)
	  {
		StringBuilder sb = new StringBuilder();
		validationReport.writeTo(sb);
		return new MigrationPlanValidationException(exceptionMessage("001", "{}", sb.ToString()), validationReport);
	  }

	  public virtual ProcessEngineException processDefinitionOfInstanceDoesNotMatchMigrationPlan(ExecutionEntity processInstance, string processDefinitionId)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Process instance '{}' cannot be migrated. Its process definition '{}' does not match the source process definition of the migration plan '{}'", processInstance.Id, processInstance.ProcessDefinitionId, processDefinitionId));
	  }

	  public virtual ProcessEngineException processInstanceDoesNotExist(string processInstanceId)
	  {
		return new ProcessEngineException(exceptionMessage("003", "Process instance '{}' cannot be migrated. The process instance does not exist", processInstanceId));
	  }

	  public virtual MigratingProcessInstanceValidationException failingMigratingProcessInstanceValidation(MigratingProcessInstanceValidationReportImpl validationReport)
	  {
		StringBuilder sb = new StringBuilder();
		validationReport.writeTo(sb);
		return new MigratingProcessInstanceValidationException(exceptionMessage("004", "{}", sb.ToString()), validationReport);
	  }

	  public virtual ProcessEngineException cannotBecomeSubordinateInNonScope(MigratingActivityInstance activityInstance)
	  {
		return new ProcessEngineException(exceptionMessage("005", "{}", "Cannot attach a subordinate to activity instance '{}'. Activity '{}' is not a scope", activityInstance.ActivityInstance.Id, activityInstance.ActivityInstance.ActivityId));
	  }

	  public virtual ProcessEngineException cannotDestroySubordinateInNonScope(MigratingActivityInstance activityInstance)
	  {
		return new ProcessEngineException(exceptionMessage("006", "{}", "Cannot destroy a subordinate of activity instance '{}'. Activity '{}' is not a scope", activityInstance.ActivityInstance.Id, activityInstance.ActivityInstance.ActivityId));
	  }

	  public virtual ProcessEngineException cannotAttachToTransitionInstance(MigratingInstance attachingInstance)
	  {
		return new ProcessEngineException(exceptionMessage("007", "{}", "Cannot attach instance '{}' to a transition instance", attachingInstance));
	  }

	  public virtual BadUserRequestException processDefinitionDoesNotExist(string processDefinitionId, string type)
	  {
		return new BadUserRequestException(exceptionMessage("008", "{} process definition with id '{}' does not exist", type, processDefinitionId));
	  }

	  public virtual ProcessEngineException cannotMigrateBetweenTenants(string sourceTenantId, string targetTenantId)
	  {
		return new ProcessEngineException(exceptionMessage("09", "Cannot migrate process instances between processes of different tenants ('{}' != '{}')", sourceTenantId, targetTenantId));
	  }

	  public virtual ProcessEngineException cannotMigrateInstanceBetweenTenants(string processInstanceId, string sourceTenantId, string targetTenantId)
	  {

		string detailMessage = null;
		if (!string.ReferenceEquals(sourceTenantId, null))
		{
		  detailMessage = exceptionMessage("010", "Cannot migrate process instance '{}' to a process definition of a different tenant ('{}' != '{}')", processInstanceId, sourceTenantId, targetTenantId);
		}
		else
		{
		  detailMessage = exceptionMessage("010", "Cannot migrate process instance '{}' without tenant to a process definition with a tenant ('{}')", processInstanceId, targetTenantId);
		}

		return new ProcessEngineException(detailMessage);
	  }

	  public virtual ProcessEngineException cannotHandleChild(MigratingScopeInstance scopeInstance, MigratingProcessElementInstance childCandidate)
	  {
		return new ProcessEngineException(exceptionMessage("011", "Scope instance of type {} cannot have child of type {}", scopeInstance.GetType().Name, childCandidate.GetType().Name));
	  }

	}

}