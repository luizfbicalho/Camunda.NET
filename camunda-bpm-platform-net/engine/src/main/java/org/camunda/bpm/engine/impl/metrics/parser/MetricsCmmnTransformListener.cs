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
namespace org.camunda.bpm.engine.impl.metrics.parser
{
	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using AbstractCmmnTransformListener = org.camunda.bpm.engine.impl.cmmn.transformer.AbstractCmmnTransformListener;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MetricsCmmnTransformListener : AbstractCmmnTransformListener
	{

	  public static MetricsCaseExecutionListener listener = new MetricsCaseExecutionListener();

	  protected internal virtual void addListeners(CmmnActivity activity)
	  {
		if (activity != null)
		{
		  activity.addBuiltInListener(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.START, listener);
		  activity.addBuiltInListener(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.MANUAL_START, listener);
		  activity.addBuiltInListener(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.OCCUR, listener);
		}
	  }

	  public override void transformHumanTask(PlanItem planItem, HumanTask humanTask, CmmnActivity activity)
	  {
		addListeners(activity);
	  }

	  public override void transformProcessTask(PlanItem planItem, ProcessTask processTask, CmmnActivity activity)
	  {
		addListeners(activity);
	  }

	  public override void transformCaseTask(PlanItem planItem, CaseTask caseTask, CmmnActivity activity)
	  {
		addListeners(activity);
	  }

	  public override void transformDecisionTask(PlanItem planItem, DecisionTask decisionTask, CmmnActivity activity)
	  {
		addListeners(activity);
	  }

	  public override void transformTask(PlanItem planItem, Task task, CmmnActivity activity)
	  {
		addListeners(activity);
	  }

	  public override void transformStage(PlanItem planItem, Stage stage, CmmnActivity activity)
	  {
		addListeners(activity);
	  }

	  public override void transformMilestone(PlanItem planItem, Milestone milestone, CmmnActivity activity)
	  {
		addListeners(activity);
	  }

	}

}