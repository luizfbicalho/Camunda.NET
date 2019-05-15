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
namespace org.camunda.bpm.qa.upgrade.timestamp
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class TaskCreateTimeScenario : AbstractTimestampMigrationScenario
	{

	  protected internal const string TASK_NAME = "createTimeTestTask";

	  [DescribesScenario("initCreateTime"), Times(1)]
	  public static ScenarioSetup initCreateTime()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine processEngine, string s)
		  {

			TaskEntity task = TaskEntity.create();
			task.Name = TASK_NAME;
			task.CreateTime = TIMESTAMP;
			processEngine.TaskService.saveTask(task);
		  }
	  }
	}
}