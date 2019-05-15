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
namespace org.camunda.bpm.engine.impl.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.ACTIVITY_INSTANCE_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.ACTIVITY_INSTANCE_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.ACTIVITY_INSTANCE_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.ACTIVITY_INSTANCE_MIGRATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.CASE_ACTIVITY_INSTANCE_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.CASE_ACTIVITY_INSTANCE_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.CASE_INSTANCE_CLOSE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.CASE_INSTANCE_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.CASE_INSTANCE_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.PROCESS_INSTANCE_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.PROCESS_INSTANCE_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.PROCESS_INSTANCE_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.PROCESS_INSTANCE_MIGRATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.TASK_INSTANCE_COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.TASK_INSTANCE_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.TASK_INSTANCE_DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.TASK_INSTANCE_MIGRATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.TASK_INSTANCE_UPDATE;

	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HistoryLevelActivity : AbstractHistoryLevel
	{

	  public override int Id
	  {
		  get
		  {
			return 1;
		  }
	  }

	  public override string Name
	  {
		  get
		  {
			return ProcessEngineConfiguration.HISTORY_ACTIVITY;
		  }
	  }

	  public override bool isHistoryEventProduced(HistoryEventType eventType, object entity)
	  {
		return PROCESS_INSTANCE_START == eventType || PROCESS_INSTANCE_UPDATE == eventType || PROCESS_INSTANCE_MIGRATE == eventType || PROCESS_INSTANCE_END == eventType || TASK_INSTANCE_CREATE == eventType || TASK_INSTANCE_UPDATE == eventType || TASK_INSTANCE_MIGRATE == eventType || TASK_INSTANCE_COMPLETE == eventType || TASK_INSTANCE_DELETE == eventType || ACTIVITY_INSTANCE_START == eventType || ACTIVITY_INSTANCE_UPDATE == eventType || ACTIVITY_INSTANCE_MIGRATE == eventType || ACTIVITY_INSTANCE_END == eventType || CASE_INSTANCE_CREATE == eventType || CASE_INSTANCE_UPDATE == eventType || CASE_INSTANCE_CLOSE == eventType || CASE_ACTIVITY_INSTANCE_CREATE == eventType || CASE_ACTIVITY_INSTANCE_UPDATE == eventType || CASE_ACTIVITY_INSTANCE_END == eventType;
	  }

	}

}