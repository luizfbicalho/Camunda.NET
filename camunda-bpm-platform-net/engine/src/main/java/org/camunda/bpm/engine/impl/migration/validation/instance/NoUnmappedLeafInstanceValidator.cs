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
	using MigratingCompensationEventSubscriptionInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingCompensationEventSubscriptionInstance;
	using MigratingEventScopeInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingEventScopeInstance;
	using MigratingProcessElementInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessElementInstance;
	using MigratingProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance;
	using MigratingTransitionInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingTransitionInstance;

	public class NoUnmappedLeafInstanceValidator : MigratingActivityInstanceValidator, MigratingTransitionInstanceValidator, MigratingCompensationInstanceValidator
	{

	  public virtual void validate(MigratingActivityInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingActivityInstanceValidationReportImpl instanceReport)
	  {
		if (isInvalid(migratingInstance))
		{
		  instanceReport.addFailure("There is no migration instruction for this instance's activity");
		}
	  }

	  public virtual void validate(MigratingTransitionInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingTransitionInstanceValidationReportImpl instanceReport)
	  {
		if (isInvalid(migratingInstance))
		{
		  instanceReport.addFailure("There is no migration instruction for this instance's activity");
		}
	  }

	  public virtual void validate(MigratingCompensationEventSubscriptionInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
	  {
		if (isInvalid(migratingInstance))
		{
		  ancestorInstanceReport.addFailure("Cannot migrate subscription for compensation handler '" + migratingInstance.SourceScope.Id + "'. " + "There is no migration instruction for the compensation boundary event");
		}
	  }

	  public virtual void validate(MigratingEventScopeInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingActivityInstanceValidationReportImpl ancestorInstanceReport)
	  {
		if (isInvalid(migratingInstance))
		{
		  ancestorInstanceReport.addFailure("Cannot migrate subscription for compensation handler '" + migratingInstance.EventSubscription.SourceScope.Id + "'. " + "There is no migration instruction for the compensation start event");
		}
	  }

	  protected internal virtual bool isInvalid(MigratingActivityInstance migratingInstance)
	  {
		return hasNoInstruction(migratingInstance) && migratingInstance.Children.Count == 0;
	  }

	  protected internal virtual bool isInvalid(MigratingEventScopeInstance migratingInstance)
	  {
		return hasNoInstruction(migratingInstance.EventSubscription) && migratingInstance.Children.Count == 0;
	  }

	  protected internal virtual bool isInvalid(MigratingTransitionInstance migratingInstance)
	  {
		return hasNoInstruction(migratingInstance);
	  }

	  protected internal virtual bool isInvalid(MigratingCompensationEventSubscriptionInstance migratingInstance)
	  {
		return hasNoInstruction(migratingInstance);
	  }

	  protected internal virtual bool hasNoInstruction(MigratingProcessElementInstance migratingInstance)
	  {
		return migratingInstance.MigrationInstruction == null;
	  }
	}

}