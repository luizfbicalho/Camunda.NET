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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class EventSubscriptionCreateTimeScenario : AbstractTimestampMigrationScenario
	{

	  protected internal const string EVENT_NAME = "createTimeTestMessage";
	  protected internal const string ACTIVITY_ID = "createTimeTestActivity";

	  [DescribesScenario("initEventSubscriptionCreateTime"), Times(1)]
	  public static ScenarioSetup initEventSubscriptionCreateTime()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine processEngine, string s)
		  {

			((ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		  }

		  private class CommandAnonymousInnerClass : Command<Void>
		  {
			  private readonly ScenarioSetupAnonymousInnerClass outerInstance;

			  public CommandAnonymousInnerClass(ScenarioSetupAnonymousInnerClass outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }


			  public Void execute(CommandContext commandContext)
			  {

				EventSubscriptionEntity messageEventSubscriptionEntity = new EventSubscriptionEntity(EventType.MESSAGE);
				messageEventSubscriptionEntity.EventName = EVENT_NAME;
				messageEventSubscriptionEntity.ActivityId = ACTIVITY_ID;
				messageEventSubscriptionEntity.Created = TIMESTAMP;
				messageEventSubscriptionEntity.insert();

				return null;
			  }
		  }
	  }
	}
}