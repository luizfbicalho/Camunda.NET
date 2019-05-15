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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{

	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using TaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.TaskActivityBehavior;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskItemHandler : ItemHandler
	{

	  protected internal override void initializeActivity(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		base.initializeActivity(element, activity, context);

		initializeBlocking(element, activity, context);
	  }

	  protected internal virtual void initializeBlocking(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		Task task = (Task) getDefinition(element);
		activity.setProperty("isBlocking", task.Blocking);
	  }

	  protected internal override CmmnActivityBehavior ActivityBehavior
	  {
		  get
		  {
			return new TaskActivityBehavior();
		  }
	  }

	  protected internal override IList<string> getStandardEvents(CmmnElement element)
	  {
		return TASK_OR_STAGE_EVENTS;
	  }

	}

}