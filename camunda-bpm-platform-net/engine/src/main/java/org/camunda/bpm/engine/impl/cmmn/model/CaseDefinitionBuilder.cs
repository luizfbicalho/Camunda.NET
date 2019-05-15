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
namespace org.camunda.bpm.engine.impl.cmmn.model
{

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using StageActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.StageActivityBehavior;
	using CoreModelElement = org.camunda.bpm.engine.impl.core.model.CoreModelElement;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseDefinitionBuilder
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			processElement = caseDefinition;
		}


	  protected internal CmmnCaseDefinition caseDefinition;
	  protected internal CmmnActivity casePlanModel;
	  protected internal Stack<CmmnActivity> activityStack = new Stack<CmmnActivity>();
	  protected internal CoreModelElement processElement;

	  public CaseDefinitionBuilder() : this(null)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public CaseDefinitionBuilder(string caseDefinitionId)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		// instantiate case definition
		caseDefinition = new CmmnCaseDefinition(caseDefinitionId);
		activityStack.Push(caseDefinition);

		// instantiate casePlanModel of case definition (ie. outermost stage)
		createActivity(caseDefinitionId);
		behavior(new StageActivityBehavior());
	  }

	  public virtual CaseDefinitionBuilder createActivity(string id)
	  {
		CmmnActivity activity = activityStack.Peek().createActivity(id);
		activityStack.Push(activity);
		processElement = activity;

		return this;
	  }

	  public virtual CaseDefinitionBuilder endActivity()
	  {
		activityStack.Pop();
		processElement = activityStack.Peek();

		return this;
	  }

	  public virtual CaseDefinitionBuilder behavior(CmmnActivityBehavior behavior)
	  {
		Activity.ActivityBehavior = behavior;
		return this;
	  }

	  public virtual CaseDefinitionBuilder autoComplete(bool autoComplete)
	  {
		Activity.setProperty("autoComplete", autoComplete);
		return this;
	  }

	  protected internal virtual CmmnActivity Activity
	  {
		  get
		  {
			return activityStack.Peek();
		  }
	  }

	  public virtual CmmnCaseDefinition buildCaseDefinition()
	  {
		return caseDefinition;
	  }

	  public virtual CaseDefinitionBuilder listener(string eventName, CaseExecutionListener planItemListener)
	  {
		activityStack.Peek().addListener(eventName, planItemListener);
		return this;
	  }

	  public virtual CaseDefinitionBuilder property(string name, object value)
	  {
		activityStack.Peek().setProperty(name, value);
		return this;
	  }

	}

}