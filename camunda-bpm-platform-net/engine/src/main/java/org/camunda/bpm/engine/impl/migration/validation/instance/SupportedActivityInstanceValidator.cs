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
	using MigratingProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance;
	using SupportedActivityValidator = org.camunda.bpm.engine.impl.migration.validation.activity.SupportedActivityValidator;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SupportedActivityInstanceValidator : MigratingActivityInstanceValidator
	{

	  public virtual void validate(MigratingActivityInstance migratingInstance, MigratingProcessInstance migratingProcessInstance, MigratingActivityInstanceValidationReportImpl instanceReport)
	  {

		ScopeImpl sourceScope = migratingInstance.SourceScope;

		if (sourceScope != sourceScope.ProcessDefinition)
		{
		  ActivityImpl sourceActivity = (ActivityImpl) migratingInstance.SourceScope;

		  if (!SupportedActivityValidator.INSTANCE.isSupportedActivity(sourceActivity))
		  {
			instanceReport.addFailure("The type of the source activity is not supported for activity instance migration");
		  }
		}
	  }

	}

}