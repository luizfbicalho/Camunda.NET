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
	using MigratingInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingInstance;
	using MigratingProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance;
	using MigratingVariableInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingVariableInstance;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;

	/// <summary>
	/// Validates that when an activity instance has a variable with the same name twice (as a scope execution variable and a
	/// a concurrent variable parent execution variable), no situation occurs in which either one is overwritten.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class VariableConflictActivityInstanceValidator : MigratingActivityInstanceValidator
	{

	  public virtual void validate(MigratingActivityInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingActivityInstanceValidationReportImpl instanceReport)
	  {

		ScopeImpl sourceScope = migratingInstance.SourceScope;
		ScopeImpl targetScope = migratingInstance.TargetScope;

		if (migratingInstance.migrates())
		{
		  bool becomesNonScope = sourceScope.Scope && !targetScope.Scope;
		  if (becomesNonScope)
		  {
			IDictionary<string, IList<MigratingVariableInstance>> dependentVariablesByName = getMigratingVariableInstancesByName(migratingInstance);
			foreach (string variableName in dependentVariablesByName.Keys)
			{
			  if (dependentVariablesByName[variableName].Count > 1)
			  {
				instanceReport.addFailure("The variable '" + variableName + "' exists in both, this scope and " + "concurrent local in the parent scope. " + "Migrating to a non-scope activity would overwrite one of them.");
			  }
			}
		  }
		}
	  }

	  protected internal virtual IDictionary<string, IList<MigratingVariableInstance>> getMigratingVariableInstancesByName(MigratingActivityInstance activityInstance)
	  {
		IDictionary<string, IList<MigratingVariableInstance>> result = new Dictionary<string, IList<MigratingVariableInstance>>();

		foreach (MigratingInstance migratingInstance in activityInstance.MigratingDependentInstances)
		{
		  if (migratingInstance is MigratingVariableInstance)
		  {
			MigratingVariableInstance migratingVariableInstance = (MigratingVariableInstance) migratingInstance;
			CollectionUtil.addToMapOfLists(result, migratingVariableInstance.VariableName, migratingVariableInstance);
		  }
		}

		return result;
	  }

	}

}